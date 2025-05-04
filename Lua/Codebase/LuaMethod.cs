using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lua.CodeBlock;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Lua.Codebase
{
    // Concrete implementation of a Lua method
    public class LuaMethod : ILuaMethod
    {
        // Name of the method, used for assets research
        protected string type = "LuaMethod";  
        
        // Numbers of parameters represented in different formats in code block UI
        protected int inputFieldNum;    // float, int, free string
        protected int dropdownNum;      // object or assets reference
        protected int toggleNum;        // bool
        
        // Prefab of the corresponding code block UI ("CB_MethodName")
        protected GameObject prefab;

        // Method parameters
        protected List<IParameter> parameters = new List<IParameter>();
        protected List<IParameter> dropdownParameters = new List<IParameter>();
        protected List<bool> toggleParameters = new List<bool>();

        // Constructor
        public LuaMethod(string type, int inputFieldNum = 0, int dropdownNum = 0, int toggleNum = 0)
        {
            this.inputFieldNum = inputFieldNum;
            this.dropdownNum = dropdownNum;
            this.toggleNum = toggleNum;
            this.type = type;
            prefab = Resources.Load<GameObject>("CodeBlock/CB_" + type); // load UI prefab from Resources
        }

        // To be overridden
        public virtual Task executeFunction()
        {
            return Task.CompletedTask;
        }

        // Instantiates the basic UI and initialize the values
        public virtual GameObject drawCodeBlock(GameObject canvas)
        {
            if (prefab == null)
            {
                Debug.LogError(type + ": failed to find code block prefab");
                return null;
            }

            // Instantiate the prefab
            GameObject codeBlock = GameObject.Instantiate(prefab, canvas.transform);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();

            // Validate the number of parameters
            if (prefab.GetComponent<CBPrefab>() == null 
                || cb.inputFields.Length != inputFieldNum 
                || cb.dropdownFields.Length != dropdownNum
                || cb.toggles.Length != toggleNum)
            {
                Debug.LogWarning(type + ": failed to instantiate code block - wrong parameter number");
                GameObject.Destroy(codeBlock);
                return null;
            }

            // Set input fields with saved values
            for (int i = 0; i < inputFieldNum; ++i)
            {
                cb.inputFields[i].text = parameters[i].ToString();
            }

            // Set dropdown fields
            for (int i = 0; i < dropdownNum; ++i)
            {
                var dropdown = cb.dropdownFields[i].GetComponent<CBResourcesDropdown>();
                if (!dropdown.skipAwakeSetup)
                    dropdown.setDropdownValue(dropdownParameters[i].ToString());
            }

            // Set toggle states
            for (int i = 0; i < toggleNum; ++i)
            {
                cb.toggles[i].GetComponent<Toggle>().isOn = toggleParameters[i];
            }

            return codeBlock;
        }

        // To be overridden
        public virtual string getCodeText()
        {
            return "";
        }

        // Conversion to a JSON representation
        public virtual JObject toJSON()
        {
            JObject parameterData = new JObject(parameters
                .Select((param, index) => new JProperty($"parameter{index}", param.ToString())));

            JObject dropdownData = new JObject(dropdownParameters
                .Select((param, index) => new JProperty($"dropdown{index}", param.ToString())));

            JObject methodData = new JObject
            {
                ["type"] = type,
                ["parameter"] = parameterData,
                ["dropdown"] = dropdownData
            };

            return methodData;
        }

        // General conversion to a line of Lua code with all parameters
        public virtual string getLuaCode()
        {
            var parametersList = new List<string>();

            // Dropdown parameters come first
            if (dropdownNum > 0)
                parametersList.AddRange(dropdownParameters.Select(p => p.ToLuaString()));

            // Then input field parameters
            if (inputFieldNum > 0)
                parametersList.AddRange(parameters.Select(p => p.ToLuaString()));

            // Finally, boolean toggles
            if (toggleNum > 0)
                parametersList.AddRange(toggleParameters.Select(p => p ? "true" : "false"));

            return $"{type}({string.Join(", ", parametersList)})\n";
        }
    }
}

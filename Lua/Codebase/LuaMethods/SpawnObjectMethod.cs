using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;
using UnityEngine.UI;

namespace Lua.Codebase
{
    public class SpawnObjectMethod : LuaMethod
    {
        /* TODO: bool toggle is a special kind of CB component */
        public SpawnObjectMethod(string prefabName, string objectName, float x, float y, float z, bool eyes) 
            : base("SpawnObject", 4, 1, 1)
        {
            dropdownParameters.Add(new StringParameter(prefabName));
            parameters.Add(new StringParameter(objectName));
            parameters.Add(new FloatParameter(x));
            parameters.Add(new FloatParameter(y));
            parameters.Add(new FloatParameter(z));
            toggleParameters.Add(eyes);
        }
        
        public override Task executeFunction()
        {
            LuaCodebase.SpawnObject((string)dropdownParameters[0].Get(),
                (string)parameters[0].Get(), 
                (float)parameters[1].Get(), 
                (float)parameters[2].Get(), 
                (float)parameters[3].Get(),
                toggleParameters[0]);
            return Task.CompletedTask;
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject prefab = Resources.Load<GameObject>(\"{dropdownParameters[0]}\");\n" +
                $"GameObject newObject = Instantiate(prefab, new Vector3({string.Join(", ", parameters.Skip(1))}), Quaternion.identity);\n" +
                $"newObject.name = \"{parameters[0]}\";\n";
            return codeText;
        }

        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();
            
            for (int i = 1; i <= 3; ++i)
            {
                int index = i;
                cb.inputFields[i].onValueChanged.AddListener((str) =>
                {
                    parameters[index].Set(float.Parse(str));
                    LuaCodebase.UpdateSpawnObject((string)parameters[0].Get(), 
                        (float)parameters[1].Get(), 
                        (float)parameters[2].Get(), 
                        (float)parameters[3].Get());
                });
            }
            cb.inputFields[0].onEndEdit.AddListener((str) =>
            {
                LuaCodebase.UpdateSpawnObject((string)parameters[0].Get(), str);
                parameters[0].Set(float.Parse(str));
            });
            cb.dropdownFields[0].onValueChanged.AddListener((index) =>
            {
                dropdownParameters[0].Set(cb.dropdownFields[0].GetComponent<CBResourcesDropdown>().getDropdownValue(index));
                LuaCodebase.UpdateSpawnObject((string)dropdownParameters[0].Get(),
                    (string)parameters[0].Get(), 
                    (float)parameters[1].Get(), 
                    (float)parameters[2].Get(), 
                    (float)parameters[3].Get(),
                    toggleParameters[0]);
            });

            Toggle toggle = codeBlock.GetComponentInChildren<Toggle>();
            toggle.isOn = toggleParameters[0];
            toggle.onValueChanged.AddListener((value) =>
            {
                toggleParameters[0] = toggle.isOn;
                LuaCodebase.ChangeEyes((string)parameters[0].Get(), toggleParameters[0]);
            });
            
            return codeBlock;
        }
    }
}
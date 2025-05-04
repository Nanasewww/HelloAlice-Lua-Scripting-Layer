using System.Collections.Generic;
using System.Linq;
using Lua.CodeBlock;
using UnityEngine;
using System.Threading.Tasks;

namespace Lua.Codebase
{
    public class PlayVisualEffectMethod : LuaMethod
    {
        private string objectName;
        private string name;
        public PlayVisualEffectMethod(string vfxName, float x, float y, float z, string objectName) 
            : base("PlayVisualEffect", 3, 1)
        {
            parameters.Add(new FloatParameter(x));
            parameters.Add(new FloatParameter(y));
            parameters.Add(new FloatParameter(z));
            dropdownParameters.Add(new StringParameter(vfxName));
            this.objectName = "vfx_" + vfxName + "_" + objectName;
            name = objectName;
        }

        public override Task executeFunction()
        {
            LuaCodebase.PlayVisualEffect((string)dropdownParameters[0].Get(),
                (float)parameters[0].Get(),
                (float)parameters[1].Get(),
                (float)parameters[2].Get(),
                objectName);
            return Task.CompletedTask;
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject prefab = Resources.Load<GameObject>(\"{dropdownParameters[0]}\");\n" +
                $"GameObject newVFX = Instantiate(prefab, new Vector3({string.Join(", ", parameters)}), Quaternion.identity);\n";
            return codeText;
        }
        
        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();
            cb.dropdownFields[0].onValueChanged.AddListener((index) =>
            {
                dropdownParameters[0].Set(cb.dropdownFields[0].GetComponent<CBResourcesDropdown>().getDropdownValue(index));
                LuaCodebase.UpdateVisualEffect(
                    (string)dropdownParameters[0].Get(),
                    (float)parameters[0].Get(),
                    (float)parameters[1].Get(),
                    (float)parameters[2].Get(),
                    objectName);
            });
            
            for (int i = 0; i < 3; ++i)
            {
                int index = i;
                cb.inputFields[i].onValueChanged.AddListener((str) =>
                {
                    parameters[index].Set(float.Parse(str));
                    LuaCodebase.UpdateVisualEffect(
                        (string)dropdownParameters[0].Get(),
                        (float)parameters[0].Get(),
                        (float)parameters[1].Get(),
                        (float)parameters[2].Get(),
                        objectName);
                });
            }
            
            return codeBlock;
        }
        
        public override string getLuaCode()
        {
            var parametersList = new List<string>();
            
            if (dropdownNum > 0)
                parametersList.AddRange(dropdownParameters.Select(p => p.ToLuaString()));
            
            if (inputFieldNum > 0)
                parametersList.AddRange(parameters.Select(p => p.ToLuaString()));
            
            return $"{type}({string.Join(", ", parametersList)}, '{name}')\n";

        }
    }
}
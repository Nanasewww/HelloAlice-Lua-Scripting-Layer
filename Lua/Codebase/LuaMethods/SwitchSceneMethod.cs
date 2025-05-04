using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;

namespace Lua.Codebase
{
    public class SwitchSceneMethod : LuaMethod
    {
        public SwitchSceneMethod(string sceneName) : base("SwitchScene", 0, 1)
        {
            dropdownParameters.Add(new StringParameter(sceneName));
        }
        
        public override async Task executeFunction()
        {
            await LuaCodebase.SwitchScene((string)dropdownParameters[0].Get());
        }

        public override string getCodeText()
        {
            string codeText = $"SceneManager.LoadScene(\"{dropdownParameters[0]}\");\n";
            return codeText;
        }

        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();
            cb.dropdownFields[0].onValueChanged.AddListener((index) =>
            {
                dropdownParameters[0].Set(cb.dropdownFields[0].GetComponent<CBResourcesDropdown>().getDropdownValue(index));
            });
            
            return codeBlock;
        }
    }
}
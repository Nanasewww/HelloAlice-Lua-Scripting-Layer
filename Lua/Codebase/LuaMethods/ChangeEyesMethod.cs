using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;
using UnityEngine.UI;

namespace Lua.Codebase
{
    public class ChangeEyesMethod : LuaMethod
    {
        public ChangeEyesMethod(string objectName, bool hasEyes) : base("ChangeEyes", 1, 0, 1)
        {
            parameters.Add(new StringParameter(objectName));
            toggleParameters.Add(hasEyes);
        }
        
        public override Task executeFunction()
        {
            LuaCodebase.ChangeEyes((string)parameters[0].Get(), toggleParameters[0]);
            return Task.CompletedTask;
        }
        
        public override string getCodeText()
        {
            string codeText = $"GameObject.Find(\"{parameters[0]}\").SetEyes({toggleParameters[0]})\n";
            return codeText;
        }
        
        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();

            Toggle toggle = cb.toggles[0];
            toggle.onValueChanged.AddListener((value) =>
            {
                toggleParameters[0] = value;
                executeFunction();
            });
            
            return codeBlock;
        }
    }
}
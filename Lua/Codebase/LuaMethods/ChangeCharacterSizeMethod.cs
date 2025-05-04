using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;

namespace Lua.Codebase
{
    public class ChangeCharacterSizeMethod : LuaMethod
    {
        public ChangeCharacterSizeMethod(float scale) : base("ChangeCharacterSize", 1)
        {
            parameters.Add(new FloatParameter(scale));
        }

        public override Task executeFunction()
        {
            LuaCodebase.ChangeCharacterSize((float)parameters[0].Get());
            return Task.CompletedTask;
        }
        
        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            codeBlock.GetComponent<CBPrefab>().
                inputFields[0].onValueChanged.AddListener((str) =>
            {
                parameters[0].Set(float.Parse(str));
                executeFunction();
            });
            
            return codeBlock;
        }

        public override string getCodeText()
        {
            string codeText =
                $"character.transform.localScale = new Vector3({parameters[0]}, {parameters[0]}, {parameters[0]});\n";
            return codeText;
        }
    }
}
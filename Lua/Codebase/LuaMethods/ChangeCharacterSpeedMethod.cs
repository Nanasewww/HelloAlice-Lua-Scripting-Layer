using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;

namespace Lua.Codebase
{
    public class ChangeCharacterSpeedMethod  : LuaMethod
    {
        public ChangeCharacterSpeedMethod(float speed) : base("ChangeCharacterSpeed", 1)
        {
            parameters.Add(new FloatParameter(speed));
        }

        public override Task executeFunction()
        {
            LuaCodebase.ChangeCharacterSpeed((float)parameters[0].Get());
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
                $"character.speed = {parameters[0]};\n";
            return codeText;
        }
    }
}
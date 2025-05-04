using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;

namespace Lua.Codebase
{
    public class DelayMethod : LuaMethod
    {
        public DelayMethod(float delayTime) : base("Delay", 1)
        {
            parameters.Add(new FloatParameter(delayTime));
        }
        
        public override async Task executeFunction()
        {
            await LuaCodebase.Delay((float)parameters[0].Get());
        }
        
        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            codeBlock.GetComponent<CBPrefab>().
                inputFields[0].onValueChanged.AddListener((str) =>
                {
                    parameters[0].Set(float.Parse(str));
                });
            
            return codeBlock;
        }

        public override string getCodeText()
        {
            string codeText = $"Delay({parameters[0]})\n";
            return codeText;
        }
    }
}
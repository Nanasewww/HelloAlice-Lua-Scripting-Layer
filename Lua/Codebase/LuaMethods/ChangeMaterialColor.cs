using System.Linq;
using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;

namespace Lua.Codebase
{
    public class ChangeMaterialColor : LuaMethod
    {
        public ChangeMaterialColor(string name, int x, int y, int z) 
            : base("ChangeMaterialColor", 4, 0)
        {
            parameters.Add(new StringParameter(name));
            parameters.Add(new IntParameter(x));
            parameters.Add(new IntParameter(y));
            parameters.Add(new IntParameter(z));
        }
        
        public override Task executeFunction()
        {
            LuaCodebase.ChangeMaterialColor((string)parameters[0].Get(), 
                (int)parameters[1].Get(), 
                (int)parameters[2].Get(), 
                (int)parameters[3].Get());
            return Task.CompletedTask;
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
                    parameters[index].Set(int.Parse(cb.inputFields[index].text));
                    executeFunction();
                });
            }
            
            return codeBlock;
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject obj = GameObject.Find(\"{parameters[0]}\");\n" +
                $"Renderer renderer = obj.GetComponent<Renderer>();" +
                $"renderer.material.color = new Color({string.Join(", ", parameters.Skip(1))});\n";
            return codeText;
        }
    }
}
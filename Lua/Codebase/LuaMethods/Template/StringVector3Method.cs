using Lua.CodeBlock;
using UnityEngine;

namespace Lua.Codebase.Template
{
    public class StringVector3Method : LuaMethod
    {
        public StringVector3Method(string type, string name, float x, float y, float z, bool progress) 
            : base(type, 4, 0, 1)
        {
            parameters.Add(new StringParameter(name));
            parameters.Add(new FloatParameter(x));
            parameters.Add(new FloatParameter(y));
            parameters.Add(new FloatParameter(z));
            toggleParameters.Add(progress);
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
                    if (toggleParameters[0]) return;
                    executeFunction();
                });
            }
            cb.toggles[0].onValueChanged.AddListener((value) =>
            {
                toggleParameters[0] = value;
                executeFunction();
            });
            
            return codeBlock;
        }
    }
}
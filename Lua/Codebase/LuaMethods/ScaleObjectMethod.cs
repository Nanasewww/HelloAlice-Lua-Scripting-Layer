using System.Linq;
using Lua.Codebase.Template;
using System.Threading.Tasks;
using UnityEngine;

namespace Lua.Codebase
{
    public class ScaleObjectMethod : StringVector3Method
    {
        private Vector3 startScale;
        public ScaleObjectMethod(string name, float x, float y, float z, bool progress) 
            : base("ScaleObject", name, x, y, z, progress)
        {
            startScale = LuaCodebase.GetObjectTransform(name).localScale;
        }
        
        public override async Task executeFunction()
        {
            await LuaCodebase.ScaleObject((string)parameters[0].Get(), 
                (float)parameters[1].Get(), 
                (float)parameters[2].Get(), 
                (float)parameters[3].Get(),
                toggleParameters[0]);
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject obj = GameObject.Find(\"{parameters[0]}\");\n" +
                $"obj.transform.localScale = new Vector3({string.Join(", ", parameters.Skip(1))});\n";
            return codeText;
        }
    }
}
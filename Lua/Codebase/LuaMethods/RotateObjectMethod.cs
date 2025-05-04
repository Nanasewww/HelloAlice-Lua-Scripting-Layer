using System.Linq;
using Lua.Codebase.Template;
using System.Threading.Tasks;
using UnityEngine;

namespace Lua.Codebase
{
    public class RotateObjectMethod : StringVector3Method
    {
        private Vector3 startRotate;
        public RotateObjectMethod(string name, float x, float y, float z, bool progress) 
            : base("RotateObject", name, x, y, z, progress)
        {
            startRotate = LuaCodebase.GetObjectTransform(name).localScale;
        }
        
        public override async Task executeFunction()
        {
            await LuaCodebase.RotateObject((string)parameters[0].Get(), 
                (float)parameters[1].Get(), 
                (float)parameters[2].Get(), 
                (float)parameters[3].Get(),
                toggleParameters[0]);
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject obj = GameObject.Find(\"{parameters[0]}\");\n" +
                $"obj.transform.eulerAngles = new Vector3({string.Join(", ", parameters.Skip(1))});\n";
            return codeText;
        }
    }
}
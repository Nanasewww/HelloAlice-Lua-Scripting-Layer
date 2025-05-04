using System.Linq;
using Lua.Codebase.Template;
using System.Threading.Tasks;
using UnityEngine;

namespace Lua.Codebase
{
    public class MoveObjectMethod : StringVector3Method
    {
        private Vector3 startPosition;
        public MoveObjectMethod(string name, float x, float y, float z, bool progress) : base("MoveObject", name, x, y, z, progress)
        {
            startPosition = LuaCodebase.GetObjectTransform(name).position;
        }
        
        public override async Task executeFunction()
        {
            await LuaCodebase.MoveObject((string)parameters[0].Get(), 
                (float)parameters[1].Get(), 
                (float)parameters[2].Get(), 
                (float)parameters[3].Get(),
                toggleParameters[0]);
            //return Task.CompletedTask;
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject obj = GameObject.Find(\"{parameters[0]}\");\n" +
                $"obj.transform.position = new Vector3({string.Join(", ", parameters.Skip(1))});\n";
            return codeText;
        }
    }
}
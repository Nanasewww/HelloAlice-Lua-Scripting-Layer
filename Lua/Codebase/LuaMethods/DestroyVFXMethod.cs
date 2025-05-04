using System.Threading.Tasks;

namespace Lua.Codebase
{
    public class DestroyVFXMethod : LuaMethod
    {
        public DestroyVFXMethod(string name) : base("DestroyVisualEffect", 1, 0)
        {
            parameters.Add(new StringParameter(name));
        }
        
        public override Task executeFunction()
        {
            LuaCodebase.DestroyVFX((string)parameters[0].Get());
            return Task.CompletedTask;
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject vfx = GameObject.Find(\"{parameters[0]}\");\n" +
                $"Destroy(vfx);\n";
            return codeText;
        }
    }
}
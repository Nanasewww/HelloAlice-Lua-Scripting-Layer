using System.Threading.Tasks;
namespace Lua.Codebase
{
    public class DestroyObjectMethod : LuaMethod
    {
       public DestroyObjectMethod(string name) : base("DestroyObject", 1, 0)
        {
            parameters.Add(new StringParameter(name));
        }
        
        public override Task executeFunction()
        {
            LuaCodebase.DestroyObject((string)parameters[0].Get());
            return Task.CompletedTask;
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject obj = GameObject.Find(\"{parameters[0]}\");\n" +
                $"Destroy(obj);\n";
            return codeText;
        }
    }
}
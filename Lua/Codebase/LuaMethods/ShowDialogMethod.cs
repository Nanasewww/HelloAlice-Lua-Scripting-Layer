using System.Threading.Tasks;
using Lua.CodeBlock;
using UnityEngine;

namespace Lua.Codebase
{
    public class ShowDialogMethod : LuaMethod
    {
        public ShowDialogMethod(string character, string content) : base("ShowDialog", 2, 0)
        {
            parameters.Add(new StringParameter(character));
            parameters.Add(new StringParameter(content));
        }
        
        public override Task executeFunction()
        {
            LuaCodebase.ShowDialog((string)parameters[0].Get(), (string)parameters[1].Get());
            return Task.CompletedTask;
        }

        public override string getCodeText()
        {
            string codeText =
                $"GameObject dialog = Instantiate(dialogPrefab);\n" +
                $"dialog.SetCharacter(\"{parameters[0]}\");\n" +
                $"dialog.SetContent(\"{parameters[1]}\");\n";
            return codeText;
        }

        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();
            for (int i = 0; i < inputFieldNum; ++i)
            {
                int index = i;
                cb.inputFields[i].onEndEdit.AddListener((str) =>
                {
                    parameters[index].Set(str);
                    LuaCodebase.UpdateShowDialog((string)parameters[0].Get(), (string)parameters[1].Get());
                });
            }
            return codeBlock;
        }
    }
}
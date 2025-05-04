using Lua.CodeBlock;
using UnityEngine;
using System.Threading.Tasks;

namespace Lua.Codebase
{
    public class PlayAnimationMethod : LuaMethod
    {
        public PlayAnimationMethod(string name) : base("PlayAnimation", 0, 1)
        {
            dropdownParameters.Add(new StringParameter(name));
        }

        public override Task executeFunction()
        {
            LuaCodebase.PlayAnimation((string)dropdownParameters[0].Get());
            return Task.CompletedTask;
        }
        
        public override string getCodeText()
        {
            string codeText =
                $"Animator animator = character.GetComponent<Animator>();\n" +
                $"animator.Play(\"{dropdownParameters[0]}\");\n";
            return codeText;
        }
        
        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();
            cb.dropdownFields[0].onValueChanged.AddListener((index) =>
            {
                dropdownParameters[0].Set(cb.dropdownFields[0].GetComponent<CBResourcesDropdown>().getDropdownValue(index));
                LuaCodebase.PlayAnimation((string)dropdownParameters[0].Get());
            });
            
            return codeBlock;
        }
    }
}
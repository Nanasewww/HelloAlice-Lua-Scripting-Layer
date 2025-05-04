using Lua.CodeBlock;
using System.Threading.Tasks;
using UnityEngine;

namespace Lua.Codebase
{
    public class PlaySoundMethod: LuaMethod
    {
        public PlaySoundMethod(string clipName) : base("PlaySound", 0, 1)
        {
            dropdownParameters.Add(new StringParameter(clipName));
        }
        
        public override Task executeFunction()
        {
            LuaCodebase.PlaySound((string)dropdownParameters[0].Get());
            return Task.CompletedTask;
        }

        public override string getCodeText()
        {
            string codeText =
                $"AudioClip newClip = Resources.Load<AudioClip>(\"{dropdownParameters[0]}\");\n" +
                $"audioSource.clip = newClip;\n" +
                $"audioSource.Play();\n";
            return codeText;
        }

        public override GameObject drawCodeBlock(GameObject canvas)
        {
            GameObject codeBlock = base.drawCodeBlock(canvas);
            CBPrefab cb = codeBlock.GetComponent<CBPrefab>();
            cb.dropdownFields[0].onValueChanged.AddListener((index) =>
            {
                dropdownParameters[0].Set(cb.dropdownFields[0].GetComponent<CBResourcesDropdown>().getDropdownValue(index));
                executeFunction();
            });
            
            return codeBlock;
        }
    }
}
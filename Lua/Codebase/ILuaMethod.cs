using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Lua
{
    // Interface to define the structure for all Lua methods used in the system
    public interface ILuaMethod
    {
        /// <summary>
        /// Executes the functionality of the method asynchronously
        /// </summary>
        /// <returns>A Task representing the async execution process.</returns>
        public Task executeFunction();
        
        /// <summary>
        /// Draws the method as a code block in the Unity canvas.
        /// </summary>
        /// <param name="canvas">The canvas GameObject to draw the code block on.</param>
        /// <returns>The instantiated code block GameObject.</returns>
        public GameObject drawCodeBlock(GameObject canvas);
        
        /// <summary>
        /// Returns a human-readable string describing this method.
        /// </summary>
        /// <returns>Code text string.</returns>
        public string getCodeText();
        
        /// <summary>
        /// Converts this method into a line of Lua code.
        /// </summary>
        /// <returns>String of Lua code.</returns>
        public string getLuaCode();
        
        /// <summary>
        /// Serializes this method into a JSON object, including all parameters.
        /// </summary>
        /// <returns>JSON representation of the method.</returns>
        public JObject toJSON();
    }
}
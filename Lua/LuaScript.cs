using System.Collections.Generic;
using Lua.Codebase;
using Lua.Codebase.Template;
using Newtonsoft.Json.Linq;

namespace Lua
{
    /// <summary>
    /// Represents a Lua script containing metadata and a list of Lua methods
    /// </summary>
    public class LuaScript
    {
        private string _luaCode;
        private string _luaName;
        private int _luaId;
        private List<ILuaMethod> _luaMethods;

        /// <summary>
        /// The name of the Lua script
        /// </summary>
        public string luaName
        {
            get => _luaName;
            set => _luaName = value;
        }

        /// <summary>
        /// The generated Lua code from all Lua methods
        /// </summary>
        public string luaCode
        {
            get
            {
                if (_luaMethods.Count > 0)
                {
                    _luaCode = "";
                    foreach (var m in _luaMethods) _luaCode += m.getLuaCode();
                }
                return _luaCode;
            }
            set => _luaCode = value;
        }

        /// <summary>
        /// The unique identifier of the Lua script
        /// </summary>
        public int luaId
        {
            get => _luaId;
            set => _luaId = value;
        }

        /// <summary>
        /// The list of ILuaMethod components
        /// </summary>
        public List<ILuaMethod> luaMethods
        {
            get => _luaMethods;
            set => _luaMethods = value;
        }
        
        /// <summary>
        /// Initializes a new instance of the LuaScript class
        /// </summary>
        /// <param name="name">The name of the Lua script</param>
        /// <param name="code">The initial Lua code (optional)</param>
        /// <param name="id">An optional identifier, defaults to -1</param>
        /// <param name="methods">An optional list of ILuaMethod objects</param>
        public LuaScript(string name, string code, int id = -1, List<ILuaMethod> methods = null)
        {
            _luaName = name;
            _luaCode = code;
            _luaId = id;
            if (methods != null) this._luaMethods = methods;
            else this._luaMethods = new List<ILuaMethod>();
        }

        /// <summary>
        /// Converts the LuaScript data into a JSON object.
        /// </summary>
        /// <returns>A JObject containing the script's name, code, and ID.</returns>
        public JObject toJSON()
        {
            return new JObject
            {
                ["name"] = _luaName,
                ["code"] = _luaCode,
                ["id"] = _luaId
            };
        }
    }
}
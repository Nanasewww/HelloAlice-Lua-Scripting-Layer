namespace Lua.Codebase
{
    public class StringParameter : IParameter<string>
    {
        private string _value;
    
        public StringParameter(string initialValue)
        {
            _value = initialValue;
        }
        protected override string getValue() => _value;
        protected override void setValue(string value) => _value = value;
        public override string ToString() => _value;

        public override string ToLuaString()
        {
            string str = _value.Replace("'", "\\'");
            return $"'{str}'";
        }
    }
}
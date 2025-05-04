namespace Lua.Codebase
{
    public class BoolParameter : IParameter<bool>
    {
        private bool _value;

        public BoolParameter(bool initialValue)
        {
            _value = initialValue;
        }
        protected override bool getValue() => _value;
        protected override void setValue(bool value) => _value = value;

        public override string ToString()
        {
            return _value ? "true" : "false";
        }
    }
}
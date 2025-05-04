namespace Lua.Codebase
{
    public class IntParameter : IParameter<int>
    {
        private int _value;
    
        public IntParameter(int initialValue)
        {
            _value = initialValue;
        }
        protected override int getValue() => _value;
        protected override void setValue(int value) => _value = value;
        public override string ToString() => _value.ToString();
    }
}
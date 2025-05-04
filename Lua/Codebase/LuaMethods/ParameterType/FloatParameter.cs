using System;

namespace Lua.Codebase
{
    public class FloatParameter : IParameter<float>
    {
        private float _value;
        private const int DECIMAL = 1;
    
        public FloatParameter(float initialValue)
        {
            _value = (float)Math.Round(initialValue, DECIMAL);
        }
        protected override float getValue() => _value;
        protected override void setValue(float value) => _value = (float)Math.Round(value, DECIMAL);
        public override string ToString() => _value.ToString("F" + DECIMAL);
    }
}
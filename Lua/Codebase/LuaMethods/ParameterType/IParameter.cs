using System;

namespace Lua.Codebase
{
    public interface IParameter
    {
        object Get();
        void Set(object value);
        string ToLuaString();
    }

    public abstract class IParameter<T> : IParameter
    {
        protected abstract T getValue();
        protected abstract void setValue(T value);
    
        public object Get() => getValue();
        public void Set(object value) => setValue((T)value);
        public virtual string ToLuaString() => ToString();
    }
}
using System;

namespace Qoden.Util
{
    public static class Default
    {
        public static DefaultValue<T> Value<T>(Func<T> valueFactory)
        {
            return new DefaultValue<T>(valueFactory);
        }
    }

    public struct DefaultValue<T>
    {
        private T _value;
        private Func<T> _valueFactory;

        public DefaultValue(Func<T> valueFactory)
        {
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _value = default(T);
        }

        public T Value
        {
            get
            {
                if (_valueFactory != null)
                {
                    _value = _valueFactory();
                    _valueFactory = null;
                }
                return _value;
            }
            set
            {
                _value = value;
                _valueFactory = null;
            }
        }
    }
}

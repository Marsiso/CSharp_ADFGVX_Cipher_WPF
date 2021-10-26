using System;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        public sealed class Ref<T>
        {
            private Func<T> getter;
            private Action<T> setter;
            public Ref(Func<T> getter, Action<T> setter)
            {
                this.getter = getter;
                this.setter = setter;
            }
            public T Value
            {
                get => getter();
                set => setter(value);
            }
        }
    }
}

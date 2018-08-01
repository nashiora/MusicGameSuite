using System;

namespace OpenRM
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class SerializeFieldAttribute : Attribute
    {
        public SerializeFieldAttribute()
        {
        }
    }
}

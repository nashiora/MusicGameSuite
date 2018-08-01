using System;
using System.Collections.Generic;

namespace OpenRM.Objects
{
    public class DynObject : Object
    {
        private readonly Dictionary<string, Variant> m_properties = new Dictionary<string, Variant>();

        public Variant this[string key]
        {
            get
            {
                if (!m_properties.TryGetValue(key, out var result))
                    result = default;
                return result;
            }
            set => m_properties[key] = value;
        }
    }
}

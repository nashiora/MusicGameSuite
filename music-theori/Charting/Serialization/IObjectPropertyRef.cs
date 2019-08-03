using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace theori.Charting.Serialization
{
    internal interface IObjectPropertyRef
    {
        string Name { get; }
        Type Type { get; }
        object Value { get; set; }

        bool HasAttribute<TAttrib>() where TAttrib : Attribute;
        TAttrib GetAttribute<TAttrib>() where TAttrib : Attribute;
    }

    internal static class ObjectPropertyInfoExt
    {
        public static IEnumerable<IObjectPropertyRef> GetTheoriPropertyInfos(this object obj)
        {
            if (obj is null) yield break;

            var objType = obj.GetType();
            var fields = from type in objType.GetFields()
                         where type.GetCustomAttribute<TheoriIgnoreAttribute>() == null &&
                              (type.GetCustomAttribute<TheoriPropertyAttribute>() != null || type.IsPublic)
                         select type;
            var props = from prop in objType.GetProperties()
                        where prop.SetMethod != null && prop.GetMethod != null
                        where prop.GetCustomAttribute<TheoriIgnoreAttribute>() == null && (prop.GetCustomAttribute<TheoriPropertyAttribute>() != null ||
                             (prop.SetMethod.IsPublic && prop.SetMethod.GetCustomAttribute<TheoriIgnoreAttribute>() == null &&
                              prop.GetMethod.IsPublic && prop.GetMethod.GetCustomAttribute<TheoriIgnoreAttribute>() == null))
                        select prop;

            foreach (var field in fields)
                yield return new ObjectFieldRef(field, obj);

            foreach (var prop in props)
                yield return new ObjectPropertyRef(prop, obj);

            yield break;
        }
    }

    internal class ObjectFieldRef : IObjectPropertyRef
    {
        private readonly FieldInfo m_info;
        private readonly object m_obj;

        public string Name => m_info.Name;
        public Type Type => m_info.FieldType;
        public object Value { get => m_info.GetValue(m_obj); set => m_info.SetValue(m_obj, value); }

        public ObjectFieldRef(FieldInfo info, object obj)
        {
            m_info = info;
            m_obj = obj;
        }

        public TAttrib GetAttribute<TAttrib>() where TAttrib : Attribute
        {
            return m_info.GetCustomAttribute<TAttrib>();
        }

        public bool HasAttribute<TAttrib>() where TAttrib : Attribute
        {
            return GetAttribute<TAttrib>() != null;
        }
    }

    internal class ObjectPropertyRef : IObjectPropertyRef
    {
        private readonly PropertyInfo m_info;
        private readonly object m_obj;

        public string Name => m_info.Name;
        public Type Type => m_info.PropertyType;
        public object Value { get => m_info.GetValue(m_obj); set => m_info.SetValue(m_obj, value); }

        public ObjectPropertyRef(PropertyInfo info, object obj)
        {
            m_info = info;
            m_obj = obj;
        }

        public TAttrib GetAttribute<TAttrib>() where TAttrib : Attribute
        {
            return m_info.GetCustomAttribute<TAttrib>();
        }

        public bool HasAttribute<TAttrib>() where TAttrib : Attribute
        {
            return GetAttribute<TAttrib>() != null;
        }
    }
}

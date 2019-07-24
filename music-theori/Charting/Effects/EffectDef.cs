using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using theori.Audio;
using theori.Audio.Effects;
using theori.GameModes;

namespace theori.Charting.Effects
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EffectTypeAttribute : Attribute
    {
        public readonly string Name;
        /// <summary>
        /// Used if we support custom serialization for effects.
        /// Needs to be removed otherwise.
        /// </summary>
        public readonly Type SerializerType;

        public EffectTypeAttribute(string name, Type serializerType = null)
        {
            Name = name;
            SerializerType = serializerType;
        }
    }

    public abstract class EffectDef : IEquatable<EffectDef>
    {
        #region Type Registry

        private static readonly Dictionary<string, Type> effectTypesById = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, string> effectIdsByType = new Dictionary<Type, string>();

        public static Type GetEntityTypeById(string id)
        {
            if (effectTypesById.TryGetValue(id, out var type))
                return type;
            return null;
        }

        public static string GetEffectId<T>() where T : EffectDef => GetEffectIdByType(typeof(T));
        public static string GetEffectIdByType(Type type)
        {
            if (effectIdsByType.TryGetValue(type, out string id))
                return id;
            return null;
        }

        internal static void RegisterTheoriTypes()
        {
            var types = from type in typeof(EffectDef).Assembly.ExportedTypes
                        where type == typeof(EffectDef) || type.IsSubclassOf(typeof(EffectDef))
                        select type;
            RegisterTypes("theori", types);
        }

        public static void RegisterTypesFromGameMode(GameMode gameMode)
        {
            var types = from type in gameMode.GetType().Assembly.ExportedTypes
                        where type.IsSubclassOf(typeof(EffectDef))
                        select type;
            RegisterTypes(gameMode.Name, types);
        }

        private static void RegisterTypes(string modeName, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                string typeName;

                var typeAttrib = type.GetCustomAttribute<EffectTypeAttribute>();
                if (typeAttrib != null)
                    typeName = typeAttrib.Name;
                else typeName = type.Name;

                string id = $"{ modeName }.{ typeName }";
                effectTypesById[id] = type;
                effectIdsByType[type] = id;
            }
        }

        #endregion

        static EffectDef()
        {
            RegisterTheoriTypes();
        }

        [TheoriProperty("mix")]
        public EffectParamF Mix;

        protected EffectDef() { }
        protected EffectDef(EffectParamF mix)
        {
            Mix = mix;
        }

        public abstract Dsp CreateEffectDsp(int sampleRate);
        public virtual void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            effect.Mix = Mix.Sample(alpha);
        }

        public abstract bool Equals(EffectDef other);

        public override bool Equals(object obj)
        {
            if (obj is EffectDef def) return Equals(def);
            return false;
        }

        public override abstract int GetHashCode();
    }
}

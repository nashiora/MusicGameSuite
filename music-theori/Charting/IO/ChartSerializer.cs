using System.Collections.Generic;
using System.IO;

using theori.Audio.Effects;

namespace theori.Charting.IO
{
    public sealed class ChartEffectTable
    {
        private readonly List<EffectDef> m_effects = new List<EffectDef>();

        public EffectDef this[int index] => m_effects[index];
        public int Count => m_effects.Count;

        internal ChartEffectTable()
        {
        }

        public int IndexOf(EffectDef effect) => m_effects.IndexOf(effect);

        public int Add(EffectDef effect)
        {
            int index = m_effects.IndexOf(effect);
            if (index < 0)
            {
                index = m_effects.Count;
                m_effects.Add(effect);
            }
            return index;
        }
    }

    public abstract class ChartObjectSerializer
    {
        /// <summary>
        /// A locally unique value > 0
        /// </summary>
        public readonly int ID;

        protected ChartObjectSerializer(int id)
        {
            ID = id;
        }

        public abstract void SerializeSubclass(ChartObject obj, BinaryWriter writer, ChartEffectTable effects);

        public abstract ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects);
    }

    public abstract class ChartObjectSerializer<T> : ChartObjectSerializer
        where T : ChartObject
    {
        protected ChartObjectSerializer(int id) : base(id) { }

        public sealed override void SerializeSubclass(ChartObject obj, BinaryWriter writer, ChartEffectTable effects) => SerializeSubclass(obj as T, writer, effects);
        public abstract void SerializeSubclass(T obj, BinaryWriter writer, ChartEffectTable effects);
    }
}

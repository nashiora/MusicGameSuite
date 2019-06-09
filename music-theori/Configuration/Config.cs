using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace theori.Configuration
{
    public enum ConfigFormat
    {
        Ini,
    }

    public static class ConfigFormatExt
    {
        public static string GetFileExtension(this ConfigFormat format)
        {
            switch (format)
            {
                case ConfigFormat.Ini: return ".ini";
                default: Debug.Assert(false); return null;
            }
        }
    }

    public abstract class Config<TKey>
    {
        private readonly Dictionary<string, TKey> namedKeys = new Dictionary<string, TKey>();
        private readonly Dictionary<TKey, ConfigEntry> entries = new Dictionary<TKey, ConfigEntry>();

        public bool Dirty { get; protected set; } = false;

        public Config()
        {
            SetDefaults();
        }

        public void Clear()
        {
            namedKeys.Clear();
            entries.Clear();

            SetDefaults();
        }

        protected abstract void SetDefaults();

        public virtual void Load(TextReader reader)
        {
            Clear();

            var setKeys = new HashSet<TKey>();
            foreach (var key in entries.Keys)
                setKeys.Add(key);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Split('=', out string keyName, out string value))
                {
                    keyName.Trim();
                    value.Trim();

                    var key = namedKeys[keyName];
                    if (entries.TryGetValue(key, out var entry))
                    {
                        setKeys.Remove(key);
                        entry.FromString(value);
                    }
                }
            }

            if (setKeys.Count > 0)
                Dirty = true;
            else Dirty = false;
        }

        public void Save(TextWriter writer)
        {
            foreach (var entry in entries)
            {
                string key = entry.Key.ToString();
                writer.WriteLine($"{ key }={ entry.Value }");
            }
            Dirty = false;
        }

        private TEntry SetEnsure<TEntry>(TKey key)
            where TEntry : ConfigEntry, new()
        {
            if (entries.TryGetValue(key, out var entry))
            {
                if (entry is TEntry result)
                    return result;
                throw new ArgumentException("Entry of invalid type");
            }

            // save the name of the key as a string, used for loading
            namedKeys[key.ToString()] = key;

            var r = new TEntry();
            entries[key] = r;
            return r;
        }

        private TEntry GetEnsure<TEntry>(TKey key)
            where TEntry : ConfigEntry
        {
            var entry = entries[key];
            if (entry is TEntry result)
                return result;
            throw new ArgumentException("Entry of invalid type");
        }

        public string GetAsStringImage(TKey key) => GetEnsure<ConfigEntry>(key).ToString();

        public int GetInt(TKey key) => GetEnsure<IntConfig>(key).Value;
        public float GetFloat(TKey key) => GetEnsure<FloatConfig>(key).Value;
        public bool GetBool(TKey key) => GetEnsure<BoolConfig>(key).Value;
        public string GetString(TKey key) => GetEnsure<StringConfig>(key).Value;
        public TEnum GetEnum<TEnum>(TKey key) where TEnum : struct => 
           GetEnsure<EnumConfig<TEnum>>(key).Value;
        
        private void CheckSet<T>(ref T target, T value)
        {
            if (!Equals(target, value))
            {
                Dirty = true;
                target = value;
            }
        }
        
        public void Set(TKey key, int value) => CheckSet(ref SetEnsure<IntConfig>(key).Value, value);
        public void Set(TKey key, float value) => CheckSet(ref SetEnsure<FloatConfig>(key).Value, value);
        public void Set(TKey key, bool value) => CheckSet(ref SetEnsure<BoolConfig>(key).Value, value);
        public void Set(TKey key, string value) => CheckSet(ref SetEnsure<StringConfig>(key).Value, value);
        public void Set<TEnum>(TKey key, TEnum value) where TEnum : struct =>
            CheckSet(ref SetEnsure<EnumConfig<TEnum>>(key).Value, value);
    }
}

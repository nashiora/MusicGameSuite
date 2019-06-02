using System;

// TODO(local): properly error handle config things like parsing

namespace theori.Configuration
{
    public abstract class ConfigEntry
    {
        public abstract override string ToString();
        public abstract void FromString(string value);
    }

    public class IntConfig : ConfigEntry
    {
        public int Value;

        public override string ToString() => Value.ToString();
        public override void FromString(string value) => int.TryParse(value, out Value);
    }

    public class FloatConfig : ConfigEntry
    {
        public float Value;

        public override string ToString() => Value.ToString();
        public override void FromString(string value) => float.TryParse(value, out Value);
    }

    public class BoolConfig : ConfigEntry
    {
        public bool Value;

        public override string ToString() => Value.ToString();
        public override void FromString(string value) => bool.TryParse(value, out Value);
    }

    public class StringConfig : ConfigEntry
    {
        public string Value;

        public override string ToString() => $"\"{ Value }\"";
        public override void FromString(string value) => Value = value.Substring(1, value.Length - 2);
    }

    public class EnumConfig<T> : ConfigEntry
        where T : struct
    {
        public T Value;

        public override string ToString() => Value.ToString();
        public override void FromString(string value) => Enum.TryParse<T>(value, out Value);
    }
}

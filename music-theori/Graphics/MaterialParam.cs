using System.Numerics;

using OpenGL;

namespace theori.Graphics
{
    // TODO(local): Type-safe value setting.
    public class MaterialParam
    {
        public GLType Type;

        private object v;
        public object Value { get => v; set => SetValue(value); }

        private MaterialParam(GLType type, object value)
        {
            Type = type;
            v = value;
        }

        public MaterialParam(GLType type)
        {
            SetDefault(type);
        }

        public MaterialParam(object value)
        {
            SetValue(value);
        }

        public MaterialParam Copy() => new MaterialParam(Type, v);

        // TODO(local): provide specific errors when the type is incorrect
        internal T Get<T>() => (T)v;

        private void SetDefault(GLType type)
        {
            Type = type;
            switch (Type)
            {
                case GLType.Int:       Value = 0;                   break;
                case GLType.Float:     Value = 0.0f;                break;
                case GLType.FloatMat4: Value = Transform.Identity;  break;
                case GLType.FloatVec2: Value = Vector2.Zero;        break;
                case GLType.FloatVec3: Value = Vector3.Zero;        break;
                case GLType.FloatVec4: Value = Vector4.Zero;        break;
                case GLType.Sampler2D: Value = Texture.Empty;       break;
            }
        }

        private void SetValue(object value)
        {
            v = value;
            switch (value)
            {
                case sbyte _: case  short _: case  int _:
                case  byte _: case ushort _: case uint _:
                    v = (int)value;
                    Type = GLType.Int;
                    break;

                case  long _: case ulong _:
                case float _: case double _:
                    v = (float)value;
                    Type = GLType.Float;
                    break;
                    
                case Matrix4x4 m:
                    v = (Transform)m;
                    Type = GLType.FloatMat4;
                    break;

                case Transform _: Type = GLType.FloatMat4; break;
                case Vector2   _: Type = GLType.FloatVec2; break;
                case Vector3   _: Type = GLType.FloatVec3; break;
                case Vector4   _: Type = GLType.FloatVec4; break;
                case Texture   _: Type = GLType.Sampler2D; break;

                default: throw new System.ArgumentException($"Invalid parameter type: { value?.GetType().Name ?? "null" }");
            }
        }
    }
}

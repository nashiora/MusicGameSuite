using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace theori.Graphics
{
    public enum BlendMode
    {
        Normal,
        Additive,
        Multiply,
    }

    public enum BuiltInParams : uint
    {
        World,
        Projection,
        Camera,
        BillboardMatrix,
        Viewport,
        AspectRatio,
        Time,
        __BuiltInCount,
        User = 0x100,
    }

    public class MaterialParams
    {
        public readonly Dictionary<string, MaterialParam> Params = new Dictionary<string, MaterialParam>();

        public object this[string name]
        {
            get
            {
                if (Params.TryGetValue(name, out var param))
                    return param.Value;
                return null;
            }

            set
            {
                if (Params.TryGetValue(name, out var param))
                    param.Value = value;
                else Params[name] = new MaterialParam(value);
            }
        }

        public MaterialParams Copy()
        {
            var result = new MaterialParams();
            foreach (var entry in Params)
                result.Params[entry.Key] = entry.Value.Copy();
            return result;
        }
    }

    public class Material : Disposable
    {
        private static readonly Dictionary<Type, object> BindFuncs = new Dictionary<Type, object>();
        private static Action<ShaderProgram, int, T> GetBindFunc<T>()
        {
            return (Action<ShaderProgram, int, T>)BindFuncs[typeof(T)];
        }

        static Material()
        {
            void AddBindFunc<T>(Action<ShaderProgram, int, T> bind)
            {
                BindFuncs[typeof(T)] = bind;
            }
            
            AddBindFunc<int>(BindShaderVar_Int);
            AddBindFunc<float>(BindShaderVar_Float);
            AddBindFunc<Transform>(BindShaderVar_Transform);
            AddBindFunc<Vector2>(BindShaderVar_Vector2);
            AddBindFunc<Vector3>(BindShaderVar_Vector3);
            AddBindFunc<Vector4>(BindShaderVar_Vector4);
        }

        private static void BindShaderVar_Int      (ShaderProgram program, int location, int       value) => program.SetUniform(location, value);
        private static void BindShaderVar_Float    (ShaderProgram program, int location, float     value) => program.SetUniform(location, value);
        private static void BindShaderVar_Transform(ShaderProgram program, int location, Transform value) => program.SetUniform(location, (Matrix4x4)value);
        private static void BindShaderVar_Vector2  (ShaderProgram program, int location, Vector2   value) => program.SetUniform(location, value);
        private static void BindShaderVar_Vector3  (ShaderProgram program, int location, Vector3   value) => program.SetUniform(location, value);
        private static void BindShaderVar_Vector4  (ShaderProgram program, int location, Vector4   value) => program.SetUniform(location, value);

        struct BoundParamInfo
        {
            public ShaderStage Stage;
            public int Location;
            public GLType Type;
        }

        private readonly ProgramPipeline pipeline;

        public BlendMode BlendMode;
        public bool Opaque;

        private readonly ShaderProgram[] shaders = new ShaderProgram[3];

        private uint userId = (uint)BuiltInParams.User;

        private readonly Dictionary<string, uint> mappedParams = new Dictionary<string, uint>();
        private readonly Dictionary<uint, List<BoundParamInfo>> boundParams = new Dictionary<uint, List<BoundParamInfo>>();
        private readonly Dictionary<string, uint> textureIDs = new Dictionary<string, uint>();

        private uint textureID = 0;

        public Material(string vertexShaderPath, string fragmentShaderPath, string geometryShaderPath = null)
        {
            pipeline = new ProgramPipeline();

            void AddShader(string path, ShaderType type)
            {
                string source = File.ReadAllText(path);

                var program = new ShaderProgram(type, source);
                if (!program || !program.Linked)
                {
                    Logger.Log(program.InfoLog);
                    Host.Quit(1);
                }

                AssignShader(program);
            }
            
            AddShader(vertexShaderPath, ShaderType.Vertex);
            AddShader(fragmentShaderPath, ShaderType.Fragment);
            if (geometryShaderPath != null)
                AddShader(geometryShaderPath, ShaderType.Geometry);
        }

        public Material(Stream vertexShaderStream, Stream fragmentShaderStream, Stream geometryShaderStream = null)
        {
            pipeline = new ProgramPipeline();

            void AddShader(Stream stream, ShaderType type)
            {
                string source = new StreamReader(stream).ReadToEnd();

                var program = new ShaderProgram(type, source);
                if (!program || !program.Linked)
                {
                    Logger.Log(program.InfoLog);
                    Host.Quit(1);
                }

                AssignShader(program);
            }

            AddShader(vertexShaderStream, ShaderType.Vertex);
            AddShader(fragmentShaderStream, ShaderType.Fragment);
            if (geometryShaderStream != null)
                AddShader(geometryShaderStream, ShaderType.Geometry);
        }

        private int GetShaderIndex(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex: return 0;
                case ShaderStage.Geometry: return 1;
                case ShaderStage.Fragment: return 2;
            }
            return -1;
        }

        public void AssignShader(ShaderProgram program)
        {
            shaders[GetShaderIndex(program.Stage)] = program;

            var uniforms = program.ActiveUniforms;
            for (int count = uniforms.Length, i = 0; i < count; i++)
            {
                var info = uniforms[i];

                if (info.Type == GLType.Sampler2D)
                {
					if(!textureIDs.ContainsKey(info.Name))
						textureIDs[info.Name] = textureID++;
                }

                uint target = 0;
                if (Enum.TryParse<BuiltInParams>(info.Name, out var builtInKind)
                    && builtInKind < BuiltInParams.__BuiltInCount)
                {
                    target = (uint)builtInKind;
                }
                else if (!mappedParams.TryGetValue(info.Name, out target))
                    mappedParams[info.Name] = target = userId++;

                var paramInfo = new BoundParamInfo()
                {
                    Stage = program.Stage,
                    Location = info.Location,
                    Type = info.Type,
                };

                if (!boundParams.TryGetValue(target, out var list))
                    boundParams[target] = list = new List<BoundParamInfo>();
                list.Add(paramInfo);

#if DEBUG
                Logger.Log($"Uniform [{ i }, loc={ info.Location }, { info.Type }] = { info.Name }");
#endif
            }

            program.Use(pipeline);
        }

        public void Bind(RenderState state, MaterialParams p)
        {
            BindAll(BuiltInParams.Projection, state.ProjectionMatrix);
            BindAll(BuiltInParams.Camera, state.CameraMatrix);

            ApplyParams(p, state.WorldTransform);
            BindToContext();
        }

        public void BindToContext()
        {
            pipeline.Bind();
        }

        public void ApplyParams(MaterialParams p, Transform world)
        {
            BindAll(BuiltInParams.World, world);
            foreach (var x in p.Params)
            {
                string name = x.Key;
                var param = x.Value;

                switch (param.Type)
                {
                    case GLType.Int: BindAll(name, param.Get<int>()); break;
                    case GLType.Float: BindAll(name, param.Get<float>()); break;
                    case GLType.FloatMat4: BindAll(name, param.Get<Matrix4x4>()); break;
                    case GLType.FloatVec2: BindAll(name, param.Get<Vector2>()); break;
                    case GLType.FloatVec3: BindAll(name, param.Get<Vector3>()); break;
                    case GLType.FloatVec4: BindAll(name, param.Get<Vector4>()); break;

                    case GLType.Sampler2D:
                        if (!textureIDs.TryGetValue(name, out uint unit))
                        {
                            // TODO(local): error!!!
                            break;
                        }

                        param.Get<Texture>().Bind(unit);
                        BindAll(name, (int)unit);
                        break;
                }
            }
        }

        private List<BoundParamInfo> GetBoundParams(string name, out int count)
        {
            count = 0;
            if (!mappedParams.TryGetValue(name, out uint mId))
                return null;
            return GetBoundParams((BuiltInParams)mId, out count);
        }

        private List<BoundParamInfo> GetBoundParams(BuiltInParams p, out int count)
        {
            count = 0;
            if (!boundParams.TryGetValue((uint)p, out var result))
                return result;
            count = result.Count;
            return result;
        }

        private void BindAll<T>(string name, T value)
        {
            var p = GetBoundParams(name, out int count);
            BindAll(p, count, value);
        }

        private void BindAll<T>(BuiltInParams builtIn, T value)
        {
            var p = GetBoundParams(builtIn, out int count);
            BindAll(p, count, value);
        }

        private void BindAll<T>(List<BoundParamInfo> p, int count, T value)
        {
            var bind = GetBindFunc<T>();
            for (int i = 0; i < count; i++)
            {
                var param = p[i];
                
                var program = shaders[GetShaderIndex(param.Stage)];
                int location = param.Location;
                
                bind(program, location, value);
            }
        }

        protected override void DisposeManaged()
        {
            for (int i = 0; i < 3; i++)
                shaders[i]?.Dispose();
            pipeline.Dispose();
        }
    }
}

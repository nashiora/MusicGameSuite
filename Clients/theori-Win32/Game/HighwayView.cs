using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using theori.Graphics;
using theori.Input;
using OpenRM;
using OpenRM.Voltex;
using System.Diagnostics;

namespace theori.Game
{
    public class HighwayView
    {
        private const float PITCH_AMT = 10;
        private const float ZOOM_POW = 1.75f;
        private const float LENGTH_BASE = 12;

        private float roll, rollBase;
        private float pitch, zoom; // "top", "bottom"
        private float critScreenY = 0.05f;

        public readonly BasicCamera Camera;
        public Transform WorldTransform { get; private set; }
        
        private Texture highwayTexture;
        private Texture btChipTexture, btHoldTexture;
        private Texture fxChipTexture, fxHoldTexture;
        private Texture laserTexture, laserEntryTexture, laserExitTexture;

        private Material basicMaterial, highwayMaterial;
        private Material btChipMaterial, btHoldMaterial;
        private Material fxChipMaterial, fxHoldMaterial;
        private Material laserMaterial, laserEntryMaterial, laserExitMaterial;
        
        private Mesh highwayMesh;
        private Mesh btChipMesh, btHoldMesh;
        private Mesh fxChipMesh, fxHoldMesh;
        private Mesh laserEntryMesh, laserExitMesh;
        
        private MaterialParams btChipParams, btHoldParams;
        private MaterialParams fxChipParams, fxHoldParams;
        private MaterialParams lLaserParams, rLaserParams;
        private MaterialParams lLaserEntryParams, rLaserEntryParams;
        private MaterialParams lLaserExitParams, rLaserExitParams;

        internal Dictionary<OpenRM.Object, ObjectRenderable3D>[] renderables = new Dictionary<OpenRM.Object, ObjectRenderable3D>[8];

        public time_t PlaybackPosition { get; set; }

        private time_t m_vd;
        public time_t ViewDuration
        {
            get => m_vd;
            set
            {
                m_vd = value;
            }
        }

        public float LaserRoll => roll;
        public float CriticalHeight => (1 - critScreenY) * Camera.ViewportHeight;

        public float HorizonHeight { get; private set; }
        
        public float LaserRollSpeed { get; set; } = 1;

        public float TargetLaserRoll { get; set; }
        public float TargetBaseRoll { get; set; }
        
        public float TargetPitch { get; set; }
        public float TargetZoom { get; set; }
        public float TargetOffset { get; set; }

        public Vector3 CameraOffset { get; set; }
        
        const float SLAM_DUR_TICKS = 1 / 32.0f;
        time_t SlamDurationTime(OpenRM.Object obj) => obj.Chart.ControlPoints.MostRecent(obj.Position).MeasureDuration * SLAM_DUR_TICKS;

        public HighwayView()
        {
            highwayTexture = new Texture();
            highwayTexture.Load2DFromFile(@".\skins\Default\textures\highway.png");

            basicMaterial = new Material("basic");

            highwayMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1, LENGTH_BASE + 1, Anchor.BottomCenter);
            highwayMaterial = new Material("highway");

            void CreateTextureAndMesh(string texName, int width, bool useAspect, ref Texture texture, ref Mesh mesh, ref MaterialParams p)
            {
                texture = new Texture();
                texture.Load2DFromFile($@".\skins\Default\textures\{ texName }.png");

                float aspect = btChipTexture.Height / (float)btChipTexture.Width;
                float height = useAspect ? width * aspect / 6 : 1;

                mesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, width / 6.0f, height, Anchor.BottomCenter);

                p = new MaterialParams();
                p["Color"] = new Vector4(1);
                p["MainTexture"] = texture;
            }
            
            CreateTextureAndMesh("bt_chip", 1, true , ref btChipTexture, ref btChipMesh, ref btChipParams);
            CreateTextureAndMesh("bt_hold", 1, false, ref btHoldTexture, ref btHoldMesh, ref btHoldParams);
            CreateTextureAndMesh("fx_chip", 2, true , ref fxChipTexture, ref fxChipMesh, ref fxChipParams);
            CreateTextureAndMesh("fx_hold", 2, false, ref fxHoldTexture, ref fxHoldMesh, ref fxHoldParams);
            
            laserTexture = new Texture();
            laserTexture.Load2DFromFile(@".\skins\Default\textures\laser.png");
            
            laserEntryTexture = new Texture();
            laserEntryTexture.Load2DFromFile(@".\skins\Default\textures\laser_entry.png");
            
            laserExitTexture = new Texture();
            laserExitTexture.Load2DFromFile(@".\skins\Default\textures\laser_exit.png");
            
            laserEntryMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1 / 6.0f, (laserEntryTexture.Height / (float)laserEntryTexture.Width) / 6.0f, Anchor.TopCenter);
            laserExitMesh  = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1 / 6.0f, (laserExitTexture.Height  / (float)laserExitTexture.Width ) / 6.0f, Anchor.BottomCenter);

            void CreateLaserMaterialsParams(Vector3 color, out MaterialParams laser, out MaterialParams laserEntry, out MaterialParams laserExit)
            {
                laser = new MaterialParams();
                laser["LaserColor"] = color;
                laser["HiliteColor"] = new Vector3(1, 1, 0);
                laser["MainTexture"] = laserTexture;
                
                laserEntry = new MaterialParams();
                laserEntry["Color"] = color;
                laser["HiliteColor"] = new Vector3(1, 1, 0);
                laserEntry["MainTexture"] = laserEntryTexture;
                
                laserExit = new MaterialParams();
                laserExit["Color"] = color;
                laser["HiliteColor"] = new Vector3(1, 1, 0);
                laserExit["MainTexture"] = laserExitTexture;
            }
            
            CreateLaserMaterialsParams(new Vector3(0, 0.5f, 1), out lLaserParams, out lLaserEntryParams, out lLaserExitParams);
            CreateLaserMaterialsParams(new Vector3(1, 0, 0.5f), out rLaserParams, out rLaserEntryParams, out rLaserExitParams);

            btChipMaterial = basicMaterial;
            btHoldMaterial = basicMaterial;
            fxChipMaterial = basicMaterial;
            fxHoldMaterial = basicMaterial;

            laserMaterial = new Material("laser")
            {
                BlendMode = BlendMode.Additive,
            };
            laserEntryMaterial = laserMaterial;
            laserExitMaterial = laserMaterial;

            Camera = new BasicCamera();
            Camera.SetPerspectiveFoV(60, Window.Aspect, 0.01f, 1000);
            
            renderables.Fill(() => new Dictionary<OpenRM.Object, ObjectRenderable3D>());
        }

        public void Reset()
        {
            foreach (var r in renderables)
                r.Clear();
        }

        public void RenderableObjectAppear(OpenRM.Object obj)
        {
            if (obj.Stream >= 8) return;

            if (obj is ButtonObject bobj)
            {
                ButtonRenderState3D br3d;
                if (obj.IsInstant)
                    br3d = new ButtonRenderState3D(bobj, obj.Stream < 4 ? btChipMesh : fxChipMesh, 0);
                else
                {
                    float zDur = (float)(obj.AbsoluteDuration.Seconds / ViewDuration.Seconds);
                    br3d = new ButtonRenderState3D(bobj, obj.Stream < 4 ? btHoldMesh : fxHoldMesh, zDur * LENGTH_BASE);
                }

                renderables[obj.Stream][obj] = br3d;
            }
            else if (obj is AnalogObject aobj)
            {
                if (obj.IsInstant)
                {
                    float zDur = (float)(SlamDurationTime(aobj).Seconds / ViewDuration.Seconds);
                    renderables[obj.Stream][obj] = new SlamRenderState3D(aobj, zDur * LENGTH_BASE);
                }
                else
                {
                    time_t duration = obj.AbsoluteDuration;
                    if (aobj.PreviousConnected != null && aobj.Previous.IsInstant)
                        duration -= SlamDurationTime(aobj.PreviousConnected);

                    float zDur = (float)(duration.Seconds / ViewDuration.Seconds);
                    renderables[obj.Stream][obj] = new LaserRenderState3D(aobj, zDur * LENGTH_BASE);
                }
            }
        }

        public void RenderableObjectDisappear(OpenRM.Object obj)
        {
            if (obj.Stream >= 8) return;
            renderables[obj.Stream].Remove(obj);
        }

        public void Update()
        {
            Camera.ViewportWidth = Window.Width;
            Camera.ViewportHeight = Window.Height;

            void LerpTo(ref float value, float target, float speed = 10)
            {
                float diff = MathL.Abs(target - value);
                float change = diff * Time.Delta * 10;
                change = MathL.Min(speed * 0.02f, change);

                if (target < value)
                    value = MathL.Max(value - change, target);
                else value = MathL.Min(value + change, target);
            }
            
            //float rollScale = 1;
            //LerpTo(ref roll, TargetLaserRoll * rollScale, 10 * LaserRollSpeed);
            //LerpTo(ref roll, TargetLaserRoll * 10);
            roll = TargetLaserRoll;
            LerpTo(ref pitch, TargetPitch);
            LerpTo(ref zoom, TargetZoom);
            //LerpTo(ref rollBase, TargetBaseRoll);
            rollBase = TargetBaseRoll;
            
            Transform GetAtRoll(float roll, float xOffset, float highwayOffs = 0.5f)
            {
                //const float ANCHOR_Y = -0.825f;
                //const float CONTNR_Z = -1.1f;
                
                const float ANCHOR_ROT = 2.5f;
                const float ANCHOR_Y = -0.85f;
                const float CONTNR_Z = -0.875f;

                var origin = Transform.RotationZ(roll);
                var anchor = Transform.RotationX(ANCHOR_ROT)
                           * Transform.Translation(xOffset, ANCHOR_Y, 0);
                var contnr = Transform.Translation(0, 0, 0)
                           * Transform.RotationX(pitch * PITCH_AMT)
                           * Transform.Translation(0, 0, CONTNR_Z);

                return contnr * anchor * origin;
            }

            var worldNormal = GetAtRoll(rollBase * 360 + roll, TargetOffset);
            var worldNoRoll = GetAtRoll(0, 0, 0);

            var zoomDir = ((Matrix4x4)worldNormal).Translation;
            float highwayDist = zoomDir.Length();
            zoomDir = Vector3.Normalize(zoomDir);
            
            float zoomAmt;
            if (zoom <= 0) zoomAmt = MathL.Pow(ZOOM_POW, -zoom) - 1;
            else zoomAmt = highwayDist * (MathL.Pow(ZOOM_POW, -MathL.Pow(zoom, 1.35f)) - 1);

            WorldTransform = worldNormal * Transform.Translation(zoomDir * zoomAmt);

            var critDir = Vector3.Normalize(((Matrix4x4)worldNoRoll).Translation);
            float rotToCrit = MathL.Atan(critDir.Y, -critDir.Z);
            
            float cameraRot = Camera.FieldOfView / 2 - Camera.FieldOfView * critScreenY;
            float cameraPitch = rotToCrit + MathL.ToRadians(cameraRot);

            Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, cameraPitch, 0);

            float pitchDeg = MathL.ToDegrees(cameraPitch);
            HorizonHeight = (0.5f + (pitchDeg / Camera.FieldOfView)) * Camera.ViewportHeight;

            Camera.Position = CameraOffset;
            Camera.FarDistance = Vector3.Transform(new Vector3(0, 0, LENGTH_BASE), WorldTransform.Matrix).Length();
        }

        public void Render()
        {
            var renderState = new RenderState
            {
                ProjectionMatrix = Camera.ProjectionMatrix,
                CameraMatrix = Camera.ViewMatrix,
            };

            using (var queue = new RenderQueue(renderState))
            {
                //queue.Draw(Transform.Scale(100, 100, 100) * Transform.RotationX(90) * Transform.Translation(0, 0, -LENGTH_BASE), btChipMesh, btChipMaterial, btChipParams);

                var highwayParams = new MaterialParams();
                highwayParams["LeftColor"] = new Vector3(0.0f, 0.5f, 1);
                highwayParams["RightColor"] = new Vector3(1, 0.0f, 0.5f);
                highwayParams["Hidden"] = 0.0f;
                highwayParams["MainTexture"] = highwayTexture;
                queue.Draw(Transform.Translation(0, 0, 1) * WorldTransform, highwayMesh, highwayMaterial, highwayParams);

                void RenderButtonStream(int i)
                {
                    foreach (var objr in renderables[i].Values)
                    {
                        float z = LENGTH_BASE * (float)((objr.Object.AbsolutePosition - PlaybackPosition) / ViewDuration);
                        float xOffs = 0;
                        if (i < 4)
                            xOffs = -3 / 12.0f + i / 6.0f;
                        else xOffs = -1 / 6.0f + (i - 4) / 3.0f;

                        MaterialParams p;
                        if (i < 4)
                            p = objr.Object.IsInstant ? btChipParams : btHoldParams;
                        else p = objr.Object.IsInstant ? fxChipParams : fxHoldParams;

                        Transform t = objr.Transform * Transform.Translation(xOffs, 0, -z) * WorldTransform;
                        queue.Draw(t, objr.Mesh, basicMaterial, p);
                    }
                }

                void RenderAnalogStream(int i)
                {
                    const float FLOAT = 0.05f;

                    foreach (var objr in renderables[i + 6].Values)
                    {
                        var analog = objr.Object as AnalogObject;

                        time_t position = objr.Object.AbsolutePosition;
                        if (objr.Object.PreviousConnected != null && objr.Object.Previous.IsInstant)
                            position += SlamDurationTime(objr.Object.PreviousConnected);

                        float z = LENGTH_BASE * (float)((position - PlaybackPosition) / ViewDuration);

                        Transform t = objr.Transform * Transform.Translation(0, FLOAT, -z) * WorldTransform;
                        queue.Draw(t, objr.Mesh, laserMaterial, i == 0 ? lLaserParams : rLaserParams);

                        if (objr.Object.PreviousConnected == null)
                        {
                            float laneSpace = 5 / 6.0f;
                            if (analog.RangeExtended) laneSpace *= 2;

                            time_t entryPosition = objr.Object.AbsolutePosition;
                            float zEntry = LENGTH_BASE * (float)((entryPosition - PlaybackPosition) / ViewDuration);

                            Transform tEntry = Transform.Translation(((objr.Object as AnalogObject).InitialValue - 0.5f) * laneSpace, FLOAT, -zEntry) * WorldTransform;
                            queue.Draw(tEntry, laserEntryMesh, laserEntryMaterial, i == 0 ? lLaserEntryParams : rLaserEntryParams);
                        }

                        if (objr.Object.NextConnected == null)
                        {
                            float laneSpace = 5 / 6.0f;
                            if (analog.RangeExtended) laneSpace *= 2;

                            time_t exitPosition = objr.Object.AbsoluteEndPosition;
                            if (objr.Object.IsInstant)
                                exitPosition += SlamDurationTime(objr.Object);

                            float zExit = LENGTH_BASE * (float)((exitPosition - PlaybackPosition) / ViewDuration);

                            Transform tExit = Transform.Translation(((objr.Object as AnalogObject).FinalValue - 0.5f) * laneSpace, FLOAT, -zExit) * WorldTransform;
                            queue.Draw(tExit, laserExitMesh, laserExitMaterial, i == 0 ? lLaserExitParams : rLaserExitParams);
                        }
                    }
                }

                for (int i = 0; i < 2; i++)
                    RenderButtonStream(i + 4);

                for (int i = 0; i < 4; i++)
                    RenderButtonStream(i);

                for (int i = 0; i < 2; i++)
                    RenderAnalogStream(i);
            }
        }
    }
}

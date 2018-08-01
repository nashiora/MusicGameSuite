using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using FxMania.Graphics;
using FxMania.Input;

namespace FxMania.Game
{
    public class HighwayView
    {
        const string SCRIPT_SRC = @"
print(""Hello, theori!"");
Gamepad.ButtonPressed:add(function(button) print(button) end);
";

        private const float PITCH_AMT = 10;
        private const float ZOOM_POW = 1.65f;
        private const float LENGTH_BASE = 12;

        private float roll, rollBase;
        private float pitch, zoom; // top, bottom
        private float critScreenY = 0.05f;

        public readonly BasicCamera Camera;
        public Transform WorldTransform { get; private set; }
        
        private Texture highwayTexture;
        private Material highwayMaterial;
        private Mesh highwayMesh;

        public float LaserRoll => roll;
        public float CriticalHeight => (1 - critScreenY) * Camera.ViewportHeight;

        public float HorizonHeight { get; private set; }
        
        public float LaserRollSpeed { get; set; }

        public float TargetLaserRoll { get; set; }
        public float TargetBaseRoll { get; set; }
        
        public float TargetPitch { get; set; }
        public float TargetZoom { get; set; }

        public float TargetOffset { get; set; }
        
        public HighwayView()
        {
            highwayTexture = new Texture();
            highwayTexture.Load2DFromFile(@".\skins\Default\textures\highway.png");

            highwayMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1, 1, Anchor.BottomCenter);
            highwayMaterial = new Material("highway");

            Camera = new BasicCamera();
            Camera.SetPerspectiveFoV(60, Window.Aspect, 0.01f, 1000);
        }

        public void Update()
        {
            Camera.ViewportWidth = Window.Width;
            Camera.ViewportHeight = Window.Height;

            void LerpTo(ref float value, float target, float speed = 10)
            {
                float diff = Mathf.Abs(target - value);
                float change = diff * Time.Delta * 10;
                change = Mathf.Min(speed * 0.02f, change);

                if (target < value)
                    value = Mathf.Max(value - change, target);
                else value = Mathf.Min(value + change, target);
            }
            
            //LerpTo(ref roll, TargetLaserRoll * rollScale, 10 * LaserRollSpeed);
            roll = TargetLaserRoll;
            LerpTo(ref pitch, TargetPitch);
            LerpTo(ref zoom, TargetZoom);
            //LerpTo(ref rollBase, TargetBaseRoll);
            rollBase = TargetBaseRoll;
            
            Transform GetAtRoll(float roll, float xOffset, float highwayOffs = 0.5f)
            {
                //const float ANCHOR_Y = -0.825f;
                //const float CONTNR_Z = -1.1f;
                
                const float ANCHOR_Y = -0.75f;
                const float CONTNR_Z = -0.875f;

                var origin = Transform.RotationZ(roll);
                var anchor = Transform.RotationX(1.5f)
                           * Transform.Translation(xOffset, ANCHOR_Y, 0);
                var contnr = Transform.Translation(0, 0, highwayOffs / (LENGTH_BASE + highwayOffs))
                           * Transform.Scale(1, 1, LENGTH_BASE + highwayOffs)
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
            if (zoom <= 0) zoomAmt = Mathf.Pow(ZOOM_POW, -zoom) - 1;
            else zoomAmt = highwayDist * (Mathf.Pow(ZOOM_POW, -Mathf.Pow(zoom, 1.35f)) - 1);

            WorldTransform = worldNormal * Transform.Translation(zoomDir * zoomAmt);

            var critDir = Vector3.Normalize(((Matrix4x4)worldNoRoll).Translation);
            float rotToCrit = Mathf.Atan(critDir.Y, -critDir.Z);
            
            float cameraRot = Camera.FieldOfView / 2 - Camera.FieldOfView * critScreenY;
            float cameraPitch = rotToCrit + Mathf.ToRadians(cameraRot);

            Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, cameraPitch, 0);

            float pitchDeg = Mathf.ToDegrees(cameraPitch);
            HorizonHeight = (0.5f + (pitchDeg / Camera.FieldOfView)) * Camera.ViewportHeight;
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
                var highwayParams = new MaterialParams();
                highwayParams["LeftColor"] = new Vector3(0.0f, 0.12f, 1);
                highwayParams["RightColor"] = new Vector3(1, 0.0f, 0.12f);
                highwayParams["Hidden"] = 0.0f;
                highwayParams["MainTexture"] = highwayTexture;
                queue.Draw(WorldTransform, highwayMesh, highwayMaterial, highwayParams);
            }
        }
    }
}

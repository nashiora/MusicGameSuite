namespace theori.Graphics
{
    public sealed class RenderState
    {
        public Transform WorldTransform;
        public Transform ProjectionMatrix;
        public Transform CameraMatrix;

        public (int X, int Y) ViewportSize;

        public float AspectRatio;
        public float TotalTime;
    }
}

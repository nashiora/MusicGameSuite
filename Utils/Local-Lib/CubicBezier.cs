using System.Numerics;

namespace System
{
    public class CubicBezier
    {
		private float a, b, c, d;

        public CubicBezier() { }
        public CubicBezier(float a, float b, float c, float d)
        {
            Set(a, b, c, d);
        }

        public CubicBezier(Ease ease)
        {
            switch (ease)
            {
                case Ease.Linear: Set(0, 0, 1, 1); break;
                    
                case Ease.InQuad: Set(0.55f, 0.085f, 0.68f, 0.53f); break;
                case Ease.OutQuad: Set(0.25f, 0.46f, 0.45f, 0.94f); break;
                case Ease.InOutQuad: Set(0.455f, 0.03f, 0.515f, 0.955f); break;
                    
		        case Ease.InCubic: Set(0.55f, 0.055f, 0.675f, 0.19f); break;
		        case Ease.OutCubic: Set(0.215f, 0.61f, 0.355f, 1f); break;
		        case Ease.InOutCubic: Set(0.645f, 0.045f, 0.355f, 1f); break;

		        case Ease.InExpo: Set(0.95f, 0.05f, 0.795f, 0.035f); break;
		        case Ease.OutExpo: Set(0.19f, 1, 0.22f, 1); break;
		        case Ease.InOutExpo: Set(1, 0, 0, 1); break;
            }
        }

        private void Set(float a, float b, float c, float d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public float Sample(float t)
	    {
		    float inv = 1.0f - t;
		    float inv2 = inv * inv;
		    float inv3 = inv2 * inv;
		    float t2 = t * t;
		    float t3 = t2 * t;

		    Vector2 r =  Vector2.Zero * inv3 +
			    new Vector2(a, b) * 3 * inv2 * t +
			    new Vector2(c, d) * 3 * inv * t2 +
			    Vector2.One * t3;

		    return r.Y;
	    }
    }
}

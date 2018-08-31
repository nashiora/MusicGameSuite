namespace theori.Game
{
    public static class PositionFunc
    {
        public delegate float Type(float value);

        private static float InOut(float input, Type inFunc, Type outFunc) =>
            input < 0.5f ? inFunc(input * 2) / 2 : outFunc(input * 2 - 1) / 2 + 0.5f;

        public static float Linear(float input) => input;

        public static float QuadIn(float input) => input * input;
        public static float QuadOut(float input) => 1 - (input * input);
        public static float QuadInOut(float input) => InOut(input, QuadIn, QuadOut);

        public static float CubeIn(float input) => input * input * input;
        public static float CubeOut(float input) => 1 - (input * input * input);
        public static float CubeInOut(float input) => InOut(input, CubeIn, CubeOut);

        public static float QuartIn(float input) => input * input * input * input;
        public static float QuartOut(float input) => 1 - (input * input * input * input);
        public static float QuartInOut(float input) => InOut(input, QuartIn, QuartOut);

        public static float QuintIn(float input) => input * input * input * input * input;
        public static float QuintOut(float input) => 1 - (input * input * input * input * input);
        public static float QuintInOut(float input) => InOut(input, QuintIn, QuintOut);
    }
}

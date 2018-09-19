namespace System.Collections.Generic
{
    public static class System_Collections_Generic_Extensions
    {
        #region Deconstruct

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1)
        {
            if (list.Count != 2)
                throw new ArgumentException($"Expected list to have 2 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2)
        {
            if (list.Count != 3)
                throw new ArgumentException($"Expected list to have 3 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2, out T v3)
        {
            if (list.Count != 4)
                throw new ArgumentException($"Expected list to have 4 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
            v3 = list[3];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2, out T v3, out T v4)
        {
            if (list.Count != 5)
                throw new ArgumentException($"Expected list to have 5 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
            v3 = list[3];
            v4 = list[4];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5)
        {
            if (list.Count != 6)
                throw new ArgumentException($"Expected list to have 6 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
            v3 = list[3];
            v4 = list[4];
            v5 = list[5];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6)
        {
            if (list.Count != 7)
                throw new ArgumentException($"Expected list to have 7 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
            v3 = list[3];
            v4 = list[4];
            v5 = list[5];
            v6 = list[6];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7)
        {
            if (list.Count != 8)
                throw new ArgumentException($"Expected list to have 8 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
            v3 = list[3];
            v4 = list[4];
            v5 = list[5];
            v6 = list[6];
            v7 = list[7];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7, out T v8)
        {
            if (list.Count != 9)
                throw new ArgumentException($"Expected list to have 9 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
            v3 = list[3];
            v4 = list[4];
            v5 = list[5];
            v6 = list[6];
            v7 = list[7];
            v8 = list[8];
        }

        public static void Deconstruct<T>(this List<T> list, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7, out T v8, out T v9)
        {
            if (list.Count != 10)
                throw new ArgumentException($"Expected list to have 10 elements, but it has { list.Count }.");
            v0 = list[0];
            v1 = list[1];
            v2 = list[2];
            v3 = list[3];
            v4 = list[4];
            v5 = list[5];
            v6 = list[6];
            v7 = list[7];
            v8 = list[8];
            v9 = list[9];
        }

        #endregion
    }
}

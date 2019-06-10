using System.Collections.Generic;

namespace System
{
    public static class System_Array_Extensions
    {
        #region Deconstruct

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1)
        {
            if (array.Length != 2)
                throw new ArgumentException($"Expected array to have 2 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2)
        {
            if (array.Length != 3)
                throw new ArgumentException($"Expected array to have 3 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2, out T v3)
        {
            if (array.Length != 4)
                throw new ArgumentException($"Expected array to have 4 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
            v3 = array[3];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2, out T v3, out T v4)
        {
            if (array.Length != 5)
                throw new ArgumentException($"Expected array to have 5 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
            v3 = array[3];
            v4 = array[4];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5)
        {
            if (array.Length != 6)
                throw new ArgumentException($"Expected array to have 6 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
            v3 = array[3];
            v4 = array[4];
            v5 = array[5];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6)
        {
            if (array.Length != 7)
                throw new ArgumentException($"Expected array to have 7 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
            v3 = array[3];
            v4 = array[4];
            v5 = array[5];
            v6 = array[6];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7)
        {
            if (array.Length != 8)
                throw new ArgumentException($"Expected array to have 8 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
            v3 = array[3];
            v4 = array[4];
            v5 = array[5];
            v6 = array[6];
            v7 = array[7];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7, out T v8)
        {
            if (array.Length != 9)
                throw new ArgumentException($"Expected array to have 9 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
            v3 = array[3];
            v4 = array[4];
            v5 = array[5];
            v6 = array[6];
            v7 = array[7];
            v8 = array[8];
        }

        public static void Deconstruct<T>(this T[] array, out T v0, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7, out T v8, out T v9)
        {
            if (array.Length != 10)
                throw new ArgumentException($"Expected array to have 10 elements, but it has { array.Length }.");
            v0 = array[0];
            v1 = array[1];
            v2 = array[2];
            v3 = array[3];
            v4 = array[4];
            v5 = array[5];
            v6 = array[6];
            v7 = array[7];
            v8 = array[8];
            v9 = array[9];
        }

        #endregion

        #region IndexOf

        public static int IndexOf<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var v = arr[i];
                if (EqualityComparer<T>.Default.Equals(v, value))
                    return i;
            }

            return -1;
        }

        #endregion

        #region Fill

        public static T[] Fill<T>(this T[] arr, T value)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
                arr[i] = value;
            return arr;
        }

        public static T[] FillDefault<T>(this T[] arr)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
                arr[i] = default(T);
            return arr;
        }

        public static T[] Fill<T>(this T[] arr, Func<T> f)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
                arr[i] = f();
            return arr;
        }

        public static T[] Fill<T>(this T[] arr, Func<int, T> f)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
                arr[i] = f(i);
            return arr;
        }

        public static T[] Fill<T>(this T[] arr, int offset, int length, Func<int, T> f)
        {
            // TODO(local): proper bounds checking
            for (int i = offset, len = Math.Min(offset + length, arr.Length); i < len; i++)
                arr[i] = f(i);
            return arr;
        }

        public static T[] Fill<T>(this T[] arr, int offset, int length, T value)
        {
            // TODO(local): proper bounds checking
            for (int i = offset, len = Math.Min(offset + length, arr.Length); i < len; i++)
                arr[i] = value;
            return arr;
        }

        #endregion

        #region ForEach

        public static void ForEach<T>(this T[] arr, Action<T> a)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
                a(arr[i]);
        }

        public static void ForEach<T>(this T[] arr, Action<int, T> a)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
                a(i, arr[i]);
        }

        #endregion

        #region Copy

        public static T[] Copy<T>(this T[] arr)
        {
            var result = new T[arr.Length];
            arr.CopyTo(result, 0);
            return result;
        }

        #endregion

        #region CopyTo

        public static void CopyTo<T>(this T[] source, int sourceIndex, T[] dest, int destIndex)
        {
            for (int i = 0; i < source.Length - sourceIndex; i++)
                dest[destIndex + i] = source[sourceIndex + i];
        }

        #endregion

        #region Concat

        public static T[] Concat<T>(this T[] left, T[] right)
        {
            var result = new T[left.Length + right.Length];
            left.CopyTo(0, result, 0);
            right.CopyTo(0, result, left.Length);
            return result;
        }

        #endregion
    }
}

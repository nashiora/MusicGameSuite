namespace System
{
    public static class System_String_Extensions
    {
        #region Split

        public static bool TrySplit(this string s, char sep, out string v0, out string v1)
        {
            v0 = v1 = null;
            string[] results = s.Split(new[] { sep }, 2);
            if (results.Length != 2)
                return false;
            (v0, v1) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2)
        {
            v0 = v1 = v2 = null;
            string[] results = s.Split(new[] { sep }, 3);
            if (results.Length != 3)
                return false;
            (v0, v1, v2) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2, out string v3)
        {
            v0 = v1 = v2 = v3 = null;
            string[] results = s.Split(new[] { sep }, 4);
            if (results.Length != 4)
                return false;
            (v0, v1, v2, v3) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2, out string v3, out string v4)
        {
            v0 = v1 = v2 = v3 = v4 = null;
            string[] results = s.Split(new[] { sep }, 5);
            if (results.Length != 5)
                return false;
            (v0, v1, v2, v3, v4) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2, out string v3, out string v4, out string v5)
        {
            v0 = v1 = v2 = v3 = v4 = v5 = null;
            string[] results = s.Split(new[] { sep }, 6);
            if (results.Length != 6)
                return false;
            (v0, v1, v2, v3, v4, v5) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2, out string v3, out string v4, out string v5, out string v6)
        {
            v0 = v1 = v2 = v3 = v4 = v5 = v6 = null;
            string[] results = s.Split(new[] { sep }, 7);
            if (results.Length != 7)
                return false;
            (v0, v1, v2, v3, v4, v5, v6) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2, out string v3, out string v4, out string v5, out string v6, out string v7)
        {
            v0 = v1 = v2 = v3 = v4 = v5 = v6 = v7 = null;
            string[] results = s.Split(new[] { sep }, 8);
            if (results.Length != 8)
                return false;
            (v0, v1, v2, v3, v4, v5, v6, v7) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2, out string v3, out string v4, out string v5, out string v6, out string v7, out string v8)
        {
            v0 = v1 = v2 = v3 = v4 = v5 = v6 = v7 = v8 = null;
            string[] results = s.Split(new[] { sep }, 9);
            if (results.Length != 9)
                return false;
            (v0, v1, v2, v3, v4, v5, v6, v7, v8) = results;
            return true;
        }

        public static bool TrySplit(this string s, char sep, out string v0, out string v1, out string v2, out string v3, out string v4, out string v5, out string v6, out string v7, out string v8, out string v9)
        {
            v0 = v1 = v2 = v3 = v4 = v5 = v6 = v7 = v8 = v9 = null;
            string[] results = s.Split(new[] { sep }, 10);
            if (results.Length != 10)
                return false;
            (v0, v1, v2, v3, v4, v5, v6, v7, v8, v9) = results;
            return true;
        }

        #endregion
    }
}

namespace System
{
    public static class StringExt
    {
        public static bool Split(this string s, char c, out string a, out string b)
        {
            a = null;
            b = null;

            string[] pieces = s.Split(c);
            if (pieces.Length != 2)
                return false;

            a = pieces[0];
            b = pieces[1];

            return true;
        }
    }
}

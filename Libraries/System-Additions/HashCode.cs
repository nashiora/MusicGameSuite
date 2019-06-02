namespace System
{
    public static class HashCode
    {
        private const int INITIAL = 17;
        private const int MULT = 31;

        public static int For(params object[] args)
        {
            unchecked
            {
                int hash = INITIAL;
                if (args != null)
                {
                    for (int len = args.Length, i = 0; i < len; i++)
                        hash = hash * MULT + (args[i]?.GetHashCode() ?? 0);
                }

                return hash;
            }
        }

        public static int For<T>(params T[] args)
            where T : struct
        {
            unchecked
            {
                int hash = INITIAL;
                if (args != null)
                {
                    for (int len = args.Length, i = 0; i < len; i++)
                        hash = hash * MULT + args[i].GetHashCode();
                }

                return hash;
            }
        }
    }
}

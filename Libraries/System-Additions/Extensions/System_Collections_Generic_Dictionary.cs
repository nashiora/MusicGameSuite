namespace System.Collections.Generic
{
    public static class DictionartyExt
    {
        public static void AddRange<K, V>(this Dictionary<K, V> target, Dictionary<K, V> source)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
                target[element.Key] = element.Value;
        }
    }
}

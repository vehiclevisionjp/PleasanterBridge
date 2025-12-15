namespace PleasanterBridge.src.APIBridge.Utilities
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> Merge(
            this Dictionary<string, object> source,
            Dictionary<string, object> other)
        {
            var result = new Dictionary<string, object>(source);
            foreach (var kv in other)
            {
                result[kv.Key] = kv.Value;
            }
            return result;
        }
    }
}
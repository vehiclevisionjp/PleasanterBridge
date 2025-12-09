namespace PleasanterBridge.src.Utilities
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 2つのDictionary<string, object>を結合します。重複キーはother側で上書きされます。
        /// </summary>
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
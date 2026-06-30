namespace PleasanterBridge.src.APIBridge.Mapping
{
    internal static class PleasanterApiRequestKeyExtensions
    {
        public static string ToKeyString(this PleasanterApiRequestKey key)
        {
            return key.ToString();
        }
        public static string ToHashPrefix(this PleasanterApiRequestKey key)
        {
            return key.ToString();
        }
    }
}
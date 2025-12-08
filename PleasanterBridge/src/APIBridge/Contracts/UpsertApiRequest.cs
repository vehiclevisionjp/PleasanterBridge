namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class UpsertApiRequest : ApiRequestBase
    {
        public readonly List<string> Keys;

        public readonly List<Dictionary<string, object>> Data;

        public UpsertApiRequest(
            string apiKey,
            List<string> keys,
            List<Dictionary<string, object>> data
        ): base(apiKey)
        {
            Keys = keys;
            Data = data;
        }
    }
}

namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class UpsertApiRequest : ApiRequestBase
    {
        public List<string> Keys { get; }

        public List<Dictionary<string, object>> Data { get; }

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

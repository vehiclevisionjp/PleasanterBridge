namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts
{
    internal class UpsertApiRequest : ApiRequestBase
    {
        public readonly List<string> Keys;

        public readonly List<PleasanterApiRequest> Data;

        public UpsertApiRequest(
            string apiKey,
            List<string> keys,
            List<PleasanterApiRequest> data
        ): base(apiKey)
        {
            Keys = keys;
            Data = data;
        }
    }
}

namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class BulkInsertRequest : ApiRequestBase
    {
        public readonly List<Dictionary<string, object>> Data;

        public BulkInsertRequest(
            string apiKey,
            List<Dictionary<string, object>> data
        ): base(apiKey)
        {
            Data = data;
        }
    }
}

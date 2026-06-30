namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class BulkInsertRequest : ApiRequestBase
    {
        public List<Dictionary<string, object>> Data { get; }

        public BulkInsertRequest(
            string apiKey,
            List<Dictionary<string, object>> data
        ): base(apiKey)
        {
            Data = data;
        }
    }
}

namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts
{
    internal class BulkInsertRequest : ApiRequestBase
    {
        public readonly List<PleasanterApiRequest> Data;

        public BulkInsertRequest(
            string apiKey,
            List<PleasanterApiRequest> data
        ): base(apiKey)
        {
            Data = data;
        }
    }
}

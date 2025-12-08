namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts
{
    internal class ApiRequest: ApiRequestBase
    {
        public readonly PleasanterApiRequest PleasanterApiRequest;
        public ApiRequest(
            string apiKey,
            PleasanterApiRequest pleasanterApiRequest
        ): base(apiKey)
        {
            PleasanterApiRequest = pleasanterApiRequest;
        }
    }
}
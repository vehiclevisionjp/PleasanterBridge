namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class ApiRequest: ApiRequestBase
    {
        public Dictionary<string, object> PleasanterApiRequest { get; }
        public ApiRequest(
            string apiKey,
            Dictionary<string, object> pleasanterApiRequest
        ) : base(apiKey)
        {
            PleasanterApiRequest = pleasanterApiRequest;
        }
    }
}
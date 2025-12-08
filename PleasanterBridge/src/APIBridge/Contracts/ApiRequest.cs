namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class ApiRequest: ApiRequestBase
    {
        public readonly Dictionary<string, object> PleasanterApiRequest;
        public ApiRequest(
            string apiKey,
            Dictionary<string, object> pleasanterApiRequest
        ) : base(apiKey)
        {
            PleasanterApiRequest = pleasanterApiRequest;
        }
    }
}
namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class ApiRequestBase
    {
        public string ApiVersion { get; } = "1.1";

        public string ApiKey { get; }

        public ApiRequestBase(
            string apiKey
        )
        {
            ApiKey = apiKey;
        }
    }
}
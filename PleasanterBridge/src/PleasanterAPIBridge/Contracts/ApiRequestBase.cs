namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts
{
    internal class ApiRequestBase
    {
        public readonly string ApiVersion = "1.1";

        public readonly string ApiKey;
        public ApiRequestBase(
            string apiKey
        )
        {
            ApiKey = apiKey;
        }
    }
}
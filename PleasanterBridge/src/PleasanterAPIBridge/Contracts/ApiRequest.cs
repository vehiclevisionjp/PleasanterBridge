namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Request
{
    public class ApiRequest
    {
        public string ApiVersion { get; set; } = "1.1";
        public string ApiKey { get; set; }

        public ApiRequest(
            string apiKey
        )
        {
            ApiKey = apiKey;
        }
    }
}
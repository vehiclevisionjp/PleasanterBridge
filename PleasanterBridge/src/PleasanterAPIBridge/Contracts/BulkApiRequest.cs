namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Request
{
    public class BulkApiRequest
    {
        public string ApiVersion { get; set; } = "1.1";
        public string ApiKey { get; set; }

        public List<object> Data { get; set; }

        public BulkApiRequest(
            List<object> data,
            string apiKey
        )
        {
            ApiKey = apiKey;
            Data = data;
        }
    }
}

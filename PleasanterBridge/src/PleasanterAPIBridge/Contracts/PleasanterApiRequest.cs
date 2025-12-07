namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Request
{
    public class PleasanterApiRequest: ApiRequest
    {
        public Dictionary<string, string> ClassHash { get; set; } = new();
        public Dictionary<string, decimal> NumHash { get; set; } = new();
        public Dictionary<string, DateTime> DateHash { get; set; } = new();
        public Dictionary<string, string> DescriptionHash { get; set; } = new();
        public Dictionary<string, bool> CheckHash { get; set; } = new();
        public Dictionary<string, string> AttachmentsHash { get; set; } = new();

        public PleasanterApiRequest(string apiKey) : base(apiKey){}
    }
}

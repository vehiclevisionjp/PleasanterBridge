namespace PleasanterBridge.src.APIBridge.Configuration
{
    internal class PleasanterSettings: IPleasanterSettings
    {
        public string ApiyKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

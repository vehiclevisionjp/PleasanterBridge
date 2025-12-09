namespace PleasanterBridge.src.APIBridge.Configuration
{
    public class PleasanterSettings : IPleasanterSettings
    {
        public string ApiyKey { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    
        public PleasanterSettings(
            string apiKey,
            string url,
            string server,
            string database,
            string userId,
            string password
        )
        {
            ApiyKey = apiKey;
            Url = url;
            Server = server;
            Database = database;
            UserId = userId;
            Password = password;
        }
    }
}
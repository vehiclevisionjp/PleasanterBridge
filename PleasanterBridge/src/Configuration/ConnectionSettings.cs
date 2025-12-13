namespace PleasanterBridge.src.Configuration
{
    public class ConnectionSettings : IConnectionSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public DatabaseType DatabaseType { get; set; }


        public ConnectionSettings(
            string server,
            string database,
            string userId,
            string password,
            DatabaseType databaseType = DatabaseType.SqlServer
        )
        {
            Server = server;
            Database = database;
            UserId = userId;
            Password = password;
            DatabaseType = databaseType;
        }
    }
}

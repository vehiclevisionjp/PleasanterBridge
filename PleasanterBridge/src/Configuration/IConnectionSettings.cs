namespace PleasanterBridge.src.Configuration
{
    public interface IConnectionSettings
    {
        string Server { get; set; }
        string Database { get; set; }
        string UserId { get; set; }
        string Password { get; set; }
    }
}

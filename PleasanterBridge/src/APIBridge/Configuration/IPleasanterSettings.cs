using PleasanterBridge.src.Configuration;

namespace PleasanterBridge.src.APIBridge.Configuration
{
    public interface IPleasanterSettings : IConnectionSettings
    {
        string ApiyKey { get; set; }
        string Url { get; set; }
    }
}

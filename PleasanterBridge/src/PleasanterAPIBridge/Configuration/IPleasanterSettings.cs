using PleasanterAPIBridge.src.PleasanterRepository.Configuration;

namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Configuration
{
    public interface IPleasanterSettings : IConnectionSettings
    {
        string ApiyKey { get; set; }
        string BaseUrl { get; set; }
    }
}

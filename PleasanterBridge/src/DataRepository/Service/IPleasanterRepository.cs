using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IPleasanterRepository
    {
        public List<Dictionary<string, object>> Select(string id);
        public List<TablesEntity> Select(string id, bool isReturnEntity = false);
        public List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object>? keyValue);
        public List<TablesEntity> Select(string siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false);
    }
}
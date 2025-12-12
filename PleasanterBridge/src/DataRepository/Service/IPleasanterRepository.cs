using PleasanterBridge.src.DataRepository.Entity;
using PleasanterBridge.src.DataRepository.Utilities;

namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IPleasanterRepository
    {
        public List<Dictionary<string, object>> Select(string id);
        public List<TablesEntity> Select(string id, bool isReturnEntity = false);

        public List<Dictionary<string, object>> Select(List<string> referenceIds);

        public List<TablesEntity> Select(List<string> referenceIds, bool isReturnEntity = false);

        public List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object>? keyValue);

        public List<TablesEntity> Select(string siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false);
    }
}
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IPleasanterRepository
    {
        public List<Dictionary<string, object>> Select(int id);

        public List<TablesEntity> Select(int id, bool isReturnEntity = false);

        public List<Dictionary<string, object>> Select(List<int> referenceIds);

        public List<TablesEntity> Select(List<int> referenceIds, bool isReturnEntity = false);

        public List<Dictionary<string, object>> Select(int siteId, Dictionary<string, object>? keyValue);

        public List<TablesEntity> Select(int siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false);
    }
}
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IPleasanterRepository
    {
        public List<Dictionary<string, object>> Select(long id);

        public List<TablesEntity> Select(long id, bool isReturnEntity = false);

        public List<Dictionary<string, object>> Select(List<long> referenceIds);

        public List<TablesEntity> Select(List<long> referenceIds, bool isReturnEntity = false);

        public List<Dictionary<string, object>> Select(long siteId, Dictionary<string, object>? keyValue);

        public List<TablesEntity> Select(long siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false);
    }
}
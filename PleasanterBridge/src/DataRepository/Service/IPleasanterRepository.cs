namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IPleasanterRepository
    {
        public List<Dictionary<string, object>> Select(string id);
        public List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object>? keyValue);
    }
}
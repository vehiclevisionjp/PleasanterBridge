using PleasanterAPIBridge.src.PleasanterRepository.Entity;

namespace PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation
{
    public interface IGetSitesProcess
    {
        public List<Dictionary<string, object>> GetAll(string siteId);
        public string GetReferenceType(string siteId);
    }
}

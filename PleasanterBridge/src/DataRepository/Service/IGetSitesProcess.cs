namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IGetSitesProcess
    {
        public List<Dictionary<string, object>> GetAll(int siteId);
        public string GetReferenceType(int siteId);
    }
}

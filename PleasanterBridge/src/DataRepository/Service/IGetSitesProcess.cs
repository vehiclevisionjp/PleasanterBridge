namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IGetSitesProcess
    {
        public List<Dictionary<string, object>> GetAll(long siteId);
        public string GetReferenceType(long siteId);
    }
}

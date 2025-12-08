namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IGetSitesProcess
    {
        public List<Dictionary<string, object>> GetAll(string siteId);
        public string GetReferenceType(string siteId);
    }
}

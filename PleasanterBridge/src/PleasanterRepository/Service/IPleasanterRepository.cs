namespace PleasanterAPIBridge.src.PleasanterRepository.Service
{
    public interface IPleasanterRepository
    {
        /*
         * @param
         * id: key
         * target: Results,Issues,Wiki
         */
        public Dictionary<string, object> Select(string id);
        public List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object> keyValue);
    }
}
using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation;

namespace PleasanterAPIBridge.src.PleasanterRepository.Service
{
    public class PleasanterRepository : IPleasanterRepository
    {
        private readonly IConnectionSettings _settings;

        public PleasanterRepository(IConnectionSettings settings)
        {
            _settings = settings;
        }

        public Dictionary<string, object> Select(string id) {
            // itemsにアクセスしてReferenceTypeを取得

            GetReferenceTypeProcess getReferenceTypeProcess = new GetReferenceTypeProcess(_settings);
            string referenceType = getReferenceTypeProcess.Execute(id);
            switch(referenceType)
            {
                case "Sites":
                    break;
                case "Results":
                    GetResultsProcess getResultsProcess = new GetResultsProcess(_settings);
                    return getResultsProcess.Execute(id);
                case "Issues":
                    break;
                case "Wikis":
                    break;
            }
            return new Dictionary<string, object>();
        }

        public async List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object> keyValue)
        {

        }
    }
}
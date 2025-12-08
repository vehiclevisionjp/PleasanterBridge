using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation;

namespace PleasanterAPIBridge.src.PleasanterRepository.Service
{
    public class PleasanterRepository : IPleasanterRepository
    {
        private readonly IConnectionSettings _settings;
        private readonly IGetReferenceProcess GetReferenceProcess;

        public PleasanterRepository(IConnectionSettings settings)
        {
            _settings = settings;
            GetReferenceProcess = new GetReferenceProcess(_settings);
        }

        public Dictionary<string, object> Select(string id) {
            string referenceType = GetReferenceProcess.Execute(id, true);
            switch(referenceType)
            {
                case "Sites":
                    break;
                case "Results":
                    IGetResultsProcess getResultsProcess = new GetResultsProcess(_settings);
                    return getResultsProcess.Execute(id);
                case "Issues":
                    break;
                case "Wikis":
                    break;
            }
            return new Dictionary<string, object>();
        }

        public List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object>? keyValue = null)
        {
            if (keyValue == null)
            {
                IGetSitesProcess getSitesProcess = new GetSitesProcess(_settings);
                return getSitesProcess.GetAll(siteId);
            }

            string referenceType = GetReferenceProcess.Execute(siteId, false);

            switch (referenceType)
            {
                case "Sites":
                    break;
                case "Results":
                    GetResultsProcess getResultsProcess = new GetResultsProcess(_settings);
                    return getResultsProcess.Execute(keyValue);
                case "Issues":
                    break;
                case "Wikis":
                    break;
            }
            return new List<Dictionary<string, object>>();
        }
    }
}
using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;
using PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess;
using PleasanterBridge.src.PleasanterRepository.Process;

namespace PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation
{
    public class GetSitesProcess : SitesProcess, IGetSitesProcess
    {


        public GetSitesProcess(IConnectionSettings settings)
            : base(settings)
        {
        }

        public new List<Dictionary<string, object>> GetAll(string siteId)
        {
            List<SitesEntity> siteEntities =  base.GetAll(siteId);

            return EntityToDictionaryProcess.Convert(siteEntities);
        }

        public string GetReferenceType(string siteId)
        {
            SitesEntity sitesEntity = Get(siteId);

            if(sitesEntity?.ReferenceType == null)
            {
                throw new Exception("ReferenceId not found.");
            }

            return sitesEntity.ReferenceType;
        }
    }
}

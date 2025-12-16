using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess.Implementaion;
using PleasanterBridge.src.DataRepository.Entity;
using PleasanterBridge.src.DataRepository.Utilities;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    internal class GetSitesProcess : SitesProcess, IGetSitesProcess
    {
        public GetSitesProcess(IConnectionSettings settings)
            : base(settings){ }

        public new List<Dictionary<string, object>> GetAll(long siteId)
        {
            List<SitesEntity> siteEntities =  base.GetAll(siteId);

             return EntityToDictionaryProcess.Convert(siteEntities);
        }

        public string GetReferenceType(long siteId)
        {
            SitesEntity? sitesEntity = Get(siteId);

            if(sitesEntity?.ReferenceType == null)
            {
                 // SiteIdが見つからない場合は空文字列を返す
                return string.Empty;
            }
                return sitesEntity.ReferenceType;
        }
    }
}

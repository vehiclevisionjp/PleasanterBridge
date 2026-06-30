using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess.Implementaion;
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    internal class GetSitesProcess : SitesProcess, IGetSitesProcess
    {
        public GetSitesProcess(IConnectionSettings settings)
            : base(settings){ }

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

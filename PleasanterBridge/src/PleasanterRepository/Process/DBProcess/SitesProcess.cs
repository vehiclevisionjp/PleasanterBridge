using offlineMeeting.Models.DBProperty;
using offlineMeeting.Models.Share;
using Microsoft.EntityFrameworkCore;

namespace PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess
{
    public class SitesProcess : DBContext
    {
        protected DbSet<SitesEntity> Sites { get; set; }

        protected SitesProcess() { }

        protected List<SitesEntity> Get()
        {
            return Sites.ToList();
        }

        protected SitesEntity Get(int siteId)
        {
            return Sites
                .Where(x => Convert.ToInt32(x.SiteId) == siteId)
                .FirstOrDefault() ?? throw new Exception("invalid userCd");
        }
    }
}
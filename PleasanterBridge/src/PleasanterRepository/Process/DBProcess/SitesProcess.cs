using Microsoft.EntityFrameworkCore;
using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Data;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;

namespace PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess
{
    public class SitesProcess : DbContext, IDBContext
    {
        protected readonly IConnectionSettings Settings;
        protected DbSet<SitesEntity> Sites { get; set; }

        protected SitesProcess(IConnectionSettings settings)
        {
            Settings = settings;
        }

        protected List<SitesEntity> Get()
        {
            return Sites.ToList();
        }

        protected List<SitesEntity> GetAll(string siteId)
        {
            return Sites
                .Where(x => Convert.ToInt32(x.SiteId) == Convert.ToInt32(siteId))
                .ToList() ?? throw new Exception("invalid siteId");
        }

        protected SitesEntity Get(string siteId)
        {
            return Sites
                .Where(x => Convert.ToInt32(x.SiteId) == Convert.ToInt32(siteId))
                .FirstOrDefault() ?? throw new Exception("invalid siteId");
        }
    }
}
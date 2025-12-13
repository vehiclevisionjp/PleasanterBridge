using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Data;
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    internal class SitesProcess : DBContext
    {
        protected readonly IConnectionSettings Settings;
        protected DbSet<SitesEntity> Sites { get; set; }

        protected SitesProcess(IConnectionSettings settings): base(settings)
        {
            Settings = settings;
        }

        protected List<SitesEntity> Get()
        {
            return Sites.ToList();
        }

        protected List<SitesEntity> GetAll(int siteId)
        {
            return Sites
                .Where(x => Convert.ToInt64(x.SiteId) == Convert.ToInt64(siteId))
                .ToList() ?? throw new Exception("invalid siteId");
        }

        protected SitesEntity Get(int siteId)
        {
            return Sites
                .Where(x => Convert.ToInt64(x.SiteId) == Convert.ToInt64(siteId))
                .FirstOrDefault() ?? throw new Exception("invalid siteId");
        }
    }
}
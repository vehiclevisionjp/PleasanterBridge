using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Data;
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    public class SitesProcess : DBContext
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
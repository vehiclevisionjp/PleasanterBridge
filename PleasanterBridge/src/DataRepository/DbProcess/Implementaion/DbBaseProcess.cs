using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;
using System.Reflection;
using PleasanterBridge.src.DataRepository.Data;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    internal abstract class DbBaseProcess<TEntity> : DbContext, IDBContext
        where TEntity : class
    {
        protected readonly IConnectionSettings Settings;
        protected DbSet<TEntity> Entities { get; set; }

        protected DbBaseProcess(IConnectionSettings settings)
        {
            Settings = settings;
        }

        protected List<TEntity> GetAll()
        {
            return Entities.ToList();
        }

        protected List<TEntity> GetFromSiteId(string siteId)
        {
            PropertyInfo prop = typeof(TEntity).GetProperty("SiteId") 
                ?? throw new Exception("SiteId property not found");
            return Entities
                .Where(x => Convert.ToString(prop.GetValue(x)) == siteId)
                .ToList();
        }

        protected List<TEntity> GetFromSiteId(string siteId, List<string> idList, string idPropertyName)
        {
            PropertyInfo siteIdProp = typeof(TEntity).GetProperty("SiteId") ?? 
                throw new Exception("SiteId property not found");
            PropertyInfo idProp = typeof(TEntity).GetProperty(idPropertyName) ??
                throw new Exception($"{idPropertyName} property not found");
            return Entities
                .Where(x => Convert.ToString(siteIdProp.GetValue(x)) == siteId)
                .Where(y => idList.Contains(Convert.ToString(idProp.GetValue(y))!))
                .ToList();
        }

        protected TEntity GetSingleRecord(string referenceId, string idPropertyName)
        {
            PropertyInfo idProp = typeof(TEntity).GetProperty(idPropertyName) ?? 
                throw new Exception($"{idPropertyName} property not found");
            return Entities
                .Where(x => Convert.ToInt32(idProp.GetValue(x)) == Convert.ToInt32(referenceId))
                .FirstOrDefault() ?? throw new Exception($"invalid {idPropertyName}");
        }

        protected List<TEntity> GetFromIdList(List<string> idList, string idPropertyName)
        {
            PropertyInfo idProp = typeof(TEntity).GetProperty(idPropertyName) ?? 
                throw new Exception($"{idPropertyName} property not found");
            return Entities
                .Where(y => idList.Contains(Convert.ToString(idProp.GetValue(y))!))
                .ToList();
        }

        protected List<TEntity> GetFromDictionary(Dictionary<string, object> keyValue)
        {
            IQueryable<TEntity> query = Entities.Execute(keyValue);
            return query.ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;
using System.Linq.Expressions;
using System.Reflection;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    internal abstract class DbBaseProcess<TEntity> : DbSetTablesProcess
        where TEntity : class
    {
        protected readonly IConnectionSettings Settings;
        protected DbSet<TEntity> Entities { get; set; }

        protected DbBaseProcess(IConnectionSettings settings) : base(settings)
        {
            Settings = settings;

            Type? entityType = typeof(TEntity);

            string dbSetPropertyName = entityType.Name;
            if (dbSetPropertyName.EndsWith("Entity"))
            {
                dbSetPropertyName = dbSetPropertyName.Substring(0, dbSetPropertyName.Length - "Entity".Length);
            }

            PropertyInfo? dbSetProperty = typeof(DbSetTablesProcess)
                .GetProperty(dbSetPropertyName, BindingFlags.Public | BindingFlags.Instance);

            if (dbSetProperty == null)
            {
                throw new Exception($"DbSet property for {entityType.Name} not found in DbSetTablesProcess.");
            }

            Entities = (DbSet<TEntity>)dbSetProperty.GetValue(this)!;
        }

        protected List<TEntity> GetAll()
        {
            return Entities.ToList();
        }

        protected List<TEntity> GetFromSiteId(string siteId)
        {
            PropertyInfo prop = typeof(TEntity).GetProperty("SiteId") 
                ?? throw new Exception("SiteId property not found");

            Expression<Func<TEntity, bool>> predicate =
                Utilities.ExpressionBuilder.BuildEqualsExpression<TEntity, string>("SiteId", siteId);

            return Entities
                .Where(predicate)
                .ToList();
        }

        protected List<TEntity> GetFromSiteId(string siteId, List<string> idList, string idPropertyName)
        {
            Expression<Func<TEntity, bool>> siteIdPredicate =
                Utilities.ExpressionBuilder.BuildEqualsExpression<TEntity, string>("SiteId", siteId);

            Expression<Func<TEntity, bool>> idListPredicate =
                Utilities.ExpressionBuilder.BuildContainsExpression<TEntity>(idPropertyName, idList);

            Expression combinedBody = Expression.AndAlso(siteIdPredicate.Body, idListPredicate.Body);

            Expression<Func<TEntity, bool>> finalLambda = Expression.Lambda<Func<TEntity, bool>>(
                combinedBody,
                siteIdPredicate.Parameters.Single()
            );

            return Entities
                .Where(finalLambda)
                .ToList();
        }

        protected TEntity GetSingleRecord(string referenceId, string idPropertyName)
        {
            Expression<Func<TEntity, bool>> lambda =
                Utilities.ExpressionBuilder.BuildEqualsExpression<TEntity, int>(idPropertyName, Convert.ToInt32(referenceId));

            return Entities
                .Where(lambda)
                .FirstOrDefault()
                ?? throw new Exception($"invalid {idPropertyName}");
        }

        protected List<TEntity> GetFromIdList(List<string> idList, string idPropertyName)
        {
            Expression<Func<TEntity, bool>> idListPredicate =
                Utilities.ExpressionBuilder.BuildContainsExpression<TEntity>(idPropertyName, idList);

            return Entities
                .Where(idListPredicate)
                .ToList();
        }

        protected List<TEntity> GetFromDictionary(Dictionary<string, object> keyValue)
        {
            IQueryable<TEntity> query = Entities.Execute(keyValue);
            return query.ToList();
        }
    }
}

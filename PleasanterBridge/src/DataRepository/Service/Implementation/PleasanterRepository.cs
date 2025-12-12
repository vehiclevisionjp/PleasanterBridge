using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Entity;
using PleasanterBridge.src.DataRepository.Utilities;
using PleasanterBridge.src.Utilities;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    public class PleasanterRepository : IPleasanterRepository
    {
        private readonly IDbProcessProvider _dbProcessProvider;

        public PleasanterRepository(
            string server, 
            string database, 
            string userId, 
            string password,
            DatabaseType databaseType = DatabaseType.SqlServer)
            : this(new DbProcessProvider(new ConnectionSettings(server, database, userId, password, databaseType)))
        {
        }

        public PleasanterRepository(IConnectionSettings settings)
            : this(new DbProcessProvider(settings))
        {
        }

        public PleasanterRepository(IDbProcessProvider dbProcessProvider)
        {
            _dbProcessProvider = dbProcessProvider;
        }

        public List<Dictionary<string, object>> Select(int referenceId)
        {
            return SelectByReferenceType(referenceId, true);
        }

        public List<TablesEntity> Select(int referenceId, bool isReturnEntity = false)
        {
            return DictionaryToTablesEntityConverter.Convert(Select(referenceId));
        }

        public List<Dictionary<string, object>> Select(List<int> referenceIds)
        {
            return referenceIds.Select(x => SelectByReferenceType(x, true))
                              .SelectMany(x => x)
                              .ToList();
        }

        public List<TablesEntity> Select(List<int> referenceIds, bool isReturnEntity = false)
        {
            return DictionaryToTablesEntityConverter.Convert(Select(referenceIds));
        }

        public List<Dictionary<string, object>> Select(int siteId, Dictionary<string, object>? keyValue)
        {
            if (keyValue == null)
            {
                IGetSitesProcess getSitesProcess = _dbProcessProvider.GetSitesProcess();
                return getSitesProcess.GetAll(siteId);
            }
            return SelectByReferenceType(siteId, false, keyValue);
        }

        public List<TablesEntity> Select(int siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false)
        {
            return DictionaryToTablesEntityConverter.Convert(Select(siteId, keyValue));
        }

        private List<Dictionary<string, object>> SelectByReferenceType(int id, bool isReferenceId, Dictionary<string, object>? keyValue = null)
        {
            IGetReferenceProcess referenceProcess = _dbProcessProvider.GetReferenceProcess();

            string referenceType = referenceProcess.Execute(id, isReferenceId);
            PleasanterReferenceType pleasanterReferenceType = PleasanterReferenceTypeExtensions.ParseReferenceType(referenceType);

            IDbExecutionProcess process = _dbProcessProvider.GetDbProcessFactory().Create(pleasanterReferenceType);

            return process.Execute(id, isReferenceId, keyValue);
        }
    }
}
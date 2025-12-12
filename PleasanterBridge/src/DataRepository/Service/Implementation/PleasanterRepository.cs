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

        public List<Dictionary<string, object>> Select(string referenceId)
        {
            return SelectByReferenceType(referenceId, true);
        }

        public List<TablesEntity> Select(string referenceId, bool isReturnEntity = false)
        {
            return DictionaryToTablesEntityConverter.Convert(SelectByReferenceType(referenceId, true));
        }

        public List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object>? keyValue)
        {
            if (keyValue == null)
            {
                IGetSitesProcess getSitesProcess = _dbProcessProvider.GetSitesProcess();
                return getSitesProcess.GetAll(siteId);
            }
            return SelectByReferenceType(siteId, false, keyValue);
        }

        public List<TablesEntity> Select(string siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false)
        {
            if (keyValue == null)
            {
                IGetSitesProcess getSitesProcess = _dbProcessProvider.GetSitesProcess();
                return DictionaryToTablesEntityConverter.Convert(getSitesProcess.GetAll(siteId));
            }
            return DictionaryToTablesEntityConverter.Convert(SelectByReferenceType(siteId, false, keyValue));
        }

        private List<Dictionary<string, object>> SelectByReferenceType(string id, bool isReferenceId, Dictionary<string, object>? keyValue = null)
        {
            IGetReferenceProcess referenceProcess = _dbProcessProvider.GetReferenceProcess();

            string referenceType = referenceProcess.Execute(id, isReferenceId);
            PleasanterReferenceType pleasanterReferenceType = PleasanterReferenceTypeExtensions.ParseReferenceType(referenceType);

            IDbExecutionProcess process = _dbProcessProvider.GetDbProcessFactory().Create(pleasanterReferenceType);

            return process.Execute(id, isReferenceId, keyValue);
        }
    }
}
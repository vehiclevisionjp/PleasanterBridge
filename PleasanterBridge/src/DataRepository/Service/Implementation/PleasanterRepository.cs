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

        public List<Dictionary<string, object>> Select(long referenceId)
        {
            return SelectByReferenceType(referenceId, true);
        }

        public List<TablesEntity> Select(long referenceId, bool isReturnEntity = false)
        {
            return DictionaryToTablesEntityConverter.Convert(Select(referenceId));
        }

        public List<Dictionary<string, object>> Select(List<long> referenceIds)
        {
            return referenceIds.Select(x => SelectByReferenceType(x, true))
                .SelectMany(x => x)
                .ToList();
        }

        public List<TablesEntity> Select(List<long> referenceIds, bool isReturnEntity = false)
        {
            return DictionaryToTablesEntityConverter.Convert(Select(referenceIds));
        }

        public List<Dictionary<string, object>> Select(long siteId, Dictionary<string, object>? keyValue)
        {
            return SelectByReferenceType(siteId, false, keyValue);
        }

        public List<TablesEntity> Select(long siteId, Dictionary<string, object>? keyValue, bool isReturnEntity = false)
        {
            return DictionaryToTablesEntityConverter.Convert(Select(siteId, keyValue));
        }

        private List<Dictionary<string, object>> SelectByReferenceType(long id, bool isReferenceId, Dictionary<string, object>? keyValue = null)
        {
            IGetReferenceProcess referenceProcess = _dbProcessProvider.GetReferenceProcess();

            string referenceType = referenceProcess.Execute(id, isReferenceId);
            PleasanterReferenceType? pleasanterReferenceType = PleasanterReferenceTypeExtensions.ParseReferenceType(referenceType);

            // referenceTypeが取得できなかった場合（IDがマッチしない場合）は空のリストを返す
            if (pleasanterReferenceType == null)
            {
                return new List<Dictionary<string, object>>();
            }

            IDbExecutionProcess process = _dbProcessProvider.GetDbProcessFactory().Create(pleasanterReferenceType.Value);

            return process.Execute(id, isReferenceId, keyValue);
        }
    }
}
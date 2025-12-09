using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess.Implementaion;
using PleasanterBridge.src.DataRepository.Service.Implementation;
using PleasanterBridge.src.Utilities;

namespace PleasanterBridge.src.DataRepository.Service
{
    public class PleasanterRepository : IPleasanterRepository
    {
        private readonly IConnectionSettings _settings;
        private readonly IGetReferenceProcess GetReferenceProcess;
        private readonly IDbProcessFactory _dbProcessFactory;

        public PleasanterRepository(
            string server,
            string database,
            string userId,
            string password
        )
        {
            _settings = new ConnectionSettings(
                server,
                database,
                userId,
                password
            );
            GetReferenceProcess = new GetReferenceProcess(_settings); 
            _dbProcessFactory = new DbProcessFactory(_settings);
        }

        public PleasanterRepository(IConnectionSettings settings): this(
            settings.Server,
            settings.Database,
            settings.UserId,
            settings.Password
        )
        {
        }

        public List<Dictionary<string, object>> Select(string referenceId)
        {
            return SelectByReferenceType(referenceId, true);
        }

        public List<Dictionary<string, object>> Select(string siteId, Dictionary<string, object>? keyValue = null)
        {
            if (keyValue == null)
            {
                IGetSitesProcess getSitesProcess = new GetSitesProcess(_settings);
                return getSitesProcess.GetAll(siteId);
            }
            return SelectByReferenceType(siteId, false, keyValue);
        }

        private List<Dictionary<string, object>> SelectByReferenceType(string id, bool isReferenceId, Dictionary<string, object>? keyValue = null)
        {
            string referenceType = GetReferenceProcess.Execute(id, isReferenceId);
            PleasanterReferenceType pleasanterReferenceType = PleasanterReferenceTypeExtensions.ParseReferenceType(referenceType);
            IDbExecutionProcess process = _dbProcessFactory.Create(pleasanterReferenceType);

            return process.Execute(id, isReferenceId, keyValue);
        }
    }
}
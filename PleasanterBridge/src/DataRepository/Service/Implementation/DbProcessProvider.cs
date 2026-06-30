using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess;
using PleasanterBridge.src.DataRepository.DbProcess.Implementaion;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    internal class DbProcessProvider : IDbProcessProvider
    {
        private readonly IConnectionSettings _settings;

        public DbProcessProvider(IConnectionSettings settings)
        {
            _settings = settings;
        }

        public IDbProcessFactory GetDbProcessFactory()
        {
            return new DbProcessFactory(_settings);
        }

        public IGetReferenceProcess GetReferenceProcess()
        {
            return new GetReferenceProcess(_settings);
        }

        public IGetSitesProcess GetSitesProcess()
        {
            return new GetSitesProcess(_settings);
        }
    }
}

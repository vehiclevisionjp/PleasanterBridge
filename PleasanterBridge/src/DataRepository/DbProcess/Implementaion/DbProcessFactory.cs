using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Entity;
using PleasanterBridge.src.DataRepository.Service.Implementation;
using PleasanterBridge.src.Utilities;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    internal class DbProcessFactory: IDbProcessFactory
    {
        private readonly IConnectionSettings _settings;

        public DbProcessFactory(IConnectionSettings settings)
        {
            _settings = settings;
        }

        public IDbExecutionProcess Create(PleasanterReferenceType type)
        {
            return type switch
            {
                PleasanterReferenceType.Results =>
                    (IDbExecutionProcess)new GetDbBaseProcess<ResultsEntity>(_settings, PleasanterIdPropertyName.ResultId),

                PleasanterReferenceType.Issues =>
                    (IDbExecutionProcess)new GetDbBaseProcess<IssuesEntity>(_settings, PleasanterIdPropertyName.IssueId),

                PleasanterReferenceType.Wikis =>
                    (IDbExecutionProcess)new GetDbBaseProcess<WikisEntity>(_settings, PleasanterIdPropertyName.WikiId),

                _ => throw new InvalidOperationException($"Unsupported reference type: {type}")
            };
        }
    }
}

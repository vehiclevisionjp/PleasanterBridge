using PleasanterBridge.src.Configuration;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    internal class GetReferenceProcess : IGetReferenceProcess
    {
        private readonly IConnectionSettings _settings;

        public GetReferenceProcess(IConnectionSettings settings)
        {
            _settings = settings;
        }

        public string Execute(int id, bool isReferenceId)
        {
            if(isReferenceId)
            {
                IGetItemsProcess getItemsProcess = new GetItemsProcess(_settings);
                return getItemsProcess.GetReferenceType(id);
            }
            else
            {
                IGetSitesProcess getSitesProcess = new GetSitesProcess(_settings);
                return getSitesProcess.GetReferenceType(id);
            }
        }
    }
}
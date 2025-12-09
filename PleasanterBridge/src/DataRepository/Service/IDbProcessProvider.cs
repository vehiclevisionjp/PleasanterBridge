using PleasanterBridge.src.DataRepository.DbProcess;

namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IDbProcessProvider
    {
        IDbProcessFactory GetDbProcessFactory();

        IGetReferenceProcess GetReferenceProcess();

        IGetSitesProcess GetSitesProcess();
    }
}

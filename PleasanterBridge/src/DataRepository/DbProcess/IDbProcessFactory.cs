using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.Utilities;

namespace PleasanterBridge.src.DataRepository.DbProcess;
public interface IDbProcessFactory
{
    IDbExecutionProcess Create(PleasanterReferenceType type);
}
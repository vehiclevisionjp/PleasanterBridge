namespace PleasanterBridge.src.DataRepository.Service;

    public interface IDbExecutionProcess
{
    List<Dictionary<string, object>> Execute(
        int id,
        bool isReferenceId,
        Dictionary<string, object>? keyValue = null
    );
}
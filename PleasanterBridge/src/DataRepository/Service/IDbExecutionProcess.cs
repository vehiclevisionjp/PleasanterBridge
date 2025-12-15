namespace PleasanterBridge.src.DataRepository.Service;

    public interface IDbExecutionProcess
{
    List<Dictionary<string, object>> Execute(
        long id,
        bool isReferenceId,
        Dictionary<string, object>? keyValue = null
    );
}
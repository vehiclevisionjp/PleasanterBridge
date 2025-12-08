internal interface IDbExecutionProcess
{
    List<Dictionary<string, object>> Execute(
        string id,
        bool isReferenceId,
        Dictionary<string, object>? keyValue = null
    );
}
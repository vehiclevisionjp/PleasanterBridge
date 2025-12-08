using PleasanterBridge.src.Utilities;

internal interface IDbProcessFactory
{
    IDbExecutionProcess Create(PleasanterReferenceType type);
}
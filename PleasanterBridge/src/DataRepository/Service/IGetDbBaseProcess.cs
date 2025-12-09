namespace PleasanterBridge.src.DataRepository.Service
{
    internal interface IGetDbBaseProcess<TEntity> : IDbExecutionProcess
    where TEntity : class
    {
    }
}

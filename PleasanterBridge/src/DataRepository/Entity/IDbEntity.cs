namespace PleasanterBridge.src.DataRepository.Entity
{
    public interface IDbEntity { }

    public interface IDbRepository<TEntity> 
        where TEntity : IDbEntity 
    {
    }
}

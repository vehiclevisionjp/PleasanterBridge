namespace PleasanterAPIBridge.src.PleasanterRepository.Entity
{
    // ファイル: Entity/IDbEntity.cs (すべての固定エンティティが継承)
    public interface IDbEntity { }

    // ファイル: Repository/IDbRepository.cs (すべてのリポジトリが継承)
    public interface IDbRepository<TEntity> where TEntity : IDbEntity { /* SelectAsync(string) などの共通メソッド */ }
}

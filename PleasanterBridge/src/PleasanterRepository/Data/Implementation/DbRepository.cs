using Microsoft.EntityFrameworkCore;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;

namespace PleasanterAPIBridge.src.PleasanterRepository.Data.Implementation
{
    public abstract class DbRepositoryBase<TContext, TEntity> : IDbRepository<TEntity>
    where TContext : DbContext, IDBContext
    where TEntity : class, IDbEntity, new()
    {
        protected readonly TContext Context;

        // 💡 コンストラクタで特定のDbContextの実装を受け取る
        public DbRepositoryBase(IDBContext context)
        {
            // 受け取ったIDBContextが、このリポジトリが要求するTContextであるかチェック
            Context = context as TContext ??
                      throw new InvalidOperationException($"Injected context must be of type {typeof(TContext).Name}.");
        }

        // --- 💡 抽象メソッド: DbSetへのアクセス方法を継承クラスに強制 ---
        // これにより、継承クラスは「どのテーブルを触るか」という固定ロジックだけを実装する
        protected abstract DbSet<TEntity> GetDbSet(TContext context);

        // --- 💡 共通のSelect実装 ---
        public async Task<IEnumerable<TEntity>> SelectAsync(string condition = "", object parameters = null)
        {
            // 固有のTContextを使って、DbSetにアクセスし、TEntityにマッピング
            return await GetDbSet(Context)
                         .Where(/* conditionに基づいたフィルタリング */)
                         .ToListAsync();
        }
    }
}

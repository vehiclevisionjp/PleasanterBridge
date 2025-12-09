using System.Linq.Expressions;

namespace PleasanterBridge.src.DataRepository.Utilities
{
    // 💡 ユーザーはアクセスしないユーティリティのため、internal static で定義
    internal static class ExpressionBuilder
    {
        /// <summary>
        /// 指定されたプロパティ名と値に基づいて、DBで実行可能な等価比較 (x => x.Prop == value) のラムダ式を構築します。
        /// </summary>
        /// <typeparam name="TEntity">エンティティ型</typeparam>
        /// <typeparam name="TProperty">プロパティの比較型 (例: int, string)</typeparam>
        /// <param name="propertyName">比較対象のプロパティ名 (例: "SiteId")</param>
        /// <param name="value">比較対象の値</param>
        /// <returns>Where句に使用可能な Expression<Func<TEntity, bool>> </returns>
        public static Expression<Func<TEntity, bool>> BuildEqualsExpression<TEntity, TProperty>(
            string propertyName,
            TProperty value)
            where TEntity : class
        {
            var entityParam = Expression.Parameter(typeof(TEntity), "x");
            var propertyInfo = typeof(TEntity).GetProperty(propertyName)
                ?? throw new Exception($"{propertyName} property not found on {typeof(TEntity).Name}");
            var propertyAccess = Expression.Property(entityParam, propertyInfo);

            // プロパティ型と値の型が異なる場合は変換
            Expression right;
            if (propertyInfo.PropertyType != typeof(TProperty))
            {
                right = Expression.Convert(Expression.Constant(value, typeof(TProperty)), propertyInfo.PropertyType);
            }
            else
            {
                right = Expression.Constant(value, typeof(TProperty));
            }

            var comparison = Expression.Equal(propertyAccess, right);
            return Expression.Lambda<Func<TEntity, bool>>(comparison, entityParam);
        }

        /// <summary>
        /// 指定されたプロパティの値がリストに含まれるかを確認する Where 句 (SQLの IN 句に相当) を構築します。
        /// </summary>
        /// <typeparam name="TEntity">エンティティ型</typeparam>
        /// <param name="propertyName">比較対象のプロパティ名 (例: "IssueId")</param>
        /// <param name="idList">含むかどうかを判定する文字列のリスト</param>
        /// <returns>Where句に使用可能な Expression<Func<TEntity, bool>> </returns>
        public static Expression<Func<TEntity, bool>> BuildContainsExpression<TEntity>(
            string propertyName,
            List<string> idList)
            where TEntity : class
        {
            var entityParam = Expression.Parameter(typeof(TEntity), "x");
            var propertyInfo = typeof(TEntity).GetProperty(propertyName)
                ?? throw new Exception($"{propertyName} property not found on {typeof(TEntity).Name}");
            var propertyAccess = Expression.Property(entityParam, propertyInfo);

            Expression valueExpression;
            if (propertyInfo.PropertyType != typeof(string))
            {
                // 型がstring以外の場合はToString()で変換
                var toStringMethod = typeof(Convert).GetMethod("ToString", new[] { typeof(object) })
                    ?? throw new InvalidOperationException("Convert.ToString(object) not found.");
                valueExpression = Expression.Call(
                    toStringMethod,
                    Expression.Convert(propertyAccess, typeof(object))
                );
            }
            else
            {
                valueExpression = propertyAccess;
            }

            var listConstant = Expression.Constant(idList, typeof(List<string>));
            var containsMethod = typeof(List<string>).GetMethod("Contains", new[] { typeof(string) })
                ?? throw new InvalidOperationException("List<string>.Contains(string) not found.");

            var body = Expression.Call(listConstant, containsMethod, valueExpression);
            return Expression.Lambda<Func<TEntity, bool>>(body, entityParam);
        }
    }
}
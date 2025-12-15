using System.Linq.Expressions;

namespace PleasanterBridge.src.DataRepository.Utilities
{
    internal static class ExpressionBuilder
    {
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

        public static Expression<Func<TEntity, bool>> BuildContainsExpression<TEntity>(
            string propertyName,
            List<long> idList)
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
using System.Linq.Expressions;
public static class WhereByDictionaryProcess
{
    public static IQueryable<T> Execute<T>(this IQueryable<T> source, Dictionary<string, object> conditions)
    {
        if (conditions == null || conditions.Count == 0) return source;

        ParameterExpression param = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        foreach (var kvp in conditions)
        {
            var property = Expression.PropertyOrField(param, kvp.Key);
            var constant = Expression.Constant(kvp.Value, property.Type);

            // property == value
            var equal = Expression.Equal(property, constant);

            body = body == null ? equal : Expression.AndAlso(body, equal);
        }

        if (body == null) return source;

        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
        return source.Where(lambda);
    }
}
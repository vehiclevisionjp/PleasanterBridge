using System.Linq.Expressions;

internal static class WhereByDictionaryProcess
{
    public static IQueryable<T> Execute<T>(this IQueryable<T> source, Dictionary<string, object> conditions)
    {
 if (conditions == null || conditions.Count == 0) return source;

        ParameterExpression param = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        foreach (var kvp in conditions)
        {
            var property = Expression.PropertyOrField(param, kvp.Key);
                Expression condition;

            // Classカラム(ClassA～ClassZ)かつ値が配列型の場合をチェック
            if (IsClassColumn(kvp.Key) && IsArrayType(kvp.Value))
            {
                condition = BuildArrayContainsExpression(property, kvp.Value);
            }
            else
            {
                var constant = Expression.Constant(kvp.Value, property.Type);
                condition = Expression.Equal(property, constant);
            }

            body = body == null ? condition : Expression.AndAlso(body, condition);
        }

        if (body == null) return source;

        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
        return source.Where(lambda);
    }

    private static bool IsClassColumn(string propertyName)
    {
        if (propertyName.Length != 6 || !propertyName.StartsWith("Class"))
            return false;

        char lastChar = propertyName[5];
            return lastChar >= 'A' && lastChar <= 'Z';
    }

    private static bool IsArrayType(object value)
    {
        if (value is string[] || value is List<string>)
            return true;

        return false;
    }

    private static Expression BuildArrayContainsExpression(Expression property, object arrayValue)
    {
        // 入力値をList<string>に変換
        List<string> searchValues = arrayValue switch
        {
            string[] arr => arr.ToList(),
            List<string> list => list,
            _ => throw new ArgumentException("配列値はstring[]またはList<string>である必要があります")
        };

        if (searchValues.Count == 0)
        {
            // 検索配列が空の場合、false条件を返す
            return Expression.Constant(false);
        }

        // 式を構築: searchValues.Any(searchValue => property.Contains(searchValue))
        var searchValuesConstant = Expression.Constant(searchValues);
        var anyMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(string));

        // Any述語用のラムダパラメータ: searchValue
        var searchValueParam = Expression.Parameter(typeof(string), "searchValue");

        // 構築: property.Contains(searchValue)
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        var containsCall = Expression.Call(property, containsMethod, searchValueParam);

        // ラムダを構築: searchValue => property.Contains(searchValue)
        var containsLambda = Expression.Lambda<Func<string, bool>>(containsCall, searchValueParam);

        // 構築: searchValues.Any(searchValue => property.Contains(searchValue))
        var anyCall = Expression.Call(null, anyMethod, searchValuesConstant, containsLambda);

        return anyCall;
    }
}
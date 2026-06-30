namespace PleasanterBridge.src.Utilities
{
    internal static class RequestToDictionaryProcess
    {
        public static Dictionary<string, object> Convert(object entity)
        {
            return entity.GetType()
                .GetProperties()
                .Where(prop =>
                {
                    var value = prop.GetValue(entity);
                    if (value == null) return false;
                    // 空文字列の場合は除外
                    if (value is string str && string.IsNullOrEmpty(str)) return false;
                    // プロパティがDictionary型かつ空の場合は除外
                    var type = value.GetType();
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        var countProp = type.GetProperty("Count");
                        if (countProp != null && (int)countProp.GetValue(value)! == 0)
                            return false;
                    }
                    return true;
                })
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(entity)!
                );
        }
    }
}

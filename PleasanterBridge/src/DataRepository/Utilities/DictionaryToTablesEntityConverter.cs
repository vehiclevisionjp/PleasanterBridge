using PleasanterBridge.src.DataRepository.Entity;
using System.Reflection;

namespace PleasanterBridge.src.DataRepository.Utilities
{
    internal static class DictionaryToTablesEntityConverter
    {
        public static List<TablesEntity> Convert(List<Dictionary<string, object>> dictionaries)
        {
            return dictionaries.Select(Convert).ToList();
        }

        private static TablesEntity Convert(Dictionary<string, object> dictionary)
        {
            var entity = new TablesEntity();

            if (dictionary == null || dictionary.Count == 0)
            {
                return entity;
            }

            var properties = typeof(TablesEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                // Dictionary キーを複数パターンで探索
                string? dictionaryKey = dictionary.Keys.FirstOrDefault(k =>
                    string.Equals(k, property.Name, StringComparison.OrdinalIgnoreCase)
                );

                if (dictionaryKey != null && dictionary.TryGetValue(dictionaryKey, out object? value))
                {
                    try
                    {
                        if (value == null)
                        {
                            property.SetValue(entity, null);
                        }
                        else if (property.PropertyType == value.GetType())
                        {
                            property.SetValue(entity, value);
                        }
                        else if (property.PropertyType == typeof(string) && value is List<object>)
                        {
                            // バイナリ画像パース済みのList<object>はそのまま設定
                            property.SetValue(entity, value);
                        }
                        else if (property.PropertyType == typeof(List<object>) && value is string)
                        {
                            // 文字列をList<object>に変換
                            property.SetValue(entity, new List<object> { value });
                        }
                        else
                        {
                            // 型の変換を試みる
                            try
                            {
                                object? convertedValue = ConvertValue(value, property.PropertyType);
                                property.SetValue(entity, convertedValue);
                            }
                            catch
                            {
                                // 変換失敗時はスキップ
                            }
                        }
                    }
                    catch
                    {
                        // 設定失敗時は次のプロパティへ
                    }
                }
            }

            return entity;
        }

        private static object? ConvertValue(object? value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (targetType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            // Nullable 型の処理
            Type? underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                return System.Convert.ChangeType(value, underlyingType);
            }
            catch
            {
                return null;
            }
        }
    }
}
using PleasanterBridge.src.APIBridge.Entity;

namespace PleasanterBridge.src.Utilities
{
    internal class TypedDataValidator
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        private static readonly Type[] AllowedTypes =
        {
            typeof(string),
            typeof(bool),
            typeof(double),
            typeof(decimal),
            typeof(int),
            typeof(long),
            typeof(DateTime),
            typeof(ImageEntity)
        };

        public static void Validate(Dictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                if (kvp.Value == null) continue;
                Type valueType = kvp.Value.GetType();

                if (!IsAllowedType(valueType))
                {
                    throw new ArgumentException(
                        $"unsupported type '{kvp.Key}' : {valueType.Name}"
                    );
                }
            }
        }

        public static void ValidateList(List<Dictionary<string, object>> dataList)
        {
            foreach (var data in dataList)
            {
                Validate(data);
            }
        }

        private static bool IsAllowedType(Type type)
        {
            // 基本型のチェック
            if (AllowedTypes.Contains(type))
            {
                return true;
            }

            // Nullable型のチェック
            Type? underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return AllowedTypes.Contains(underlyingType);
            }

            // Dictionary<string, ImageEntity>のチェック
            if (type.IsGenericType)
            {
                Type genericTypeDef = type.GetGenericTypeDefinition();
                if (genericTypeDef == typeof(Dictionary<,>))
                {
                    Type[] genericArgs = type.GetGenericArguments();
                    if (genericArgs.Length == 2 &&
                        genericArgs[0] == typeof(string) &&
                        genericArgs[1] == typeof(ImageEntity))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

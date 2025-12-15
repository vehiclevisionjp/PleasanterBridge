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
            typeof(ImageEntity)
        };

        public static void Validate(Dictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                if (kvp.Value == null) continue;
                Type valueType = kvp.Value.GetType();
                if (!AllowedTypes.Contains(valueType))
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
    }
}

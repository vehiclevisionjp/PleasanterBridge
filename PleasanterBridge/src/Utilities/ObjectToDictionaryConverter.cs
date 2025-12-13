using System.Reflection;

namespace PleasanterBridge.src.Utilities
{
    internal static class ObjectToDictionaryConverter
    {
        public static Dictionary<string, object> Convert<T>(T obj) where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var dictionary = new Dictionary<string, object>();
            
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
   
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
            
                // null値は除外
                if (value == null) continue;
     
                // 空文字列は除外
                if (value is string str && string.IsNullOrEmpty(str)) continue;
       
                dictionary[property.Name] = value;
            }
            
            return dictionary;
        }

        public static List<Dictionary<string, object>> ConvertList<T>(IEnumerable<T> objects) where T : class
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));

            return objects.Select(Convert).ToList();
        }
    }
}

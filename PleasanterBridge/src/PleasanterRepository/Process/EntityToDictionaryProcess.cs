namespace PleasanterBridge.src.PleasanterRepository.Process
{
    public static class EntityToDictionaryProcess
    {
        public static Dictionary<string, object> Convert(object entity)
        {
            return entity.GetType()
                    .GetProperties()
                    .Where(prop => prop.GetValue(entity) != null)
                    .ToDictionary(
                        prop => prop.Name,
                        prop => prop.GetValue(entity)!
                    );
        }
        public static List<Dictionary<string, object>> Convert<T>(List<T> entities)
        {
            return entities
                .Where(entity => entity != null)
                .Select(entity => entity!.GetType()
                    .GetProperties()
                    .Where(prop => prop.GetValue(entity) != null)
                    .ToDictionary(
                        prop => prop.Name,
                        prop => prop.GetValue(entity)!
                    )
                )
                .ToList();
        }
    }
}

using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess.Implementaion;
using PleasanterBridge.src.DataRepository.Utilities;
using PleasanterBridge.src.Utilities;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    internal class GetDbBaseProcess<TEntity> : DbBaseProcess<TEntity>, IDbExecutionProcess
        where TEntity : class
    {
        private readonly PleasanterIdPropertyName _idPropertyName;
        private readonly BinaryImageContentParser _binaryImageContentParser;

        public GetDbBaseProcess(IConnectionSettings settings, PleasanterIdPropertyName idPropertyName)
            : base(settings)
        {
            Entities = Set<TEntity>();
            _idPropertyName = idPropertyName;
            _binaryImageContentParser = new BinaryImageContentParser(settings);
        }

        public List<Dictionary<string, object>> Execute(
            int id,
            bool isReferenceId,
            Dictionary<string, object>? keyValue = null
        )
        {
            List<Dictionary<string, object>>? entities = null;

            if (isReferenceId)
            {
                TEntity record = GetSingleRecord(id, _idPropertyName.ToString());
                if (record == null)
                {
                    throw new Exception("data not found.");
                }
                entities = new List<Dictionary<string, object>> { EntityToDictionaryProcess.Convert(record) };
            }
            else
            {
                List<TEntity>? entityList = null;

                if (keyValue != null)
                {
                    entityList = GetFromDictionary(keyValue);
                }
                else
                {
                    entityList = GetFromSiteId(id);
                }

                if(entityList == null || entityList.Count == 0)
                {
                    throw new Exception("data not found.");
                }

                entities = entityList.Select(EntityToDictionaryProcess.Convert).ToList();
            }

            if (entities == null)
            {
                throw new Exception("data not found.");
            }

            return BinarySetup(entities);
        }

        private List<Dictionary<string, object>> BinarySetup(List<Dictionary<string, object>> entities)
        {
            entities.ForEach(entity =>
            {
                // Body, Description, AttachmentsÇ™ÉLÅ[Ç…ä‹Ç‹ÇÍÇÈèÍçá
                // Ç∆ÇËÇ†Ç¶Ç∏AttachmentÇÕñ≥éã
                if (entity.ContainsKey("Body"))
                {
                    entity["Body"] = _binaryImageContentParser.ParseContentWithBinaryReferences(entity["Body"]?.ToString() ?? string.Empty);
                }

                // DescriptionÇÃèàóù
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    string descriptionKey = $"Description{c}";
                    if (entity.ContainsKey(descriptionKey))
                    {
                        entity[descriptionKey] = _binaryImageContentParser.ParseContentWithBinaryReferences(entity[descriptionKey]?.ToString() ?? string.Empty);
                    }
                }
            });

            return entities;
        }
    }
}

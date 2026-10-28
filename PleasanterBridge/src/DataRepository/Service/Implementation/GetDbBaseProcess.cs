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
            long id,
            bool isReferenceId,
            Dictionary<string, object>? keyValue = null
        )
        {
            List<Dictionary<string, object>>? entities = null;

            if (isReferenceId)
            {
                try
                {
                    TEntity record = GetSingleRecord(id, _idPropertyName.ToString());
                    if (record == null)
                    {
                        return new List<Dictionary<string, object>>();
                    }
                    entities = new List<Dictionary<string, object>> { EntityToDictionaryProcess.Convert(record) };
                }
                catch (Exception)
                {
                    return new List<Dictionary<string, object>>();
                }
            }
            else
            {
                List<TEntity>? entityList = null;

                if (keyValue != null)
                {
                    // keyValueにSiteIdを追加（既に存在する場合は上書きしない）
                    Dictionary<string, object> keyValueWithSiteId = new Dictionary<string, object>(keyValue);
                    if (!keyValueWithSiteId.ContainsKey("SiteId"))
                    {
                        keyValueWithSiteId["SiteId"] = id;
                    }

                    entityList = GetFromDictionary(keyValueWithSiteId);
                }
                else
                {
                    entityList = GetFromSiteId(id);
                }

                if(entityList == null || entityList.Count == 0)
                {
                    return new List<Dictionary<string, object>>();
                }

                entities = entityList.Select(EntityToDictionaryProcess.Convert).ToList();
            }

            if (entities == null)
            {
                return new List<Dictionary<string, object>>();
            }

            entities = BinarySetup(entities);
            entities = NormalizeReferenceId(entities);
            entities = SetItemTitles(entities);

            return entities;
        }

        private List<Dictionary<string, object>> SetItemTitles(List<Dictionary<string, object>> entities)
        {
            List<long> referenceIds = entities
                .Where(entity => entity.TryGetValue("ReferenceId", out object? value) && value != null)
                .Select(entity => Convert.ToInt64(entity["ReferenceId"]))
                .Distinct()
                .ToList();

            Dictionary<long, string?> itemTitles = Items
                .Where(item => referenceIds.Contains(item.ReferenceId))
                .ToDictionary(item => item.ReferenceId, item => item.Title);

            entities.ForEach(entity =>
            {
                entity.Remove("Title");

                if (entity.TryGetValue("ReferenceId", out object? value) &&
                    value != null &&
                    itemTitles.TryGetValue(Convert.ToInt64(value), out string? title) &&
                    title != null)
                {
                    entity["Title"] = title;
                }
            });

            return entities;
        }

        private List<Dictionary<string, object>> BinarySetup(List<Dictionary<string, object>> entities)
        {
            entities.ForEach(entity =>
            {
                // Body, Description, Attachmentsがキーに含まれる場合
                // とりあえずAttachmentは無視
                if (entity.ContainsKey("Body"))
                {
                    var bodyContent = _binaryImageContentParser.ParseContentWithBinaryReferences(entity["Body"]?.ToString() ?? string.Empty);
                    entity["Body"] = bodyContent.Count > 0 ? bodyContent : new List<string> { "" };
                }

                // Descriptionの処理
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    string descriptionKey = $"Description{c}";
                    if (entity.ContainsKey(descriptionKey))
                    {
                        var descriptionContent = _binaryImageContentParser.ParseContentWithBinaryReferences(entity[descriptionKey]?.ToString() ?? string.Empty);
                        entity[descriptionKey] = descriptionContent.Count > 0 ? descriptionContent : new List<string> { "" };
                    }
                }
            });

            return entities;
        }

        private List<Dictionary<string, object>> NormalizeReferenceId(List<Dictionary<string, object>> entities)
        {
            string originalIdPropertyName = _idPropertyName.ToString();

            entities.ForEach(entity =>
            {
                if (entity.ContainsKey(originalIdPropertyName))
                {
                    // 元のIDプロパティの値をReferenceIdに設定
                    entity["ReferenceId"] = entity[originalIdPropertyName];
                    // 元のIDプロパティを削除
                    entity.Remove(originalIdPropertyName);
                }
            });

            return entities;
        }
    }
}

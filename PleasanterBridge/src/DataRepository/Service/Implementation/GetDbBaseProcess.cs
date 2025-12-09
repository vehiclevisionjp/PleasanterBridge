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

        public GetDbBaseProcess(IConnectionSettings settings, PleasanterIdPropertyName idPropertyName)
            : base(settings)
        {
            Entities = Set<TEntity>();
            _idPropertyName = idPropertyName;
        }

        public List<Dictionary<string, object>> Execute(
            string id,
            bool isReferenceId,
            Dictionary<string, object>? keyValue = null
        )
        {
            List<Dictionary<string, object>>? entity = null;

            if (isReferenceId)
            {
                TEntity record = GetSingleRecord(id, _idPropertyName.ToString());
                if (record == null)
                {
                    throw new Exception("data not found.");
                }
                entity = new List<Dictionary<string, object>> { EntityToDictionaryProcess.Convert(record) };
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

                entity = entityList.Select(EntityToDictionaryProcess.Convert).ToList();
            }

            if (entity == null)
            {
                throw new Exception("data not found.");
            }

            return entity;
        }
    }
}

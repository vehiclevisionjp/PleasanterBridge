using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess.Implementaion;
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    internal class GetItemsProcess : ItemsProcess, IGetItemsProcess
    {
        public GetItemsProcess(IConnectionSettings settings)
            : base(settings)
        {
        }

        public string GetReferenceType(long referenceId)
        {
            ItemsEntity? itemsEntity = Get(referenceId);

            if(itemsEntity?.ReferenceType == null)
            {
                // ReferenceIdが見つからない場合は空文字列を返す
                return string.Empty;
            }

            return itemsEntity.ReferenceType;
        }
    }
}

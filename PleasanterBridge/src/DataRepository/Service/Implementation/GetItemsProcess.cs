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

        public string GetReferenceType(int referenceId)
        {
            ItemsEntity itemsEntity = Get(referenceId);

            if(itemsEntity?.ReferenceType == null)
            {
                throw new Exception("ReferenceId not found.");
            }

            return itemsEntity.ReferenceType;
        }
    }
}

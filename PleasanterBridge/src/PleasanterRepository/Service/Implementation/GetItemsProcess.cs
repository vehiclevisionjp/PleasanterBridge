using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;
using PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess;

namespace PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation
{
    public class GetItemsProcess : ItemsProcess, IGetItemsProcess
    {
        public GetItemsProcess(IConnectionSettings settings)
            : base(settings)
        {
        }

        public string GetReferenceType(string referenceId)
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

using Microsoft.EntityFrameworkCore;
using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Data;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;

namespace PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess
{
    public class ItemsProcess : DbContext, IDBContext
    {
        protected readonly IConnectionSettings Settings;
        protected DbSet<ItemsEntity> Items { get; set; }

        protected ItemsProcess(IConnectionSettings settings)
        {
            Settings = settings;
        }

        protected List<ItemsEntity> Get()
        {
            return Items.ToList();
        }

        protected ItemsEntity Get(string referenceId)
        {
            return Items
                .Where(x => Convert.ToInt32(x.ReferenceId) == Convert.ToInt32(referenceId))
                .FirstOrDefault() ?? throw new Exception("invalid resultId");
        }
    }
}
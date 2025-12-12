using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Data;
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    internal class ItemsProcess : DBContext
    {
        protected readonly IConnectionSettings Settings;
        protected DbSet<ItemsEntity> Items { get; set; }

        protected ItemsProcess(IConnectionSettings settings): base(settings)
        {
            Settings = settings;
        }

        protected List<ItemsEntity> Get()
        {
            return Items.ToList();
        }

        protected ItemsEntity Get(int referenceId)
        {
            return Items
                .Where(x => Convert.ToInt32(x.ReferenceId) == Convert.ToInt32(referenceId))
                .FirstOrDefault() ?? throw new Exception("invalid resultId");
        }
    }
}
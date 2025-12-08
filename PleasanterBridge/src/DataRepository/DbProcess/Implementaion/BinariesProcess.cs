using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.DataRepository.Data;
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    public class BinariesProcess : DbContext, IDBContext
    {
        protected DbSet<BinariesEntity> Binaries { get; set; }

        protected BinariesProcess() { }

        protected List<BinariesEntity> Get()
        {
            return Binaries.ToList();
        }

        protected byte[]? GetBin(string guid)
        {
            return Binaries
                .Where(x => x.Guid == guid)
                .Select(x => x.Bin)
                .FirstOrDefault();
        }
    }
}
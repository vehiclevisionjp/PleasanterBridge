using offlineMeeting.Models.DBProperty;
using offlineMeeting.Models.Process.Share;
using offlineMeeting.Models.Share;
using Microsoft.EntityFrameworkCore;

namespace PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess
{
    public class BinariesProcess : DBContext
    {
        protected DbSet<BinariesEntity> Binaries { get; set; }

        protected BinariesProcess() { }

        protected List<BinariesEntity> Get()
        {
            return Binaries.ToList();
        }

        protected byte[]? GetBin(string guid)
        {
            var aaaa = Binaries.Where(x => x.Guid == guid);
            var bbbb = aaaa.Select(x => x.Bin);
            var cccc = bbbb.FirstOrDefault();

            return Binaries
                .Where(x => x.Guid == guid)
                .Select(x => x.Bin)
                .FirstOrDefault();
        }
    }
}
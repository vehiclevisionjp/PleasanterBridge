using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.DbProcess.Implementaion;

namespace PleasanterBridge.src.DataRepository.Service.Implementation
{
    internal class GetBinariesProcess : BinariesProcess, IGetBinariesProcess
    {
        public GetBinariesProcess(IConnectionSettings settings)
            : base(settings)
        {
        }

        public string GetImage(string guid)
        {
            byte[]? bin = GetBin(guid);

            if (bin == null)
            {
                return string.Empty;
            }

            return Convert.ToBase64String(bin);
        }
    }
}

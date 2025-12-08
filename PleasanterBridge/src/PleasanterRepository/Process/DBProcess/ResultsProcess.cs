using Microsoft.EntityFrameworkCore;
using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Data;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;

namespace PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess
{
    public class ResultsProcess : DbContext, IDBContext
    {
        protected readonly IConnectionSettings Settings;

        protected DbSet<ResultsEntity> Results { get; set; }

        protected ResultsProcess(IConnectionSettings settings)
        {
            Settings = settings;
        }
        protected List<ResultsEntity> GetAll()
        {
            return Results.ToList();
        }

        protected List<ResultsEntity> GetFromSiteId(int siteId)
        {
            return Results
                .Where(x => Convert.ToInt32(x.SiteId) == siteId).ToList();
        }

        protected List<ResultsEntity> GetFromSiteId(int siteId, List<int> resultIdList)
        {
            return Results
                .Where(x => Convert.ToInt32(x.SiteId) == siteId)
                .Where(y => resultIdList.Contains(Convert.ToInt32(y.ResultId)))
                .ToList();
        }

        protected ResultsEntity GetSingleRecord(string resultId)
        {
            return Results
                .Where(x => Convert.ToInt32(x.ResultId) == Convert.ToInt32(resultId))
                .FirstOrDefault() ?? throw new Exception("invalid resultId");
        }

        protected List<ResultsEntity> GetFromResultIdList(List<int> resultIdList)
        {
            return Results
                .Where(y => resultIdList.Contains(Convert.ToInt32(y.ResultId)))
                .ToList();
        }

        protected List<ResultsEntity> GetFromDictionaryt(Dictionary<string, object> keyValue)
        {
            var query = WhereByDictionaryProcess.Execute(Results, keyValue);
            return query.ToList();
        }
    }
}
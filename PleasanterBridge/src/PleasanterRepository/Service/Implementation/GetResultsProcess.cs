using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;
using PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess;

namespace PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation
{
    public class GetResultsProcess : ResultsProcess, IGetResultsProcess
    {
        public GetResultsProcess(IConnectionSettings settings)
            : base(settings)
        {
        }

        public Dictionary<string, object> Execute(string resultId)
        {
            ResultsEntity resultsEntity = GetSingleRecord(resultId);

            if(resultsEntity == null)
            {
                throw new Exception("resultsData not found.");
            }

            // データのない行を削る
            // 最終的にはSitesと突き合せてユーザ設定のあるColumnだけ残す

            return resultsEntity.GetType()
                .GetProperties()
                .Where(prop => prop.GetValue(resultsEntity) != null)
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(resultsEntity)!
                );
        }
    }
}

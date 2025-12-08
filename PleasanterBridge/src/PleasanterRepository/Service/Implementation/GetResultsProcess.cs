using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Entity;
using PleasanterAPIBridge.src.PleasanterRepository.Process.DBProcess;
using PleasanterBridge.src.PleasanterRepository.Process;

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

            return EntityToDictionaryProcess.Convert(resultsEntity);
        }

        public List<Dictionary<string, object>> Execute(Dictionary<string, object> keyValue)
        {
            List<ResultsEntity> resultsEntities = base.GetFromDictionaryt(keyValue);

            return EntityToDictionaryProcess.Convert(resultsEntities);
        }
    }
}

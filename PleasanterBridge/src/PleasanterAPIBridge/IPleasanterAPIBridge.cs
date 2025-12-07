using PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts;

namespace PleasanterAPIBridge.src.PleasanterAPIBridge
{
    public interface IPleasanterApiBridge
    {
        public Task<PleasanterApiResponse> Insert(Object data, string siteId);
        public Task<PleasanterApiResponse> BulkInsert<T>(IEnumerable<T> dataList, string siteId);
        public Task<PleasanterApiResponse> Update<T>(Dictionary<string, T> data, string resultId);
        //public Task<PleasanterApiResponseEntity> Update<T>(Object data, string siteId, Dictionary<string, T> keyValuePair);
        public Task<List<PleasanterApiResponse>> BulkUpdate<T>(Dictionary<string, T> dataDic, string keyColumn = "ResultId");
    }
}
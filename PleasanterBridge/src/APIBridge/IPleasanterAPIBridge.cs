using PleasanterBridge.src.APIBridge.Contracts;

namespace PleasanterBridge.src.APIBridge
{
    public interface IPleasanterApiBridge
    {
        public Task<PleasanterApiResponse> Insert(string siteId, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkInsert(string siteId, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Update(string referenceId, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> Update(string siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> BulkUpdate(Dictionary<string, object> updateData/*, string keyColumn = "ResultId"*/);
        public Task<PleasanterApiResponse> Upsert(string siteId, List<string> matchColumns, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkUpsert(string siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Delete(string referenceId);
        public Task<List<PleasanterApiResponse>> Delete(string siteId, Dictionary<string, object> targetColumns);
    }
}
using PleasanterBridge.src.APIBridge.Contracts;

namespace PleasanterBridge.src.APIBridge
{
    public interface IPleasanterApiBridge
    {
        // Dictionary版メソッド（既存）
        public Task<PleasanterApiResponse> Insert(string siteId, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkInsert(string siteId, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Update(string referenceId, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> Update(string siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> BulkUpdate(Dictionary<string, object> updateData/*, string keyColumn = "ResultId"*/);
        public Task<PleasanterApiResponse> Upsert(string siteId, List<string> matchColumns, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkUpsert(string siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Delete(string referenceId);
        public Task<List<PleasanterApiResponse>> Delete(string siteId, Dictionary<string, object> targetColumns);

        // ジェネリック版メソッド（新規）
        public Task<PleasanterApiResponse> Insert<T>(string siteId, T dataRecord) where T : class;
        public Task<PleasanterApiResponse> BulkInsert<T>(string siteId, List<T> dataRecords) where T : class;
        public Task<PleasanterApiResponse> Update<T>(string referenceId, T updateData) where T : class;
        public Task<List<PleasanterApiResponse>> Update<TTarget, TUpdate>(string siteId, TTarget targetColumns, TUpdate updateData)
            where TTarget : class
            where TUpdate : class;
        public Task<PleasanterApiResponse> Upsert<T>(string siteId, List<string> matchColumns, T dataRecord) where T : class;
        public Task<PleasanterApiResponse> BulkUpsert<T>(string siteId, List<string> matchColumns, List<T> dataRecords) where T : class;
        public Task<List<PleasanterApiResponse>> Delete<T>(string siteId, T targetColumns) where T : class;
    }
}
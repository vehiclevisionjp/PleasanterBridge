using PleasanterBridge.src.APIBridge.Contracts;

namespace PleasanterBridge.src.APIBridge
{
    public interface IPleasanterApiBridge
    {
        // Dictionary版メソッド（既存）
        public Task<PleasanterApiResponse> Insert(long siteId, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkInsert(long siteId, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Update(long referenceId, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> Update(long siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> BulkUpdate(Dictionary<string, object> updateData/*, string keyColumn = "ResultId"*/);
        public Task<PleasanterApiResponse> Upsert(long siteId, List<string> matchColumns, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkUpsert(long siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Delete(long referenceId);
        public Task<List<PleasanterApiResponse>> Delete(long siteId, Dictionary<string, object> targetColumns);

        // ジェネリック版メソッド（新規）
        public Task<PleasanterApiResponse> Insert<T>(long siteId, T dataRecord) where T : class;
        public Task<PleasanterApiResponse> BulkInsert<T>(long siteId, List<T> dataRecords) where T : class;
        public Task<PleasanterApiResponse> Update<T>(long referenceId, T updateData) where T : class;
        public Task<List<PleasanterApiResponse>> Update<TTarget, TUpdate>(long siteId, TTarget targetColumns, TUpdate updateData)
            where TTarget : class
            where TUpdate : class;
        public Task<PleasanterApiResponse> Upsert<T>(long siteId, List<string> matchColumns, T dataRecord) where T : class;
        public Task<PleasanterApiResponse> BulkUpsert<T>(long siteId, List<string> matchColumns, List<T> dataRecords) where T : class;
        public Task<List<PleasanterApiResponse>> Delete<T>(long siteId, T targetColumns) where T : class;
    }
}
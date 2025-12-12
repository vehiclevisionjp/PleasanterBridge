using PleasanterBridge.src.APIBridge.Contracts;

namespace PleasanterBridge.src.APIBridge
{
    public interface IPleasanterApiBridge
    {
        // Dictionary版メソッド（既存）
        public Task<PleasanterApiResponse> Insert(int siteId, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkInsert(int siteId, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Update(int referenceId, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> Update(int siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData);
        public Task<List<PleasanterApiResponse>> BulkUpdate(Dictionary<string, object> updateData/*, string keyColumn = "ResultId"*/);
        public Task<PleasanterApiResponse> Upsert(int siteId, List<string> matchColumns, Dictionary<string, object> dataRecord);
        public Task<PleasanterApiResponse> BulkUpsert(int siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords);
        public Task<PleasanterApiResponse> Delete(int referenceId);
        public Task<List<PleasanterApiResponse>> Delete(int siteId, Dictionary<string, object> targetColumns);

        // ジェネリック版メソッド（新規）
        public Task<PleasanterApiResponse> Insert<T>(int siteId, T dataRecord) where T : class;
        public Task<PleasanterApiResponse> BulkInsert<T>(int siteId, List<T> dataRecords) where T : class;
        public Task<PleasanterApiResponse> Update<T>(int referenceId, T updateData) where T : class;
        public Task<List<PleasanterApiResponse>> Update<TTarget, TUpdate>(int siteId, TTarget targetColumns, TUpdate updateData)
            where TTarget : class
            where TUpdate : class;
        public Task<PleasanterApiResponse> Upsert<T>(int siteId, List<string> matchColumns, T dataRecord) where T : class;
        public Task<PleasanterApiResponse> BulkUpsert<T>(int siteId, List<string> matchColumns, List<T> dataRecords) where T : class;
        public Task<List<PleasanterApiResponse>> Delete<T>(int siteId, T targetColumns) where T : class;
    }
}
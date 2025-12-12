using PleasanterBridge.src.APIBridge.Configuration;
using PleasanterBridge.src.APIBridge.Contracts;
using PleasanterBridge.src.APIBridge.Exception;
using PleasanterBridge.src.APIBridge.Mapping;
using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.DataRepository.Service.Implementation;
using PleasanterBridge.src.Utilities;
using System.Data;

namespace PleasanterBridge.src.APIBridge
{
    public class PleasanterAPIBridge : IPleasanterApiBridge
    {
        private readonly HttpClient _httpClient;
        private readonly IPleasanterSettings _settings;
        private readonly IPleasanterUrlProvider _pleasanterUrlProvider;
        private readonly IPleasanterRepository _pleasanterRepository;
        private readonly IGetReferenceProcess _getReferenceProcess;
        private readonly IPleasanterResponseHandler _pleasanterResponseHandler; 


        public PleasanterAPIBridge(
            HttpClient httpClient, 
            string pleasanterUrl, 
            string apiKey,
            string server,
            string database,
            string userId,
            string password,
            src.Configuration.DatabaseType databaseType = src.Configuration.DatabaseType.SqlServer
        )
        {
            if (string.IsNullOrEmpty(pleasanterUrl))
            {
                throw new ArgumentException("Base URL is required.");
            }

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = new PleasanterSettings(
                apiKey,
                pleasanterUrl,
                server,
                database,
                userId,
                password,
                databaseType
            );
            _pleasanterUrlProvider = new PleasanterUrlProvider(pleasanterUrl);
            _pleasanterRepository = new PleasanterRepository(_settings);
            _getReferenceProcess = new GetReferenceProcess(_settings);
            _pleasanterResponseHandler = new PleasanterResponseHandler(httpClient);
        }

        public PleasanterAPIBridge(
            HttpClient httpClient,
            IPleasanterSettings settings
        ) : this(
            httpClient,
            settings.Url,
            settings.ApiyKey,
            settings.Server,
            settings.Database,
            settings.UserId,
            settings.Password,
            settings.DatabaseType
        )
        {
        }

        public PleasanterAPIBridge(
            HttpClient httpClient,
            IPleasanterSettings settings,
            IPleasanterUrlProvider urlProvider,
            IPleasanterRepository repository,
            IGetReferenceProcess referenceProcess,
            IPleasanterResponseHandler responseHandler
        )
        {
            _httpClient = httpClient;
            _settings = settings;
            _pleasanterUrlProvider = urlProvider;
            _pleasanterRepository = repository;
            _getReferenceProcess = referenceProcess;
            _pleasanterResponseHandler = responseHandler;
        }

        // Dictionary版メソッド（既存）
        public async Task<PleasanterApiResponse> Insert(int siteId, Dictionary<string, object> dataRecord)
        {
            TypedDataValidator.Validate(dataRecord);
            string url = _pleasanterUrlProvider.GetItemCreateUrl(siteId);
            return await _pleasanterResponseHandler.SendPost(SetRequest(dataRecord), url);
        }

        public async Task<PleasanterApiResponse> BulkInsert(int siteId, List<Dictionary<string, object>> dataRecords)
        {
            TypedDataValidator.ValidateList(dataRecords);
            string url = _pleasanterUrlProvider.GetBulkUpsertUrl(siteId);
            return await _pleasanterResponseHandler.SendPost(setBulkInsertRequest(dataRecords), url);
        }

        public async Task<PleasanterApiResponse> Update(int referenceId, Dictionary<string, object> updateData)
        {
            TypedDataValidator.Validate(updateData);
            return await DoUpdate(referenceId, updateData);
        }

        public async Task<List<PleasanterApiResponse>> Update(int siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData)
        {
            TypedDataValidator.Validate(targetColumns);
            TypedDataValidator.Validate(updateData);
            List<int> recordIds = GetTargetRecordIds(siteId, targetColumns);

            List<Task<PleasanterApiResponse>> tasks = recordIds.Select(resultId => DoUpdate(resultId, updateData)).ToList();

            return (await Task.WhenAll(tasks)).ToList();
        }

        public Task<List<PleasanterApiResponse>> BulkUpdate(Dictionary<string, object> updateData/*, string keyColumn = "ResultId"*/)
        {
            // Updateが実質SQLにおけるBulkupdateを実装してるので
            // 明示的にBulkupdateを使う場合の処理については別途考える
            // bulkupsertは存在するのだが、PleasanterのBulkupdateはId指定して更新などはできないため少し違う
            /*
            List<Task<PleasanterApiResponse>> tasks = dataDic.Select(data => Update(data.Value!, data.Key)).ToList();

            List<PleasanterApiResponse> results = (await Task.WhenAll(tasks)).ToList();

            return results;
            */
            // keyなしBulkUpsertは必ずBulkupdateにはなるんだが…
            // どうすっかなあ
            TypedDataValidator.Validate(updateData);
            throw new NotImplementedException("BulkUpdate is not implemented yet.");
        }

        public async Task<PleasanterApiResponse> Upsert(int siteId, List<string> matchColumns, Dictionary<string, object> dataRecord)
        {
            TypedDataValidator.Validate(dataRecord);
            return await BulkUpsert(siteId, matchColumns, new List<Dictionary<string, object>> { dataRecord });
        }

        public async Task<PleasanterApiResponse> BulkUpsert(int siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords)
        {
            TypedDataValidator.ValidateList(dataRecords);
            string url = _pleasanterUrlProvider.GetBulkUpsertUrl(siteId);

            return await _pleasanterResponseHandler.SendPost(setUpsertRequest(matchColumns, dataRecords), url);
        }

        public async Task<PleasanterApiResponse> Delete(int referenceId)
        {
            return await DoDelete(referenceId);
        }

        public async Task<List<PleasanterApiResponse>> Delete(int siteId, Dictionary<string, object> targetColumns)
        {
            TypedDataValidator.Validate(targetColumns);
            List<int> resultIds = GetTargetRecordIds(siteId, targetColumns);

            List<Task<PleasanterApiResponse>> tasks = resultIds.Select(resultId => DoDelete(resultId)).ToList();

            return (await Task.WhenAll(tasks)).ToList();
        }

        // ジェネリック版メソッド（新規）
        public Task<PleasanterApiResponse> Insert<T>(int siteId, T dataRecord) where T : class
        {
            var dictionary = ObjectToDictionaryConverter.Convert(dataRecord);
            return Insert(siteId, dictionary);
        }

        public Task<PleasanterApiResponse> BulkInsert<T>(int siteId, List<T> dataRecords) where T : class
        {
            var dictionaries = ObjectToDictionaryConverter.ConvertList(dataRecords);
            return BulkInsert(siteId, dictionaries);
        }

        public Task<PleasanterApiResponse> Update<T>(int referenceId, T updateData) where T : class
        {
            var dictionary = ObjectToDictionaryConverter.Convert(updateData);
            return Update(referenceId, dictionary);
        }

        public Task<List<PleasanterApiResponse>> Update<TTarget, TUpdate>(int siteId, TTarget targetColumns, TUpdate updateData) 
            where TTarget : class 
            where TUpdate : class
        {
            var targetDictionary = ObjectToDictionaryConverter.Convert(targetColumns);
            var updateDictionary = ObjectToDictionaryConverter.Convert(updateData);
            return Update(siteId, targetDictionary, updateDictionary);
        }

        public Task<PleasanterApiResponse> Upsert<T>(int siteId, List<string> matchColumns, T dataRecord) where T : class
        {
            var dictionary = ObjectToDictionaryConverter.Convert(dataRecord);
            return Upsert(siteId, matchColumns, dictionary);
        }

        public Task<PleasanterApiResponse> BulkUpsert<T>(int siteId, List<string> matchColumns, List<T> dataRecords) where T : class
        {
            var dictionaries = ObjectToDictionaryConverter.ConvertList(dataRecords);
            return BulkUpsert(siteId, matchColumns, dictionaries);
        }

        public Task<List<PleasanterApiResponse>> Delete<T>(int siteId, T targetColumns) where T : class
        {
            var dictionary = ObjectToDictionaryConverter.Convert(targetColumns);
            return Delete(siteId, dictionary);
        }

        private List<int> GetTargetRecordIds(int siteId, Dictionary<string, object> targetColumns)
        {
            string referenceType = _getReferenceProcess.Execute(siteId, false);
            PleasanterReferenceType pleasanterReferenceType = PleasanterReferenceTypeExtensions.ParseReferenceType(referenceType);

            string idName = pleasanterReferenceType switch
            {
                PleasanterReferenceType.Results => PleasanterIdPropertyName.ResultId.ToString(),
                PleasanterReferenceType.Issues => PleasanterIdPropertyName.IssueId.ToString(),
                PleasanterReferenceType.Wikis => PleasanterIdPropertyName.WikiId.ToString(),
                _ => ""
            };

            if (idName == "")
            {
                throw new PleasanterApiException(
                    message: $"Update/Delete by key is not supported for reference type '{referenceType}'.",
                    responseInfo: new PleasanterApiResponse()
                );
            }

            List<Dictionary<string, object>> filteredRecords = _pleasanterRepository.Select(siteId: siteId, keyValue: targetColumns);
            List<int> resultIds = filteredRecords
                .Where(data => data.ContainsKey(idName) && data[idName] != null)
                .Select(data => int.Parse(data[idName].ToString()!))
                .ToList();

            return resultIds;
        }

        private async Task<PleasanterApiResponse> DoUpdate(int recordId, Dictionary<string, object> updateData)
        {
            string url = _pleasanterUrlProvider.GetItemUpdateUrl(recordId);
            return await _pleasanterResponseHandler.SendPost(SetRequest(updateData), url);
        }
        private async Task<PleasanterApiResponse> DoDelete(int recordId)
        {
            string url = _pleasanterUrlProvider.GetItemDeleteUrl(recordId);
            return await _pleasanterResponseHandler.SendPost(SetDeleteRequest(), url);
        }

        private Dictionary<string, object> SetRequest(Dictionary<string, object> dataSet)
        {
            Dictionary<string, object> apiRequest = RequestToDictionaryProcess.Convert(new ApiRequestBase(_settings.ApiyKey));
            Dictionary<string, object> converted = SetEntity(dataSet);

            return DictionaryExtensions.Merge(apiRequest, converted);
        }

        private Dictionary<string, object> setBulkInsertRequest(List<Dictionary<string, object>> dataSets)
        {
            List<Dictionary<string, object>> convertedList = new();

            dataSets.ForEach(dataSet =>
            {
                convertedList.Add(SetEntity(dataSet));
            });

            BulkInsertRequest bulkInsertRequest = new(_settings.ApiyKey, convertedList);

            return RequestToDictionaryProcess.Convert(bulkInsertRequest);
        }

        private Dictionary<string, object> setUpsertRequest(List<string> matchColumns, List<Dictionary<string, object>> dataSets)
        {
            List<Dictionary<string, object>> convertedList = new();

            dataSets.ForEach(dataSet =>
            {
                convertedList.Add(SetEntity(dataSet));
            });

            UpsertApiRequest upsertApiRequest = new(_settings.ApiyKey, matchColumns, convertedList);

            return RequestToDictionaryProcess.Convert(upsertApiRequest);
        }

        private Dictionary<string, object> SetDeleteRequest()
        {
            ApiRequestBase apiRequestBase = new(_settings.ApiyKey);

            return RequestToDictionaryProcess.Convert(apiRequestBase);
        }

        private Dictionary<string, object> SetEntity(Dictionary<string, object> dataSet)
        {
            string? title = dataSet.TryGetValue(PleasanterApiRequestKey.Title.ToKeyString(), out var t) ? t as string : null;
            string? body = dataSet.TryGetValue(PleasanterApiRequestKey.Body.ToKeyString(), out var b) ? b as string : null;
            string? startTime = dataSet.TryGetValue(PleasanterApiRequestKey.StartTime.ToKeyString(), out var st) ? st as string : null;
            string? completionTime = dataSet.TryGetValue(PleasanterApiRequestKey.CompletionTime.ToKeyString(), out var ct) ? ct as string : null;
            decimal? workValue = dataSet.TryGetValue(PleasanterApiRequestKey.WorkValue.ToKeyString(), out var wv) && wv is decimal d1 ? d1 : null;
            decimal? progressRate = dataSet.TryGetValue(PleasanterApiRequestKey.ProgressRate.ToKeyString(), out var pr) && pr is decimal d2 ? d2 : null;

            PleasanterApiRequest pleasanterApiRequest = (PleasanterApiRequest)Activator.CreateInstance(
                typeof(PleasanterApiRequest),
                title, body, startTime, completionTime, workValue, progressRate
            )!;

            pleasanterApiRequest.ClassHash = dataSet
                .Where(data => data.Key.Contains(PleasanterApiRequestKey.Class.ToHashPrefix()) && data.Value is string)
                .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
            pleasanterApiRequest.NumHash = dataSet
                .Where(data =>
                    data.Key.Contains(PleasanterApiRequestKey.Num.ToHashPrefix()) &&
                    (data.Value is decimal || data.Value is int || data.Value is long || data.Value is float || data.Value is double))
                .ToDictionary(
                    data => data.Key,
                    data => Convert.ToDecimal(data.Value)
                );
            pleasanterApiRequest.DateHash = dataSet
                .Where(data => data.Key.Contains(PleasanterApiRequestKey.Date.ToHashPrefix()) && data.Value is string)
                .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
            pleasanterApiRequest.DescriptionHash = dataSet
                .Where(data => data.Key.Contains(PleasanterApiRequestKey.Description.ToHashPrefix()) && data.Value is string)
                .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
            pleasanterApiRequest.CheckHash = dataSet
                .Where(data => data.Key.Contains(PleasanterApiRequestKey.Check.ToHashPrefix()) && data.Value is bool)
                .ToDictionary(data => data.Key, data => data.Value is bool b ? b : false);
            /*
                pleasanterApiRequest.ImageHash = dataSet
             */

            // fixme hashが一つもなかった場合どうするかは検証の必要アリ
            return RequestToDictionaryProcess.Convert(pleasanterApiRequest);
        }
    }
}

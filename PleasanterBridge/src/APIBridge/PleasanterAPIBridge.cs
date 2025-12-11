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

        public async Task<PleasanterApiResponse> Insert(string siteId, Dictionary<string, object> dataRecord)
        {
            TypedDataValidator.Validate(dataRecord);
            string url = _pleasanterUrlProvider.GetItemCreateUrl(siteId);
            return await _pleasanterResponseHandler.SendPost(SetRequest(dataRecord), url);
        }

        public async Task<PleasanterApiResponse> BulkInsert(string siteId, List<Dictionary<string, object>> dataRecords)
        {
            TypedDataValidator.ValidateList(dataRecords);
            string url = _pleasanterUrlProvider.GetBulkUpsertUrl(siteId);
            return await _pleasanterResponseHandler.SendPost(setBulkInsertRequest(dataRecords), url);
        }

        public async Task<PleasanterApiResponse> Update(string referenceId, Dictionary<string, object> updateData)
        {
            TypedDataValidator.Validate(updateData);
            return await DoUpdate(referenceId, updateData);
        }

        public async Task<List<PleasanterApiResponse>> Update(string siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData)
        {
            TypedDataValidator.Validate(targetColumns);
            TypedDataValidator.Validate(updateData);
            List<string> recordIds = GetTargetRecordIds(siteId, targetColumns);

            List<Task<PleasanterApiResponse>> tasks = recordIds.Select(resultId => DoUpdate(resultId, updateData)).ToList();

            return (await Task.WhenAll(tasks)).ToList();
        }

        public Task<List<PleasanterApiResponse>> BulkUpdate(Dictionary<string, object> updateData/*, string keyColumn = "ResultId"*/)
        {
            TypedDataValidator.Validate(updateData);
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
            throw new NotImplementedException("BulkUpdate is not implemented yet.");
        }

        public async Task<PleasanterApiResponse> Upsert(string siteId, List<string> matchColumns, Dictionary<string, object> dataRecord)
        {
            TypedDataValidator.Validate(dataRecord);
            return await BulkUpsert(siteId, matchColumns, new List<Dictionary<string, object>> { dataRecord });
        }

        public async Task<PleasanterApiResponse> BulkUpsert(string siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords)
        {
            TypedDataValidator.ValidateList(dataRecords);
            string url = _pleasanterUrlProvider.GetBulkUpsertUrl(siteId);

            return await _pleasanterResponseHandler.SendPost(setUpsertRequest(matchColumns, dataRecords), url);
        }

        public async Task<PleasanterApiResponse> Delete(string referenceId)
        {
            return await DoDelete(referenceId);
        }

        public async Task<List<PleasanterApiResponse>> Delete(string siteId, Dictionary<string, object> targetColumns)
        {
            TypedDataValidator.Validate(targetColumns);
            List<string> resultIds = GetTargetRecordIds(siteId, targetColumns);

            List<Task<PleasanterApiResponse>> tasks = resultIds.Select(resultId => DoDelete(resultId)).ToList();

            return (await Task.WhenAll(tasks)).ToList();
        }

        private List<string> GetTargetRecordIds(string siteId, Dictionary<string, object> targetColumns)
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
            List<string> resultIds = filteredRecords
                .Where(data => data.ContainsKey(idName))
                .Select(data => data[idName]?.ToString() ?? string.Empty)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            return resultIds;
        }

        private async Task<PleasanterApiResponse> DoUpdate(string recordId, Dictionary<string, object> updateData)
        {
            string url = _pleasanterUrlProvider.GetItemUpdateUrl(recordId);
            return await _pleasanterResponseHandler.SendPost(SetRequest(updateData), url);
        }
        private async Task<PleasanterApiResponse> DoDelete(string recordId)
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

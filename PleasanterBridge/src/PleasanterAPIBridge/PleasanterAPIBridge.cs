using PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts;
using PleasanterAPIBridge.src.PleasanterAPIBridge.Exception;
using PleasanterAPIBridge.src.PleasanterRepository.Configuration;
using PleasanterAPIBridge.src.PleasanterRepository.Service;
using PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation;
using System.Data;
using System.Net;
using System.Text.Json;

namespace PleasanterAPIBridge.src.PleasanterAPIBridge
{
    public class PleasanterAPIBridge : IPleasanterApiBridge
    {
        private readonly HttpClient _httpClient;
        private readonly string _pleasanterUrlBase;
        private readonly string _apiKey;
        private readonly IPleasanterRepository PleasanterRepository;
        private readonly IGetReferenceProcess GetReferenceProcess;

        public PleasanterAPIBridge(HttpClient httpClient, string pleasanterUrlBase, string apiKey, IConnectionSettings settings)
        {
            if (string.IsNullOrEmpty(pleasanterUrlBase))
            {
                throw new ArgumentException("Base URL is required.");
            }

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _pleasanterUrlBase = pleasanterUrlBase.TrimEnd('/');
            _apiKey = apiKey;
            PleasanterRepository = new PleasanterRepository.Service.PleasanterRepository(settings);
            GetReferenceProcess = new GetReferenceProcess(settings);
        }

        public async Task<PleasanterApiResponse> Insert(string siteId, Dictionary<string, object> dataRecord)
        {
            StringContent content = MakeJson(SetRequest(dataRecord));
            string url = $"{_pleasanterUrlBase}/api/items/{siteId}/create";

            return await SendPost(url, content);
        }

        public async Task<PleasanterApiResponse> BulkInsert(string siteId, List<Dictionary<string, object>> dataRecords)
        {
            StringContent content = MakeJson(setBulkInsertRequest(dataRecords));
            string url = $"{_pleasanterUrlBase}/api/items/{siteId}/bulkupsert";

            return await SendPost(url, content);
        }

        public async Task<PleasanterApiResponse> Update(string referenceId, Dictionary<string, object> updateData)
        {
            return await DoUpdate(referenceId, updateData);
        }

        public async Task<List<PleasanterApiResponse>> Update(string siteId, Dictionary<string, object> targetColumns, Dictionary<string, object> updateData)
        {
            // 最終、Updateの型を作るにあたって共通でない部分のUpdateを用意しなければならないため
            // 今は不要だがReferenceTypeでの分岐が必須になる
            // BulkUpsert問題はまた考える…
            // でも空だけ用意して更新かけると空が入っちゃうのどうするかなあ
            // 更新ナシ(Keyがない)部分は消したDictionaryにしてからUpdateに流せばいいか
            // 事前にKey用意しとく方が、KeyのTypoによるエラーが防げるし
            string referenceType = GetReferenceProcess.Execute(siteId, false);
            /*
             * いずれReferenceTypeごとの処理をここに書く
             */

            string idName = referenceType switch
            {
                "Results" => "ResultId",
                "Issues" => "IssueId",
                "Wikis" => "WikiId",
                _ => ""
            };

            if(idName == "")
            {
                throw new PleasanterApiException(
                    message: $"Update by key is not supported for reference type '{referenceType}'.",
                    responseInfo: new PleasanterApiResponse()
                );
            }

            // entityFrameworkでIdを取得してリスト化
            List<Dictionary<string, object>> tableData = PleasanterRepository.Select(siteId: siteId, keyValue: targetColumns);
            List<string> resultIds = tableData
                .Where(data => data.ContainsKey(idName))
                .Select(data => data[idName].ToString() ?? string.Empty)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            List<Task<PleasanterApiResponse>> tasks = resultIds.Select(resultId => DoUpdate(resultId, updateData)).ToList();

            List<PleasanterApiResponse> results = (await Task.WhenAll(tasks)).ToList();

            return results;
        }

        // 暫定的にResultIdでしか更新できないようにする
        // ゆくゆくは別のキー対応する
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
            throw new NotImplementedException("BulkUpdate is not implemented yet.");
        }

        public async Task<PleasanterApiResponse> Upsert(string siteId, List<string> matchColumns, Dictionary<string, object> dataRecord)
        {
            return await BulkUpsert(siteId, matchColumns, new List<Dictionary<string, object>> { dataRecord });
        }

        public async Task<PleasanterApiResponse> BulkUpsert(string siteId, List<string> matchColumns, List<Dictionary<string, object>> dataRecords)
        {
            StringContent content = MakeJson(setUpsertRequest(matchColumns, dataRecords));
            string url = $"{_pleasanterUrlBase}/api/items/{siteId}/bulkupsert";

            return await SendPost(url, content);
        }

        private async Task<PleasanterApiResponse> DoUpdate<T>(string recordId, Dictionary<string, T> updateData)
        {
            StringContent content = MakeJson(SetRequest(updateData));
            string url = $"{_pleasanterUrlBase}/api/items/{recordId}/update";

            return await SendPost(url, content);
        }

        private async Task<PleasanterApiResponse> SendPost(string url, StringContent content)
        {
            PleasanterApiResponse responseInfo = new();

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                responseInfo = AddInformation(DeserializeJson(responseBody), url, content);
            }
            catch (HttpRequestException ex)
            {
                responseInfo.Message = $"Infrastructure error: {ex.Message}";
                responseInfo.StatusCode = (int)HttpStatusCode.ServiceUnavailable; // 例外的な状況を示すコード

                throw new PleasanterApiException(
                    message: $"Infrastructure connection error.",
                    responseInfo: responseInfo,
                    innerException: ex
                );
            }
            catch (PleasanterApiException)
            {
                throw;
            }

            return responseInfo;
        }

        private ApiRequest SetRequest<T>(Dictionary<string, T> dataSet)
        {
            PleasanterApiRequest pleasanterApiRequest = SetEntity(dataSet);
            ApiRequest apiRequest = new(_apiKey, pleasanterApiRequest);

            return apiRequest;
        }

        private BulkInsertRequest setBulkInsertRequest<T>(List<Dictionary<string, T>> dataSets)
        {
            List<PleasanterApiRequest> pleasanterApiRequests = new();

            dataSets.ForEach(dataSet =>
            {
                pleasanterApiRequests.Add(SetEntity(dataSet));
            });

            BulkInsertRequest bulkInsertRequest = new(_apiKey, pleasanterApiRequests);

            return bulkInsertRequest;
        }

        private UpsertApiRequest setUpsertRequest<T>(List<string> matchColumns, List<Dictionary<string, T>> dataSets)
        {
            List<PleasanterApiRequest> pleasanterApiRequests = new();

            dataSets.ForEach(dataSet =>
            {
                pleasanterApiRequests.Add(SetEntity(dataSet));
            });

            UpsertApiRequest upsertApiRequest = new(_apiKey, matchColumns, pleasanterApiRequests);

            return upsertApiRequest;
        }

        private PleasanterApiRequest SetEntity<T>(Dictionary<string, T> dataSet)
        {
            PleasanterApiRequest pleasanterApiRequest = new();

            pleasanterApiRequest.ClassHash = dataSet
                .Where(data => data.Key.Contains("Class") && data.Value is string)
                .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
            pleasanterApiRequest.NumHash = dataSet
                .Where(data => data.Key.Contains("Num") && data.Value is decimal)
                .ToDictionary(data => data.Key, data => data.Value is decimal d ? d : 0m);
            pleasanterApiRequest.DateHash = dataSet
                .Where(data => data.Key.Contains("Date") && data.Value is DateTime)
                .ToDictionary(data => data.Key, data => data.Value is DateTime dt ? dt : default(DateTime));
            pleasanterApiRequest.DescriptionHash = dataSet
                .Where(data => data.Key.Contains("Description") && data.Value is string)
                .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
            pleasanterApiRequest.CheckHash = dataSet
                .Where(data => data.Key.Contains("Check") && data.Value is bool)
                .ToDictionary(data => data.Key, data => data.Value is bool b ? b : false);
            /*
                pleasanterApiRequest.AttachmentsHash = dataSet
                    .Where(data => data.Key.Contains("Attachments") && data.Value is string)
                    .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
             */

            return pleasanterApiRequest;
        }

        private StringContent MakeJson(Object data)
        {
            string json = JsonSerializer.Serialize(data);
            return new(json, System.Text.Encoding.UTF8, "application/json");
        }

        private PleasanterApiResponse DeserializeJson(string responseBody)
        {
            return JsonSerializer.Deserialize<PleasanterApiResponse>(responseBody)
                   ?? new PleasanterApiResponse();
        }

        private PleasanterApiResponse AddInformation(
            PleasanterApiResponse pleasanterApiResponseEntity,
            string url,
            StringContent content
        )
        {
            pleasanterApiResponseEntity.Url = url;
            pleasanterApiResponseEntity.SendData = content.ToString();

            return pleasanterApiResponseEntity;
        }
    }
}

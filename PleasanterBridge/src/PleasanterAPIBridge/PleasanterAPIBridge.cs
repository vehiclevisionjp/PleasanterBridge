using PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts;
using PleasanterAPIBridge.src.PleasanterAPIBridge.Exception;
using PleasanterAPIBridge.src.PleasanterAPIBridge.Request;
using System.Net;
using System.Text.Json;

namespace PleasanterAPIBridge.src.PleasanterAPIBridge
{
    public class PleasanterAPIBridge : IPleasanterApiBridge
    {

        private readonly HttpClient _httpClient;
        private readonly string _pleasanterUrlBase;
        private readonly string _apiKey;

        public PleasanterAPIBridge(HttpClient httpClient, string pleasanterUrlBase, string apiKey)
        {
            if (string.IsNullOrEmpty(pleasanterUrlBase))
            {
                throw new ArgumentException("Base URL is required.");
            }

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _pleasanterUrlBase = pleasanterUrlBase.TrimEnd('/');
            _apiKey = apiKey;
        }

        // 複数キーに対応したUpdate
        // 複数キー対応のBulkUpdateはとりあえず対応しない方針
        // ResultId以外の単独キーに対応したものはあとで考える
        // ...KeyNameを取ればいいのか

        public async Task<PleasanterApiResponse> Insert(Object data, string siteId)
        {
            StringContent content = MakeJson(data);
            string url = $"{_pleasanterUrlBase}/api/items/{siteId}/create";

            return await SendPost(url, content);
        }

        public async Task<PleasanterApiResponse> BulkInsert<T>(IEnumerable<T> dataList, string siteId)
        {
            BulkApiRequest sendPlesanterApiEntity = new(data: dataList.Cast<object>().ToList(), _apiKey);
            StringContent content = MakeJson(sendPlesanterApiEntity);
            string url = $"{_pleasanterUrlBase}/api/items/{siteId}/bulkupsert";

            return await SendPost(url, content);
        }
        public async Task<PleasanterApiResponse> Update<T>(string resultId, Dictionary<string, T> updateValues, string siteId) { 

            var updateKeys = new Dictionary<string, T> { { "ResultId", (T)Convert.ChangeType(resultId, typeof(T)) } };
            return await Update<T>(updateKeys: updateKeys, updateValues: updateValues, siteId: siteId);
        }

        public async Task<PleasanterApiResponse> Update<T>(Dictionary<string, T> updateKeys, Dictionary<string, T> updateValues, string siteId)
        {
            // entityFrameworkでデータ取得

            StringContent content = MakeJson(SetEntity(data));
            string url = $"{_pleasanterUrlBase}/api/items/{resultId}/update";

            return await SendPost(url, content);
        }

        // siteId必須
        private async Task<PleasanterApiResponse> Update<T>(string resultId, Dictionary<string, T> updateValues)
        {
            StringContent content = MakeJson(SetEntity(updateValues));
            string url = $"{_pleasanterUrlBase}/api/items/{resultId}/update";

            return await SendPost(url, content);
        }

        // 暫定的にResultIdでしか更新できないようにする
        // ゆくゆくは別のキー対応する
        public async Task<List<PleasanterApiResponse>> BulkUpdate<T>(Dictionary<string, T> dataDic/*, string keyColumn = "ResultId"*/)
        {
            // 各UpdateのTaskを作成
            // ここでTask作らなくてよくなる
            // シンプルに、Updateに流す内容がListになってる状態にすればOK
            List<Task<PleasanterApiResponse>> tasks = dataDic.Select(data => Update(data.Value!, data.Key)).ToList();

            List<PleasanterApiResponse> results = (await Task.WhenAll(tasks)).ToList();

            return results;
        }

        // upsert

        // bulkupsert

        private StringContent MakeJson(Object data)
        {
            string json = JsonSerializer.Serialize(data);
            return new(json, System.Text.Encoding.UTF8, "application/json");
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
                // 💡 インフラレベルの失敗 (接続、DNS、タイムアウト)
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
                // 既に上記のチェックでスローされた例外をそのまま再スロー
                throw;
            }

            return responseInfo;
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

        private PleasanterApiRequest SetEntity<T>(Dictionary<string, T> dataSet)
        {
            PleasanterApiRequest pleasanterApiEntity = new(_apiKey);
            var anoymous = new { };

            pleasanterApiEntity.ClassHash = dataSet
                .Where(data => data.Key.Contains("Class") && data.Value is string)
                .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
            pleasanterApiEntity.NumHash = dataSet
                .Where(data => data.Key.Contains("Num") && data.Value is decimal)
                .ToDictionary(data => data.Key, data => data.Value is decimal d ? d : 0m);
            pleasanterApiEntity.DateHash = dataSet
                .Where(data => data.Key.Contains("Date") && data.Value is DateTime)
                .ToDictionary(data => data.Key, data => data.Value is DateTime dt ? dt : default(DateTime));
            pleasanterApiEntity.DescriptionHash = dataSet
                .Where(data => data.Key.Contains("Description") && data.Value is string)
                .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
            pleasanterApiEntity.CheckHash = dataSet
                .Where(data => data.Key.Contains("Check") && data.Value is bool)
                .ToDictionary(data => data.Key, data => data.Value is bool b ? b : false);
            /*
                pleasanterApiEntity.AttachmentsHash = dataSet
                    .Where(data => data.Key.Contains("Attachments") && data.Value is string)
                    .ToDictionary(data => data.Key, data => data.Value as string ?? string.Empty);
             */

            return pleasanterApiEntity;
        }
    }
}

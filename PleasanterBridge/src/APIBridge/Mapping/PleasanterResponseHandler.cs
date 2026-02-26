using PleasanterBridge.src.APIBridge.Contracts;
using PleasanterBridge.src.APIBridge.Exception;
using System.Net;
using System.Text.Json;

namespace PleasanterBridge.src.APIBridge.Mapping
{
    internal class PleasanterResponseHandler: IPleasanterResponseHandler
    {
        private readonly HttpClient _httpClient;
        public PleasanterResponseHandler(HttpClient httpClient)
        {
      _httpClient = httpClient;
      }

        public async Task<PleasanterApiResponse> SendPost(Dictionary<string, object> request, string url)
        {
    PleasanterApiResponse responseInfo = new();

         try
            {
   StringContent content = MakeJson(request);
    string sendJson = await content.ReadAsStringAsync();

      HttpResponseMessage response = await _httpClient.PostAsync(url, content);
   string responseBody = await response.Content.ReadAsStringAsync();

           // レスポンスがJSONでない場合（HTMLエラーページなど）のハンドリング
      if (!IsJsonResponse(responseBody))
    {
      responseInfo.StatusCode = (int)response.StatusCode;
           responseInfo.Message = $"APIから非JSONレスポンスを受信しました。StatusCode: {(int)response.StatusCode}, Body: {Truncate(responseBody, 500)}";
        responseInfo.Url = url;
       responseInfo.SendData = sendJson;

      throw new PleasanterApiException(
                message: responseInfo.Message,
         responseInfo: responseInfo
      );
         }

           responseInfo = DeserializeJson(responseBody);
        responseInfo.Url = url;
          responseInfo.SendData = sendJson;
    }
            catch (HttpRequestException ex)
    {
        responseInfo.Message = $"Infrastructure error: {ex.Message}";
    responseInfo.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

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
            catch (JsonException ex)
            {
     responseInfo.Message = $"JSONデシリアライズエラー: {ex.Message}";
      responseInfo.Url = url;

         throw new PleasanterApiException(
         message: $"APIレスポンスのJSON解析に失敗しました。",
      responseInfo: responseInfo,
      innerException: ex
       );
    }

            return responseInfo;
        }

      private StringContent MakeJson(Dictionary<string, object> data)
        {
       string json = JsonSerializer.Serialize(data);
   return new(json, System.Text.Encoding.UTF8, "application/json");
     }

   private PleasanterApiResponse DeserializeJson(string responseBody)
   {
      return JsonSerializer.Deserialize<PleasanterApiResponse>(responseBody)
  ?? new PleasanterApiResponse();
   }

        /// <summary>
    /// レスポンスがJSON形式かどうかを判定する
        /// </summary>
        private static bool IsJsonResponse(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
  {
        return false;
      }

            string trimmed = responseBody.TrimStart();
            return trimmed.StartsWith('{') || trimmed.StartsWith('[');
    }

        /// <summary>
        /// 文字列を指定された長さに切り詰める
    /// </summary>
        private static string Truncate(string value, int maxLength)
        {
if (string.IsNullOrEmpty(value)) return string.Empty;
       return value.Length <= maxLength ? value : value[..maxLength] + "...";
        }
    }
}

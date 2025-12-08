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

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                responseInfo = AddInformation(DeserializeJson(responseBody), url, content);
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

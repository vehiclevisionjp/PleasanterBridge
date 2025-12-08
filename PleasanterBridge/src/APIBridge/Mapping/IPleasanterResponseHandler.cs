using PleasanterBridge.src.APIBridge.Contracts;

namespace PleasanterBridge.src.APIBridge.Mapping
{
    interface IPleasanterResponseHandler
    {
        public Task<PleasanterApiResponse> SendPost(Dictionary<string, object> request, string url);
    }
}

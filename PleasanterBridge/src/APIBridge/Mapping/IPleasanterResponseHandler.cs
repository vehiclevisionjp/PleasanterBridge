using PleasanterBridge.src.APIBridge.Contracts;

namespace PleasanterBridge.src.APIBridge.Mapping
{
    public interface IPleasanterResponseHandler
    {
        public Task<PleasanterApiResponse> SendPost(Dictionary<string, object> request, string url);
    }
}

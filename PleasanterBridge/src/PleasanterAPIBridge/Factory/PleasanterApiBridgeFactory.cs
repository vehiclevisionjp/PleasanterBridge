using PleasanterAPIBridge.src.PleasanterAPIBridge.Configuration;
using PleasanterAPIBridge.src.PleasanterAPIBridge.Factory.share;
namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Factory
{
    public class PleasanterApiBridgeFactory : ApiBridgeFactoryBase<IPleasanterSettings>, IApiBridgeFactory<IPleasanterApiBridge, IPleasanterSettings>
    {
        protected override void ApplySpecificSettings(IPleasanterSettings settings, HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                throw new ArgumentException("Pleasanter BaseUrl は必須です。");
            }
            if (string.IsNullOrWhiteSpace(settings.ApiyKey))
            {
                throw new ArgumentException("ApiyKey は必須です。");
            }
        }

        public IPleasanterApiBridge Create(IPleasanterSettings settings)
        {
            HttpClient client = CreateInitializedHttpClient(settings);

            return new PleasanterAPIBridge(
                httpClient: client,
                pleasanterUrlBase: settings.BaseUrl,
                apiKey: settings.ApiyKey
            );
        }
    }
}
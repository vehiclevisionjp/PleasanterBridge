using PleasanterBridge.src.APIBridge.Configuration;
using PleasanterBridge.src.APIBridge.Factory.Share;
namespace PleasanterBridge.src.APIBridge.Factory
{
    public class PleasanterApiBridgeFactory : ApiBridgeFactoryBase<IPleasanterSettings>, IApiBridgeFactory<IPleasanterApiBridge, IPleasanterSettings>
    {
        protected override void ApplySpecificSettings(IPleasanterSettings settings, HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(settings.Url))
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
                pleasanterUrl: settings.Url,
                apiKey: settings.ApiyKey,
                server: settings.Server,
                database: settings.Database,
                userId: settings.UserId,
                password: settings.Password
            );
        }
    }
}
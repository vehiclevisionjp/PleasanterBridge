using Microsoft.Extensions.DependencyInjection;
using PleasanterBridge.src.APIBridge;
using PleasanterBridge.src.APIBridge.Configuration;
using PleasanterBridge.src.APIBridge.Mapping;
using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.DataRepository.Service.Implementation;

namespace PleasanterBridge.src.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPleasanterBridge(
            this IServiceCollection services,
            IPleasanterSettings settings // ユーザーが提供した設定DTO
        )
        {
            // 1. 設定DTOの登録（Singleton）
            services.AddSingleton(settings);

            // HttpClient の設定 (API Keyなど)
            services.AddHttpClient<IPleasanterApiBridge>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(settings.Url);
                    client.DefaultRequestHeaders.Add("X-API-Key", settings.ApiyKey);
                });

            // 2. 依存オブジェクトの登録 (ScopedまたはTransient)
            services.AddTransient<IConnectionSettings>(_ => new ConnectionSettings(
                settings.Server, settings.Database, settings.UserId, settings.Password));

            // 💡 PleasanterRepository 登録 (サービス層)
            // IPleasanterRepository が要求されたら、PleasanterRepository のインスタンスを返す
            services.AddScoped<IPleasanterRepository, PleasanterRepository>();

            // 💡 プロセス/ヘルパーの登録 (Newの代わりにコンテナで解決)
            services.AddTransient<IGetReferenceProcess, GetReferenceProcess>();
            services.AddTransient<IPleasanterUrlProvider, PleasanterUrlProvider>();
            services.AddTransient<IPleasanterResponseHandler, PleasanterResponseHandler>();

            // 3. 本体（API Bridge）の登録
            // HttpClientをコンテナから取得し、他の依存関係もコンテナで解決して注入
            services.AddScoped<IPleasanterApiBridge, PleasanterBridge.src.APIBridge.PleasanterAPIBridge>();

            return services;
        }
    }
}

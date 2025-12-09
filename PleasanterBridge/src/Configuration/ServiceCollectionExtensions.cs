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
            IPleasanterSettings settings
        )
        {
            services.AddSingleton(settings);

            services.AddHttpClient<IPleasanterApiBridge>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(settings.Url);
                    client.DefaultRequestHeaders.Add("X-API-Key", settings.ApiyKey);
                });

            services.AddTransient<IConnectionSettings>(_ => new ConnectionSettings(
                settings.Server, settings.Database, settings.UserId, settings.Password));

            services.AddScoped<IPleasanterRepository, PleasanterRepository>();

            services.AddTransient<IGetReferenceProcess, GetReferenceProcess>();
            services.AddTransient<IPleasanterUrlProvider, PleasanterUrlProvider>();
            services.AddTransient<IPleasanterResponseHandler, PleasanterResponseHandler>();

            services.AddScoped<IPleasanterApiBridge, PleasanterBridge.src.APIBridge.PleasanterAPIBridge>();

            return services;
        }
    }
}

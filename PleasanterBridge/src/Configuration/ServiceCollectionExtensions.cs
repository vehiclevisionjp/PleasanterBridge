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
            IPleasanterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "Settings cannot be null");
            }

            // İ’èî•ñ‚ğƒVƒ“ƒOƒ‹ƒgƒ“‚Å“o˜^
            services.AddSingleton(settings);

            // HttpClient ‚ğ“o˜^iAPI ƒAƒNƒZƒX—pj
            services.AddHttpClient();

            // PleasanterUrlProvider ‚ğ“o˜^isettings.Url ‚ğ’“üj
            services.AddTransient<IPleasanterUrlProvider>(provider =>
                new PleasanterUrlProvider(settings.Url));

            // PleasanterResponseHandler ‚ğ“o˜^
            services.AddTransient<IPleasanterResponseHandler>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                return new PleasanterResponseHandler(httpClient);
            });

            // PleasanterRepository ‚ğ“o˜^
            services.AddScoped<IPleasanterRepository>(provider =>
                new PleasanterRepository(settings));

            // GetReferenceProcess ‚ğ“o˜^
            services.AddTransient<IGetReferenceProcess>(provider =>
                new GetReferenceProcess(settings));

            // PleasanterAPIBridge ‚ğ“o˜^
            services.AddScoped<IPleasanterApiBridge>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                var urlProvider = provider.GetRequiredService<IPleasanterUrlProvider>();
                var repository = provider.GetRequiredService<IPleasanterRepository>();
                var referenceProcess = provider.GetRequiredService<IGetReferenceProcess>();
                var responseHandler = provider.GetRequiredService<IPleasanterResponseHandler>();

                return new PleasanterAPIBridge(
                    httpClient,
                    settings,
                    urlProvider,
                    repository,
                    referenceProcess,
                    responseHandler);
            });

            return services;
        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage;

public static class ServiceCollectionExt
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, ExtensionEgressPayload payload)
        => services
            .AddSingleton<GoogleCloudStorageEgressProvider>()
            .AddSingleton<IGoogleCloudStorage, GoogleCloudStorage>()
            .AddEgressOptions(payload);

    public static IServiceCollection AddEgressOptions(this IServiceCollection services, ExtensionEgressPayload payload)
    {
        services.AddOptions<GoogleCloudStorageEgressProviderOptions>()
            .Configure(options =>
            {
                var builder = new ConfigurationBuilder();
                builder
                    .AddInMemoryCollection(payload.Configuration)
                    .Build()
                    .Bind(options);
            }).ValidateDataAnnotations();
        services.AddSingleton<GoogleCloudStorageEgressProviderOptions>(c =>
            c.GetRequiredService<IOptions<GoogleCloudStorageEgressProviderOptions>>().Value);
        return services;
    }
}
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        using var cancellationTokenSource = GetCts();
        var cancellationToken = cancellationTokenSource.Token;

        return await Run();

        async Task<int> Run()
        {
            if (args.Length < 2)
                return -1;

            if (!args[0].Equals("egress", StringComparison.OrdinalIgnoreCase))
                return -2;

            var result = await RunInMode(args[1].ToLowerInvariant());
            return ProcessEgressResult(result);
        }

        async Task<EgressArtifactResult> RunInMode(string mode)
        {
            switch (mode)
            {
                case "execute":
                    return await Egress(cancellationToken);
                case "validate":
                    return await Validate(cancellationToken);
                default:
                    return new EgressArtifactResult
                    {
                        Succeeded = false,
                        FailureMessage = "Unexpected mode"
                    };
            }
        }
    }
    
    private static async Task<EgressArtifactResult> Egress(CancellationToken cancellationToken)
    {
        try
        {
            var (payload, services) = await BuildServiceProvider(cancellationToken);
            await using var _1 = services;
            var resourceId = await services.GetRequiredService<GoogleCloudStorageEgressProvider>()
                .Egress(payload.Settings, cancellationToken);
            return new EgressArtifactResult
            {
                Succeeded = true,
                ArtifactPath = resourceId,
            };
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Failed to egress artifact to google cloud storage: {0}", e);
            return new EgressArtifactResult
            {
                Succeeded = false,
                FailureMessage = e.ToString()
            };
        }
    }

    private static async Task<EgressArtifactResult> Validate(CancellationToken cancellationToken)
    {
        try
        {
            var (_, services) = await BuildServiceProvider(cancellationToken);
            await using var _1 = services;
            services.GetRequiredService<GoogleCloudStorageEgressProviderOptions>();
            return new EgressArtifactResult
            {
                Succeeded = true,
            };
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Failed to validate google cloud storage egress extension configuration: {0}", e);
            return new EgressArtifactResult
            {
                Succeeded = false,
                FailureMessage = e.ToString()
            };
        }
    }

    private static async Task<(ExtensionEgressPayload payload, ServiceProvider services)> BuildServiceProvider(CancellationToken cancellationToken)
    {
        ServiceProvider? services = null;
        try
        {
            Console.WriteLine("Reading stdin...");
            var stdIn = new StdIn();
            var payload = await stdIn.GetPayload(cancellationToken);
            Console.WriteLine("Received payload from stdin");
            services = BuildServices(payload, stdIn);
            return (payload, services);
        }
        catch
        {
            if (services != null) await services.DisposeAsync();
            throw;
        }
    }


    private static int ProcessEgressResult(EgressArtifactResult result)
    {
        var jsonBlob = JsonSerializer.Serialize(result);
        Console.Write(jsonBlob);
        // return non-zero exit code when failed
        return result.Succeeded ? 0 : 1;
    }

    private static ServiceProvider BuildServices(ExtensionEgressPayload extensionEgressPayload, StdIn stdIn)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var serviceProvider = new ServiceCollection()
            .ConfigureServices(extensionEgressPayload)
            .AddSingleton(stdIn)
            .AddLogging(builder => builder.AddConfiguration(config.GetRequiredSection("Logging")).AddConsole())
            .BuildServiceProvider();
        return serviceProvider;
    }

    private static CancellationTokenSource GetCts()
    {
        var cancellationTokenSource1 = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            cancellationTokenSource1.Cancel();
            e.Cancel = true;
        };
        return cancellationTokenSource1;
    }
}
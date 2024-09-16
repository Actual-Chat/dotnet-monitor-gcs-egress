using Microsoft.Extensions.Logging;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage;

public sealed class GoogleCloudStorageEgressProvider(
    IGoogleCloudStorage googleCloudStorage,
    StdIn stdIn,
    ILogger<GoogleCloudStorageEgressProvider> log)
{
    public async Task<string> Egress(EgressArtifactSettings artifactSettings, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = stdIn.GetBlobStream();
            var resource = await googleCloudStorage.UploadAsync(artifactSettings.Name, artifactSettings.ContentType, stream, cancellationToken);
            return resource.Name;
        }
        catch (Exception e) when(e is not OperationCanceledException && !cancellationToken.IsCancellationRequested)
        {
            log.LogCritical(e, "Failed to egress artifact to google cloud storage");
            throw;
        }
    }
}
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage
{
    internal sealed class GoogleCloudStorage(GoogleCloudStorageEgressProviderOptions options, ILogger<GoogleCloudStorage> log) : IGoogleCloudStorage
    {
        public async Task<Object> UploadAsync(string objectId, string contentType, Stream inputStream, CancellationToken token)
        {
            var bucketName = options.BucketName;
            log.LogInformation("Uploading to gcs '{BucketName}/{ObjectId}'", bucketName, objectId);
            using var client = await StorageClient.CreateAsync().ConfigureAwait(false);
            return await client.UploadObjectAsync(bucketName, objectId, contentType, inputStream, cancellationToken: token);
        }
    }
}

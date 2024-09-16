using Object = Google.Apis.Storage.v1.Data.Object;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage
{
    public interface IGoogleCloudStorage
    {
        Task<Object> UploadAsync(string objectId, string contentType, Stream inputStream, CancellationToken token);
    }
}

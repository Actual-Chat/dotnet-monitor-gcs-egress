using System.ComponentModel.DataAnnotations;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage
{
    public sealed class GoogleCloudStorageEgressProviderOptions
    {
        [Required]
        public string BucketName { get; set; } = "";
    }
}

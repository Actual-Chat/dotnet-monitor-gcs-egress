using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage;

// TODO: copied from Microsoft.Diagnostics.Monitoring.Extension.Common. Use nuget when available
public sealed class ExtensionEgressPayload
{
    public EgressArtifactSettings Settings { get; set; } = null!;
    public Dictionary<string, string> Properties { get; set; } = null!;
    public Dictionary<string, string?> Configuration { get; set; } = null!;
    public string ProviderName { get; set; } = null!;
    [JsonConverter(typeof(JsonStringEnumConverter))] public LogLevel LogLevel { get; set; }
}

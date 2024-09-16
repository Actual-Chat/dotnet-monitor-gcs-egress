namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage;

// TODO: copied from Microsoft.Diagnostics.Monitoring.Extension.Common. Use nuget when available
public sealed class EgressArtifactSettings
{
    public string ContentType { get; set; } = "";
    public Dictionary<string, string> Metadata { get; set; }
        = new Dictionary<string, string>(StringComparer.Ordinal);
    public Dictionary<string, string> CustomMetadata { get; }
        = new Dictionary<string, string>(StringComparer.Ordinal);
    public Dictionary<string, string> EnvBlock { get; set; }
        = new Dictionary<string, string>(StringComparer.Ordinal);
    public string Name { get; set; } = "";
}

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage;

// TODO: copied from Microsoft.Diagnostics.Monitoring.Extension.Common. Use nuget when available
public sealed class EgressException : Exception
{
    public EgressException(string message) : base(message) { }

    public EgressException(string message, Exception innerException) : base(message, innerException) { }
}
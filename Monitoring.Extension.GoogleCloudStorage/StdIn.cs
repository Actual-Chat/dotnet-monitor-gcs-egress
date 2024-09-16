using System.Text;
using System.Text.Json;

namespace Microsoft.Diagnostics.Monitoring.Extension.GoogleCloudStorage;

public class StdIn
{
    private const int ExpectedPayloadProtocolVersion = 1;
    private readonly Stream _stream = Console.OpenStandardInput();

    public async Task<ExtensionEgressPayload> GetPayload(CancellationToken cancellationToken)
    {
        using var reader = new BinaryReader(_stream, Encoding.UTF8, leaveOpen: true);
        var dotnetMonitorPayloadProtocolVersion = reader.ReadInt32();
        if (dotnetMonitorPayloadProtocolVersion != ExpectedPayloadProtocolVersion)
            throw new EgressException(
                $"Dotnet monitor payload protocol version {dotnetMonitorPayloadProtocolVersion} does not match expected one {ExpectedPayloadProtocolVersion}.");

        var payloadLengthBuffer = reader.ReadInt64();
        if (payloadLengthBuffer < 0)
            throw new ArgumentOutOfRangeException(nameof(payloadLengthBuffer));
        
        var payloadBuffer = new byte[payloadLengthBuffer];
        await _stream.ReadExactlyAsync(payloadBuffer, cancellationToken);
        var configPayload = JsonSerializer.Deserialize<ExtensionEgressPayload>(payloadBuffer)!;
        return configPayload;
    }

    public async ValueTask DisposeAsync() 
        => await _stream.DisposeAsync();

    public Stream GetBlobStream() 
        => _stream;
}
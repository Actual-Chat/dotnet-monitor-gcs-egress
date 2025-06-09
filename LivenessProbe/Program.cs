using System.Net.Http.Json;

var url = args.FirstOrDefault() ?? "http://localhost:54323/processes";
using var http = new HttpClient();
var processes = await http.GetFromJsonAsync<Process[]>(url, CancellationToken.None) ?? [];
if (processes.Any(x => x.Pid > 0))
    return 0;

Console.Error.WriteLine($"No processes returned by endpoint {url}.");
return 1;

public sealed record Process(int Pid);
using System.Text;
using System.Linq;

namespace CosmosDbLocalReverseProxy;

public class ReverseProxyClient
{
    private static string[] AllowedHeaders = new[] {
        "Accept",
        "authorization",
        "x-ms-date",
        "x-ms-version",
        "x-ms-offer-throughput"
    };

    private static Uri RelayServerHost = new Uri("https://localhost:8081");

    private HttpClient m_client;

    public ReverseProxyClient(HttpClient client)
    {
        m_client = client;
    }

    public async Task<HttpResponseMessage> RelayCall(HttpRequest request)
    {
        HttpMethod method = new(request.Method);
        HttpRequestMessage relayedRequest = new(method, BuildPath(request));

        if (method != HttpMethod.Get)
        {
            using var reader = new StreamReader(request.Body);
            var requestBody = await reader.ReadToEndAsync();
            relayedRequest.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        }

        foreach (var header in request.Headers.Where(h => AllowedHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase)))
        {
            relayedRequest.Headers.Add(header.Key, header.Value.ToArray());
        }

        return await m_client.SendAsync(relayedRequest);
    }

    private Uri BuildPath(HttpRequest request) =>
        new(RelayServerHost, request.Path);
}

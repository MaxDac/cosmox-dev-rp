using System.Text;
using System.Linq;

namespace CosmosDbLocalReverseProxy;

public class ReverseProxyClient
{

    private static Uri RelayServerHost = new Uri("https://localhost:8081");
    private const string QueryContentType = "application/query+json";

    private readonly HttpClient m_client;
    private readonly ILogger<ReverseProxyClient> m_logger;

    public ReverseProxyClient(HttpClient client, ILogger<ReverseProxyClient> logger)
    {
        m_client = client;
        m_logger = logger;
    }

    public async Task<HttpResponseMessage> RelayCall(HttpRequest request)
    {
        m_logger.LogInformation("Received {Method} request to {Path}", request.Method, request.Path);
        HttpMethod method = new(request.Method);
        HttpRequestMessage relayedRequest = new(method, BuildPath(request));

        if (method != HttpMethod.Get)
        {
            using var reader = new StreamReader(request.Body);
            var requestBody = await reader.ReadToEndAsync();
            m_logger.LogInformation("Request contains body: {Body}", requestBody);
            var contentType = request.Headers.ContentType;
            relayedRequest.Content = new StringContent(requestBody, Encoding.UTF8, contentType);
            
            if (contentType == QueryContentType)
            {
                // Important for the query requests.
                relayedRequest.Content.Headers!.ContentType!.CharSet = "";
            }
        }

        var headers = string.Join("\r\n", request.Headers
            .Select(h => $"{h.Key} - {h.Value}"));

        m_logger.LogInformation("Request headers: {Headers}", headers);

        foreach (var header in request.Headers.Where(h => Constants.AllowedRequestHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase)))
        {
            m_logger.LogInformation("Adding header: {HeaderKey}", header.Key);
            relayedRequest.Headers.Add(header.Key, header.Value.ToArray());
        }

        return await m_client.SendAsync(relayedRequest);
    }

    private Uri BuildPath(HttpRequest request) =>
        new(RelayServerHost, request.Path);
}

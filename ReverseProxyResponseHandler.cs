using CosmosDbLocalReverseProxy.ResponseHandlers;

namespace CosmosDbLocalReverseProxy;

internal class ReverseProxyResponseHandler
{
    private readonly ResponseHandlerFactory m_factory;
    private readonly ILogger<ReverseProxyResponseHandler> m_logger;

    public ReverseProxyResponseHandler(ResponseHandlerFactory factory, ILogger<ReverseProxyResponseHandler> logger)
    {
        m_factory = factory;
        m_logger = logger;
    }

    public async Task HandleResponseAsync(HttpContext context, HttpResponseMessage response)
    {
        var lastHeaderKey = "";
        context.Response.StatusCode = (int)response.StatusCode;
        var handler = m_factory.GetResponseHandler(response.StatusCode);

        if (context.Response.StatusCode < 300)
        {
            m_logger?.LogWarning("Received Status Code {StatusCode}", context.Response.StatusCode);
        }
        else
        {
            m_logger?.LogInformation("Received Status Code {StatusCode}", context.Response.StatusCode);
        }

        foreach (var header in response.Headers.Where(h => Constants.AllowedResponseHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase)))
        {
            TryAddHeader(context.Response, header, ref lastHeaderKey);
        }

        await handler.HandleResponse(context, response);
    }

    private static void TryAddHeader(HttpResponse response, KeyValuePair<string, IEnumerable<string>> header, ref string lastHeaderKey)
    {
        try
        {
            lastHeaderKey = header.Key;
            response.Headers.TryAdd(header.Key, header.Value.ToArray());
        }
        catch
        {
            Console.WriteLine("Can't put this header into the response: {HeaderName}", header.Key);
        } 
    }
}
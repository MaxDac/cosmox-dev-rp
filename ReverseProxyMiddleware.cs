namespace CosmosDbLocalReverseProxy;

public class ReverseProxyMiddleware
{
    private readonly RequestDelegate m_next;
    private readonly ReverseProxyClient m_client;
    private readonly ILogger<ReverseProxyMiddleware> m_logger;

    public ReverseProxyMiddleware(RequestDelegate next, ReverseProxyClient client, ILogger<ReverseProxyMiddleware> logger)
    {
        m_next = next;
        m_logger = logger;
        m_client = client;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.ToString().Contains("something", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsJsonAsync(new Error("It's really not an error"));
            return;
        }
        
        if (m_logger is null)
        {
            Console.WriteLine("The m_logger is null");
        }

        var lastHeaderKey = "";

        if (m_client is null)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new Error("The m_client is not configured"));
            return;
        }

        try
        {
            var response = await m_client.RelayCall(context.Request);

            context.Response.StatusCode = (int)response.StatusCode;

            if (context.Response.StatusCode != 200)
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

            var peekResponseBody = await response.Content.ReadAsStringAsync();
            m_logger?.LogInformation("Received body: {Body}", peekResponseBody);
            await context.Response.WriteAsync(peekResponseBody);
        }
        catch (Exception ex)
        {
            m_logger?.LogError(ex, "An error occurred while calling CosmosDB: {Message}", ex.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new Error(ex.Message));
        }
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
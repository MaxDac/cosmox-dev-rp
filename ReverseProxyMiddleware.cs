namespace CosmosDbLocalReverseProxy;

internal class ReverseProxyMiddleware
{
    private readonly RequestDelegate m_next;
    private readonly ReverseProxyClient m_client;
    private readonly ReverseProxyResponseHandler m_responseHandler;
    private readonly ILogger<ReverseProxyMiddleware> m_logger;

    public ReverseProxyMiddleware(
        RequestDelegate next,
        ReverseProxyClient client, 
        ReverseProxyResponseHandler responseHandler, 
        ILogger<ReverseProxyMiddleware> logger)
    {
        m_next = next;
        m_client = client;
        m_responseHandler = responseHandler;
        m_logger = logger;
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

        if (m_client is null)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new Error("The m_client is not configured"));
            return;
        }

        try
        {
            var response = await m_client.RelayCall(context.Request);
            await m_responseHandler.HandleResponseAsync(context, response);
        }
        catch (Exception ex)
        {
            m_logger?.LogError(ex, "An error occurred while calling CosmosDB: {Message}", ex.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new Error(ex.Message));
        }
    }
}
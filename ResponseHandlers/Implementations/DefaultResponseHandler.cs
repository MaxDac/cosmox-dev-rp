namespace CosmosDbLocalReverseProxy.ResponseHandlers.Implementations;

internal class DefaultResponseHandler : IResponseHandler
{
    private readonly ILogger<DefaultResponseHandler> m_logger;

    public DefaultResponseHandler(ILogger<DefaultResponseHandler> logger)
    {
        m_logger = logger;
    }

    public async Task HandleResponse(HttpContext context, HttpResponseMessage response)
    {
        var peekResponseBody = await response.Content.ReadAsStringAsync();
        m_logger?.LogInformation("Received body: {Body}", peekResponseBody);
        await context.Response.WriteAsync(peekResponseBody);
    }
}
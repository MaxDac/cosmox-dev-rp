namespace CosmosDbLocalReverseProxy.ResponseHandlers.Implementations;

internal class NoBodyResponseHandler : IResponseHandler
{
    private readonly ILogger<NoBodyResponseHandler> m_logger;

    public NoBodyResponseHandler(ILogger<NoBodyResponseHandler> logger)
    {
        m_logger = logger;
    }

    public Task HandleResponse(HttpContext context, HttpResponseMessage response)
    {
        return Task.CompletedTask;
    }
}

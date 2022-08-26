using System.Net;
using CosmosDbLocalReverseProxy.ResponseHandlers.Implementations;

namespace CosmosDbLocalReverseProxy.ResponseHandlers;

internal class ResponseHandlerFactory
{
    private readonly DefaultResponseHandler m_defaultResponseHandler;
    private readonly NoBodyResponseHandler m_noBodyResponseHandler;
    private readonly ILogger<ResponseHandlerFactory> m_logger;

    public ResponseHandlerFactory(
        DefaultResponseHandler defaultResponseHandler,
        NoBodyResponseHandler noBodyResponseHandler,
        ILogger<ResponseHandlerFactory> logger)
    {
        m_defaultResponseHandler = defaultResponseHandler;
        m_noBodyResponseHandler = noBodyResponseHandler;
        m_logger = logger;
    }

    public IResponseHandler GetResponseHandler(HttpStatusCode statusCode) =>
        (int)statusCode switch
        {
            204 => m_noBodyResponseHandler,
            _ => m_defaultResponseHandler
        };
}

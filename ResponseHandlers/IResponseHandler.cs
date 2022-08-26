using System.Net;

namespace CosmosDbLocalReverseProxy.ResponseHandlers;

public interface IResponseHandler
{
    public Task HandleResponse(HttpContext context, HttpResponseMessage response);
}

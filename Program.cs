using CosmosDbLocalReverseProxy;
using CosmosDbLocalReverseProxy.ResponseHandlers;
using CosmosDbLocalReverseProxy.ResponseHandlers.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ReverseProxyClient>();

builder.Services.AddSingleton<ReverseProxyResponseHandler>();

builder.Services
    .AddSingleton<ResponseHandlerFactory>()
    .AddSingleton<DefaultResponseHandler>()
    .AddSingleton<NoBodyResponseHandler>();

var app = builder.Build();

app.UseMiddleware<ReverseProxyMiddleware>();

app.Run();

record struct Error(string Message);

using System.Diagnostics;
using System.Net.Http.Headers;
using CosmosDbLocalReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ReverseProxyClient>();

var app = builder.Build();



app.UseMiddleware<ReverseProxyMiddleware>();

app.Run();

record struct Error(string Message);

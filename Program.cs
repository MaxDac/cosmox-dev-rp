using System.Diagnostics;
using System.Net.Http.Headers;
using CosmosDbLocalReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ReverseProxyClient>();

var app = builder.Build();

var AllowedHeaders = new[] {
    "Cache-Control",
    "Pragma",
    // "Transfer-Encoding",
    "Content-Type",
    "Server",
    "Access-Control-Allow-Origin",
    "Access-Control-Allow-Credentials",
    "x-ms-activity-id",
    "x-ms-last-state-change-utc",
    "etag",
    "x-ms-resource-quota",
    "x-ms-resource-usage",
    "x-ms-schemaversion",
    "lsn",
    "x-ms-request-charge",
    "x-ms-quorum-acked-lsn",
    "x-ms-current-write-quorum",
    "x-ms-current-replica-set-size",
    "x-ms-xp-role",
    "x-ms-global-Committed-lsn",
    "x-ms-number-of-read-regions",
    "x-ms-item-lsn",
    "x-ms-transport-request-id",
    "x-ms-cosmos-llsn",
    "x-ms-cosmos-quorum-acked-llsn",
    "x-ms-cosmos-item-llsn",
    "x-ms-session-token",
    "x-ms-request-duration-ms",
    "x-ms-serviceversion",
    "x-ms-gatewayversion"
};

void TryAddHeader(HttpResponse response, KeyValuePair<string, IEnumerable<string>> header, ref string lastHeaderKey)
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

app.Use(async (HttpContext context, Func<Task> _next) => 
{
    if (context.Request.Path.ToString().Contains("something", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsJsonAsync(new Error("It's really not an error"));
        return;
    }

    var client = app.Services.GetService<ReverseProxyClient>();
    var logger = app.Services.GetService<ILogger>();
    
    if (logger is null)
    {
        Console.WriteLine("The logger is null");
    }

    var lastHeaderKey = "";

    if (client is null)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new Error("The client is not configured"));
        return;
    }

    try
    {
        var response = await client.RelayCall(context.Request);

        context.Response.StatusCode = (int)response.StatusCode;

        if (context.Response.StatusCode != 200)
        {
            logger?.LogWarning("Received Status Code {StatusCode}", context.Response.StatusCode);
        }
        else
        {
            logger?.LogInformation("Received Status Code {StatusCode}", context.Response.StatusCode);
        }

        foreach (var header in response.Headers.Where(h => AllowedHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase)))
        {
            TryAddHeader(context.Response, header, ref lastHeaderKey);
        }

        var peekResponseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Received Body: {peekResponseBody}");
        Debug.WriteLine($"Received Body: {peekResponseBody}");
        logger?.LogInformation("Received body: {Body}", peekResponseBody);
        await context.Response.WriteAsync(peekResponseBody);
    }
    catch (Exception ex)
    {
        logger?.LogError(ex, "An error occurred while calling CosmosDB: {Message}", ex.Message);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new Error(ex.Message));
    }
});

app.Run();

record struct Error(string Message);

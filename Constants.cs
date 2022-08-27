namespace CosmosDbLocalReverseProxy;

public static class Constants
{
    public static string[] AllowedRequestHeaders = new[] {
        "Accept",
        "authorization",
        "x-ms-date",
        "x-ms-version",
        "x-ms-offer-throughput",
        "x-ms-version",
        "x-ms-documentdb-partitionkey"
    };

    public static string[] AllowedResponseHeaders = new[] {
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
}
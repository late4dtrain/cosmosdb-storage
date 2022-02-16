namespace Late4dTrain.CosmosDbStorage.Extensions;

using System.Net;

public static class StatusExtensions
{
    public static bool IsSuccessStatusCode(this HttpStatusCode statusCode) =>
        (int)statusCode >= 200 && (int)statusCode <= 299;
}

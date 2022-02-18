namespace Late4dTrain.CosmosDbStorage;

using Microsoft.Azure.Cosmos;

public interface IWriteOnlyStorage<TEntity>
{
    Task<Result<string, ItemResponse<TEntity>>> SaveAsync(
        TEntity document,
        string partitionKey,
        string? eTag = null,
        CancellationToken cancellationToken = default);

    Task<Result<string, ItemResponse<TEntity>>> DeleteAsync(
        string identifier,
        string partitionKey,
        string? eTag = null,
        CancellationToken cancellationToken = default);
}

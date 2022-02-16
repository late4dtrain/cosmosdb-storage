namespace Late4dTrain.CosmosDbStorage;

public interface IWriteOnlyStorage<TEntity>
{
    Task<Result<string>> SaveAsync(
        TEntity document,
        string partitionKey,
        string? eTag = null,
        CancellationToken cancellationToken = default);

    Task<Result<string>> DeleteAsync(
        string identifier,
        string partitionKey,
        string? eTag = null,
        CancellationToken cancellationToken = default);
}

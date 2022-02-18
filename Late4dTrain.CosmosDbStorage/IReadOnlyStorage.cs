namespace Late4dTrain.CosmosDbStorage;

using System.Linq.Expressions;

using Microsoft.Azure.Cosmos;

public interface IReadOnlyStorage<TEntity>
{
    Task<Result<string, ItemResponse<TEntity>>> GetByIdAsync(
        string identifier,
        string partitionKey,
        string? eTag = default,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<Result<string, TEntity>> GetByAsync(
        Expression<Func<TEntity, bool>> expression, string continuationToken = "",
        CancellationToken cancellationToken = default);
}

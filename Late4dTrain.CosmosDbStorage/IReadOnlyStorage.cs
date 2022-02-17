namespace Late4dTrain.CosmosDbStorage;

using System.Linq.Expressions;

public interface IReadOnlyStorage<TEntity>
{
    Task<Result<string, TEntity>> GetByIdAsync(
        string identifier,
        string partitionKey,
        string? eTag = default,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<Result<string, TEntity>> GetByAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default);
}

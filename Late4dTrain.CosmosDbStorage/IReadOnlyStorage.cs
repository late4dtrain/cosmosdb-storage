namespace Late4dTrain.CosmosDbStorage;

using System.Linq.Expressions;

public interface IReadOnlyStorage<TEntity>
{
    IAsyncEnumerable<Result<TEntity, string>> GetByAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default(CancellationToken));
}

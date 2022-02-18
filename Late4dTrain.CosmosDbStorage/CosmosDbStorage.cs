namespace Late4dTrain.CosmosDbStorage;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Configuration;

using Extensions;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

using Strategies;

using static Result<string>;

public class CosmosDbStorage<TEntity> : IReadOnlyStorage<TEntity>, IWriteOnlyStorage<TEntity>
{
    private readonly CosmosClientConfiguration _configuration;
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<CosmosDbStorage<TEntity>> _logger;
    private readonly IRetryStrategy<CosmosDbResiliencePolicyConfiguration> _retryStrategy;

    public CosmosDbStorage(
        CosmosClient cosmosClient,
        CosmosClientConfiguration configuration,
        IRetryStrategy<CosmosDbResiliencePolicyConfiguration> retryStrategy,
        ILogger<CosmosDbStorage<TEntity>> logger) =>
        (_cosmosClient, _configuration, _retryStrategy, _logger) = (cosmosClient, configuration, retryStrategy, logger);

    public CosmosDbStorage(Action<CosmosClientConfiguration> options, ILogger<CosmosDbStorage<TEntity>> logger)
    {
        _configuration = new CosmosClientConfiguration();
        options(_configuration);
        _cosmosClient = new CosmosClient(_configuration.EndpointUrl, _configuration.PrimaryKey);
        _retryStrategy = DefaultCosmosDbExponentialRetryStrategy.Instance;
        _logger = logger;
    }

    public async Task<Result<string, ItemResponse<TEntity>>> GetByIdAsync(
        string identifier,
        string partitionKey,
        string? eTag = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var container = GetContainer();

            var itemResponse = await container.ReadItemAsync<TEntity>(
                identifier,
                new PartitionKey(partitionKey),
                GetRequestOptions(eTag),
                cancellationToken
            );

            return itemResponse.StatusCode.IsSuccessStatusCode()
                ? Ok(itemResponse)
                : Fail<ItemResponse<TEntity>>($"{itemResponse.StatusCode}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Fail<ItemResponse<TEntity>>(e.Message);
        }
    }

    public async IAsyncEnumerable<Result<string, TEntity>> GetByAsync(
        Expression<Func<TEntity, bool>> expression,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var container = GetContainer();

        using var setIterator = container
            .GetItemLinqQueryable<TEntity>()
            .Where(expression)
            .ToFeedIterator();

        while (setIterator.HasMoreResults)
            foreach (var item in await _retryStrategy
                         .Policy
                         .ExecuteAsync(
                             async ct =>
                                 await setIterator.ReadNextAsync(ct),
                             cancellationToken
                         ))
            {
                Result<string, TEntity> result;
                try
                {
                    result = Ok(item);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result = Fail<TEntity>(e.Message);
                }

                yield return result;
            }
    }

    public async Task<Result<string, ItemResponse<TEntity>>> DeleteAsync(
        string identifier,
        string partitionKey,
        string? eTag = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var container = GetContainer();

            var itemResponse = await _retryStrategy
                .Policy
                .ExecuteAsync(
                    async ct => await container.DeleteItemAsync<TEntity>(
                        identifier,
                        new PartitionKey(partitionKey),
                        GetRequestOptions(eTag),
                        ct
                    ),
                    cancellationToken
                );

            return itemResponse.StatusCode.IsSuccessStatusCode()
                ? Ok(itemResponse)
                : Fail<ItemResponse<TEntity>>($"{itemResponse.StatusCode}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Fail<ItemResponse<TEntity>>(e.Message);
        }
    }

    public async Task<Result<string, ItemResponse<TEntity>>> SaveAsync(
        TEntity document,
        string partitionKey,
        string? eTag = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var container = GetContainer();

            var itemResponse = await _retryStrategy
                .Policy
                .ExecuteAsync(
                    async ct => await container.UpsertItemAsync(
                        document,
                        new PartitionKey(partitionKey),
                        GetRequestOptions(eTag),
                        ct
                    ),
                    cancellationToken
                );

            return itemResponse.StatusCode.IsSuccessStatusCode()
                ? Ok(itemResponse)
                : Fail<ItemResponse<TEntity>>($"{itemResponse.StatusCode}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Fail<ItemResponse<TEntity>>(e.Message);
        }
    }

    private Container GetContainer() => _cosmosClient.GetContainer(
        _configuration.DatabaseName,
        _configuration.ContainerName
    );

    private ItemRequestOptions? GetRequestOptions(string? eTag) =>
        eTag == null
            ? null
            : new ItemRequestOptions
            {
                IfMatchEtag = eTag
            };
}

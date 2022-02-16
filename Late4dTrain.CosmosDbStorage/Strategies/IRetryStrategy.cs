namespace Late4dTrain.CosmosDbStorage.Strategies;

using Polly;

public interface IRetryStrategy<T>
{
    public IAsyncPolicy Policy { get; }
}

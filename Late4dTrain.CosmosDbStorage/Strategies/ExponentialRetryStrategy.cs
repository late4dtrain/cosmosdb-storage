namespace Late4dTrain.CosmosDbStorage.Strategies;

using Configuration;

using Polly;

public class ExponentialRetryStrategy<T> : IRetryStrategy<T> where T : class, IResiliencePolicyConfiguration, new()
{
    public ExponentialRetryStrategy(T configuration) =>
        Policy =
            Polly.Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    configuration.Resilience.Retry,
                    i => TimeSpan.FromSeconds(Math.Pow(++i, 2))
                );

    public IAsyncPolicy Policy { get; }
}

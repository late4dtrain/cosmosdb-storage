namespace Late4dTrain.CosmosDbStorage.Strategies;

using Configuration;

public class DefaultCosmosDbExponentialRetryStrategy : ExponentialRetryStrategy<CosmosDbResiliencePolicyConfiguration>
{
    protected DefaultCosmosDbExponentialRetryStrategy(CosmosDbResiliencePolicyConfiguration configuration) : base(
        configuration
    )
    {
    }

    public static DefaultCosmosDbExponentialRetryStrategy Instance => new(
        new CosmosDbResiliencePolicyConfiguration
        {
            Resilience = new ResilienceConfiguration { Retry = 3 }
        }
    );
}

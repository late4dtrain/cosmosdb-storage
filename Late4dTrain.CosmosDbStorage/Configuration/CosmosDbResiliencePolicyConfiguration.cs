namespace Late4dTrain.CosmosDbStorage.Configuration;

public class CosmosDbResiliencePolicyConfiguration : IResiliencePolicyConfiguration
{
    public ResilienceConfiguration Resilience { get; set; } = new() { Retry = 3 };
}

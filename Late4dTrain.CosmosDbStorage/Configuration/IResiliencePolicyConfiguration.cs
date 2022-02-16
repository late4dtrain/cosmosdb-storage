namespace Late4dTrain.CosmosDbStorage.Configuration;

public interface IResiliencePolicyConfiguration
{
    public ResilienceConfiguration Resilience { get; set; }
}

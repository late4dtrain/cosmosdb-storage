namespace Late4dTrain.CosmosDbStorage.Configuration;

public class ResilienceConfiguration
{
    public int Retry { get; set; }

    public int DelayInSeconds { get; set; }
}

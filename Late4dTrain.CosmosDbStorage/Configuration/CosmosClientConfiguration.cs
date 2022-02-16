namespace Late4dTrain.CosmosDbStorage.Configuration;

public class CosmosClientConfiguration
{
    public string DatabaseName { get; set; } = string.Empty;

    public string ContainerName { get; set; } = string.Empty;

    public string EndpointUrl { get; set; } = string.Empty;

    public string PrimaryKey { get; set; } = string.Empty;
}

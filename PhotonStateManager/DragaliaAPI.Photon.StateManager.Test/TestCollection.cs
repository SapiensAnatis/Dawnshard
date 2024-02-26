namespace DragaliaAPI.Photon.StateManager.Test;

[CollectionDefinition(Name)]
public class TestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "IntegrationTest";
}

namespace DragaliaAPI.Integration.Test;

[CollectionDefinition(Name)]
public class TestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "DragaliaIntegration";
}

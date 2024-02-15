namespace DragaliaAPI.Integration.Test;

// ReSharper disable once RedundantTypeDeclarationBody
[CollectionDefinition("DragaliaIntegration")]
public class TestContainersFixture : ICollectionFixture<TestContainersHelper>
{
    public TestContainersFixture(TestContainersHelper helper)
    {
        Console.WriteLine("a");
    }
}

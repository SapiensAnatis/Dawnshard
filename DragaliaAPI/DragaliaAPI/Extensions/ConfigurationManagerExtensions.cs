namespace DragaliaAPI.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddResourcesJsonFile(
        this IConfigurationBuilder builder,
        string fileName
    )
    {
        return builder.AddJsonFile(
            Path.Join("Resources", fileName),
            optional: true,
            reloadOnChange: true
        );
    }
}

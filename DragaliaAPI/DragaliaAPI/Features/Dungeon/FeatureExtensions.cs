using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Dungeon.Start;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddDungeonFeature(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<IDungeonService, DungeonService>()
            .AddScoped<IDungeonStartService, DungeonStartService>()
            .AddScoped<IDungeonRepository, DungeonRepository>()
            .AddScoped<IQuestEnemyService, QuestEnemyService>()
            .AddScoped<IOddsInfoService, OddsInfoService>()
            .AddScoped<IDungeonRecordService, DungeonRecordService>()
            .AddScoped<IDungeonRecordHelperService, DungeonRecordHelperService>()
            .AddScoped<IDungeonRecordRewardService, DungeonRecordRewardService>()
            .AddScoped<IDungeonRecordDamageService, DungeonRecordDamageService>()
            .AddScoped<IQuestCompletionService, QuestCompletionService>()
            .AddScoped<IAutoRepeatService, AutoRepeatService>()
            .AddScoped<EventDropService>();
    }
}

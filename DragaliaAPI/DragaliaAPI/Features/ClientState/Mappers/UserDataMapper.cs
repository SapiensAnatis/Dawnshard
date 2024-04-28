using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class UserDataMapper
{
    public static UserData ToUserData(this DbPlayerUserData dbModel)
    {
        UserData userData = ToUserDataInternal(dbModel);
        userData.PrologueEndTime = DateTimeOffset.UnixEpoch;
        return userData;
    }

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(UserData.AgeGroup))]
    [MapperIgnoreTarget(nameof(UserData.IsOptin))]
    [MapperIgnoreTarget(nameof(UserData.PrologueEndTime))]
    private static partial UserData ToUserDataInternal(DbPlayerUserData dbModel);
}

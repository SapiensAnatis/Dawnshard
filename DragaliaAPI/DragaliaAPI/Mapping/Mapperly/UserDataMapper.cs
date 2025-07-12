using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class UserDataMapper
{
    public static UserData MapToUserData(this DbPlayerUserData dbModel)
    {
        UserData userData = MapToUserDataInternal(dbModel);
        userData.PrologueEndTime = DateTimeOffset.UnixEpoch;
        return userData;
    }

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(UserData.AgeGroup))]
    [MapperIgnoreTarget(nameof(UserData.IsOptin))]
    [MapperIgnoreTarget(nameof(UserData.PrologueEndTime))]
    private static partial UserData MapToUserDataInternal(DbPlayerUserData dbModel);

    public static DbPlayerUserData MapToDbPlayerUserData(this UserData userData, long viewerId)
    {
        return new DbPlayerUserData()
        {
            ViewerId = viewerId,
            Name = userData.Name,
            Level = userData.Level,
            Exp = userData.Exp,
            Crystal = userData.Crystal,
            Coin = userData.Coin,
            MainPartyNo = userData.MainPartyNo,
            EmblemId = userData.EmblemId,
            ActiveMemoryEventId = userData.ActiveMemoryEventId,
            ManaPoint = userData.ManaPoint,
            DewPoint = userData.DewPoint,
            BuildTimePoint = userData.BuildTimePoint,
            LastLoginTime = userData.LastLoginTime,
            StaminaSingle = userData.StaminaSingle,
            LastStaminaSingleUpdateTime = userData.LastStaminaSingleUpdateTime,
            StaminaSingleSurplusSecond = userData.StaminaSingleSurplusSecond,
            StaminaMulti = userData.StaminaMulti,
            LastStaminaMultiUpdateTime = userData.LastStaminaMultiUpdateTime,
            StaminaMultiSurplusSecond = userData.StaminaMultiSurplusSecond,
            TutorialStatus = userData.TutorialStatus,
            TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(userData.TutorialFlagList),
            FortOpenTime = userData.FortOpenTime,
            CreateTime = userData.CreateTime,
        };
    }
}

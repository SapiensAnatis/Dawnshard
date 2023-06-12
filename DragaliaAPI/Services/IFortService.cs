﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IFortService
{
    Task<IEnumerable<BuildList>> GetBuildList();
    Task<FortDetail> AddCarpenter(PaymentTypes paymentType);
    Task<FortDetail> GetFortDetail();

    Task CompleteAtOnce(PaymentTypes paymentType, long buildId);

    Task<DbFortBuild> CancelUpgrade(long buildId);
    Task EndUpgrade(long buildId);

    Task<DbFortBuild> BuildStart(FortPlants fortPlantId, int positionX, int positionZ);

    Task<DbFortBuild> LevelupStart(long buildId);
    Task<DbFortBuild> Move(long buildId, int afterPositionX, int afterPositionZ);
    Task GetFortPlantIdList(IEnumerable<int> fortPlantIdList);
}

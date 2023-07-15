using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.ClearParty;

public class ClearPartyService : IClearPartyService
{
    private readonly IClearPartyRepository clearPartyRepository;
    private readonly IMapper mapper;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<ClearPartyService> logger;

    public ClearPartyService(
        IClearPartyRepository clearPartyRepository,
        IMapper mapper,
        IPlayerIdentityService playerIdentityService,
        ILogger<ClearPartyService> logger
    )
    {
        this.clearPartyRepository = clearPartyRepository;
        this.mapper = mapper;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public async Task<IEnumerable<PartySettingList>> GetQuestClearParty(int questId, bool isMulti)
    {
        IEnumerable<DbQuestClearPartyUnit> clearPartyUnits =
            await this.clearPartyRepository.GetQuestClearPartyAsync(questId, isMulti);

        this.logger.LogDebug(
            "Retrieved quest clear party for quest {questId}: {@party}",
            questId,
            clearPartyUnits
        );

        // The game seems to gracefully handle receiving an empty list, by disabling the button. How nice!
        return clearPartyUnits.Select(this.mapper.Map<PartySettingList>);
    }

    public void SetQuestClearParty(int questId, bool isMulti, IEnumerable<PartySettingList> party)
    {
        this.logger.LogDebug(
            "Storing quest clear party for quest {questId}: {@party}",
            questId,
            party
        );

        IEnumerable<DbQuestClearPartyUnit> dbUnits = party.Select(
            x =>
                this.mapper.Map<DbQuestClearPartyUnit>(
                    x,
                    opts =>
                        opts.AfterMap(
                            (src, dest) =>
                            {
                                dest.DeviceAccountId = this.playerIdentityService.AccountId;
                                dest.QuestId = questId;
                                dest.IsMulti = isMulti;
                            }
                        )
                )
        );

        this.clearPartyRepository.SetQuestClearParty(questId, isMulti, dbUnits);
    }
}

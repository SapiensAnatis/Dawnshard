using DragaliaAPI.Database;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Features.Web.Savefile;

internal sealed partial class SavefileEditService(
    IPresentService presentService,
    ApiContext apiContext,
    ILogger<SavefileEditService> logger
)
{
    public bool ValidateEdits(SavefileEditRequest request) => ValidatePresents(request.Presents);

    public async Task PerformEdits(SavefileEditRequest request)
    {
        foreach (PresentFormSubmission presentFormSubmission in request.Presents)
        {
            presentService.AddPresent(
                new Present.Present(
                    PresentMessage.DragaliaLostTeamGift, // Could consider adding a new message to the master asset
                    presentFormSubmission.Type,
                    presentFormSubmission.Item,
                    presentFormSubmission.Quantity
                )
            );
        }

        Log.AddedPresents(logger, request.Presents.Count);

        await apiContext.SaveChangesAsync();

        Log.EditSuccessful(logger);
    }

    private bool ValidatePresents(List<PresentFormSubmission> presents)
    {
        if (presents.Count > 100)
        {
            Log.PresentLimitExceeded(logger, presents.Count);
            return false;
        }

        foreach (PresentFormSubmission present in presents)
        {
            if (present.Quantity < 1)
            {
                Log.InvalidSinglePresent(logger, present);
                return false;
            }

            bool idValid = present.Type switch
            {
                EntityTypes.Chara => ValidateCharaPresent(present),
                EntityTypes.Dragon => ValidateDragonPresent(present),
                EntityTypes.Item => ValidateItemPresent(present),
                EntityTypes.Material => ValidateMaterialPresent(present),
                EntityTypes.DmodePoint => ValidateDmodePointPresent(present),
                EntityTypes.SkipTicket => ValidateId0Present(present),
                EntityTypes.DragonGift => ValidateDragonGiftPresent(present),
                EntityTypes.FreeDiamantium => ValidateId0Present(present),
                EntityTypes.Wyrmite => ValidateId0Present(present),
                EntityTypes.HustleHammer => ValidateId0Present(present),
                _ => false,
            };

            if (!idValid)
            {
                Log.InvalidSinglePresent(logger, present);
                return false;
            }
        }

        return true;
    }

    private static bool ValidateCharaPresent(PresentFormSubmission present) =>
        MasterAsset.CharaData.ContainsKey((Charas)present.Item) && present.Quantity == 1;

    private static bool ValidateDragonPresent(PresentFormSubmission present) =>
        MasterAsset.DragonData.ContainsKey((Dragons)present.Item);

    private static bool ValidateItemPresent(PresentFormSubmission present) =>
        MasterAsset.UseItem.ContainsKey((UseItem)present.Item);

    private static bool ValidateMaterialPresent(PresentFormSubmission present) =>
        MasterAsset.MaterialData.ContainsKey((Materials)present.Item);

    private static bool ValidateDmodePointPresent(PresentFormSubmission present) =>
        (DmodePoint)present.Item is DmodePoint.Point1 or DmodePoint.Point2;

    private static bool ValidateId0Present(PresentFormSubmission present) => present.Item == 0;

    private static bool ValidateDragonGiftPresent(PresentFormSubmission present) =>
        Enum.IsDefined((DragonGifts)present.Item);

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Information,
            "Request was invalid: {Count} presents exceeds limit of 100"
        )]
        public static partial void PresentLimitExceeded(ILogger logger, int count);

        [LoggerMessage(LogLevel.Information, "Request was invalid: present {@Present} was invalid")]
        public static partial void InvalidSinglePresent(
            ILogger logger,
            PresentFormSubmission present
        );

        [LoggerMessage(LogLevel.Information, "Added {Count} presents to the gift box")]
        public static partial void AddedPresents(ILogger logger, int count);

        [LoggerMessage(LogLevel.Information, "Savefile edited successfully")]
        public static partial void EditSuccessful(ILogger logger);
    }
}

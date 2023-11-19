using DragaliaAPI.Shared.PlayerDetails;
using Moq;

namespace DragaliaAPI.Test.Utils;

public static class IdentityTestUtils
{
    public const string DeviceAccountId = "id";
    public const long ViewerId = 1;

    public static readonly Mock<IPlayerIdentityService> MockPlayerDetailsService;

    static IdentityTestUtils()
    {
        MockPlayerDetailsService = new(MockBehavior.Loose);
        MockPlayerDetailsService.SetupGet(x => x.AccountId).Returns(DeviceAccountId);
        MockPlayerDetailsService.SetupGet(x => x.ViewerId).Returns(ViewerId);
    }
}

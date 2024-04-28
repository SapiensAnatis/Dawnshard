using DragaliaAPI.Features.Shared.Models.Generated;
using DragaliaAPI.Infrastructure.Results;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("option")]
public class OptionController : DragaliaControllerBase
{
    private static class StubData
    {
        public static OptionGetOptionResponse OptionData =
            new()
            {
                OptionData = new()
                {
                    IsEnableAutoLockUnit = false,
                    IsAutoLockAmuletSr = false,
                    IsAutoLockAmuletSsr = false,
                    IsAutoLockDragonSr = false,
                    IsAutoLockDragonSsr = false,
                    IsAutoLockWeaponSr = false,
                    IsAutoLockWeaponSsr = false,
                    IsAutoLockWeaponSssr = false,
                }
            };
    }

    [HttpPost("get_option")]
    public DragaliaResult GetOption()
    {
        return this.Ok(StubData.OptionData);
    }

    [HttpPost("set_option")]
    public DragaliaResult SetOption()
    {
        return this.Ok(StubData.OptionData);
    }
}

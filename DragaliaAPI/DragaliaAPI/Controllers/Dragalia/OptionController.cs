using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("option")]
public class OptionController : DragaliaControllerBase
{
    private static class StubData
    {
        public static OptionGetOptionData OptionData =
            new()
            {
                option_data = new()
                {
                    is_enable_auto_lock_unit = false,
                    is_auto_lock_amulet_sr = false,
                    is_auto_lock_amulet_ssr = false,
                    is_auto_lock_dragon_sr = false,
                    is_auto_lock_dragon_ssr = false,
                    is_auto_lock_weapon_sr = false,
                    is_auto_lock_weapon_ssr = false,
                    is_auto_lock_weapon_sssr = false,
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

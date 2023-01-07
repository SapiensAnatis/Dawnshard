using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("dragon")]
public class DragonController : DragaliaControllerBase
{
    [HttpPost("get_contact_data")]
    public DragaliaResult GetContactData(DragonGetContactDataRequest request)
    {
        return this.Ok(new DragonGetContactDataData() { shop_gift_list = StubData.GiftList });
    }

    private static class StubData
    {
        public static IEnumerable<AtgenShopGiftList> GiftList = new int[]
        {
            10001,
            10002,
            10003,
            10004,
            20005,
        }.Select(
            x =>
                new AtgenShopGiftList()
                {
                    dragon_gift_id = x,
                    is_buy = 1,
                    price = 400
                }
        );
    }
}

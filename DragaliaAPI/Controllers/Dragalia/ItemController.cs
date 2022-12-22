using AutoMapper;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("item")]
public class ItemController : DragaliaControllerBase
{
    private readonly IInventoryRepository inventoryRepository;
    private readonly IMapper mapper;

    public ItemController(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        this.inventoryRepository = inventoryRepository;
        this.mapper = mapper;
    }

    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetList()
    {
        IEnumerable<ItemList> itemList = this.inventoryRepository
            .GetMaterials(this.DeviceAccountId)
            .Select(mapper.Map<ItemList>);

        return this.Ok(new ItemGetListData() { item_list = itemList });
    }
}

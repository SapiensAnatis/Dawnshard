using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Talisman;

public interface ITalismanService
{
    Task<DeleteDataList> SellTalismans(IEnumerable<long> talismanIds);
    Task SetLock(long talismanId, bool locked);
}

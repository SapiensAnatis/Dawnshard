using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Login;

public interface ILoginBonusRepository
{
    Task<DbLoginBonus> Get(int id);
}

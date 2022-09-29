namespace DragaliaAPI.Models.Database
{
    public interface IApiRepository
    {
        Task AddNewDeviceAccount(string id, string hashedPassword);
        Task<DbDeviceAccount?> GetDeviceAccountById(string id);
        Task AddNewPlayerSavefile(string deviceAccountId);
        Task<DbPlayerSavefile> GetSavefileByDeviceAccountId(string deviceAccountId);
    }
}
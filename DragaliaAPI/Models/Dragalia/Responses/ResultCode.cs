namespace DragaliaAPI.Models.Dragalia.Responses
{
    public enum ResultCode
    {
        Success = 1,
        Maintenance = 101,
        ServerError = 102,
        SessionError = 201,
        AccountBlockError = 203,
        ApiVersionError = 204,
        ProcessedError = 213,
        MaintenanceFrom = 2000,
        MaintenanceTo = 2999
    }
}

namespace DragaliaAPI.Models.Nintendo
{
    public interface ILoginService
    {
        /// <summary>
        /// Create a login response for the given device account.
        /// Will check username and password against the DB.
        /// </summary>
        /// <param name="deviceAccount">The device account to log in to.</param>
        /// <returns>A LoginResponse object.</returns>
        Task<LoginResponse> Login(DeviceAccount deviceAccount);

        /// <summary>
        /// Create a new device account and register it against the DB.
        /// </summary>
        /// <returns>The created DeviceAccount.</returns>
        Task<DeviceAccount> DeviceAccountFactory();
    }
}
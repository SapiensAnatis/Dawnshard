namespace DragaliaAPI.Models.Nintendo
{
    public interface ILoginFactory
    {
        /// <summary>
        /// Create a login response without a device account being passed.
        /// Will create a new DeviceAccount and register that against the DB.
        /// </summary>
        /// <returns>A LoginResponse object.</returns>
        LoginResponse LoginResponseFactory();

        /// <summary>
        /// Create a login response for the given device account.
        /// Will check username and password against the DB.
        /// </summary>
        /// <param name="deviceAccount">The device account to log in to.</param>
        /// <returns>A LoginResponse object.</returns>
        LoginResponse LoginResponseFactory(DeviceAccount deviceAccount);

        /// <summary>
        /// Create a new device account and register it against the DB.
        /// </summary>
        /// <returns>The created DeviceAccount.</returns>
        DeviceAccount DeviceAccountFactory();
    }
}
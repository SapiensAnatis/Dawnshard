using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Services;

public class DeviceAccountService : IDeviceAccountService
{
    private readonly IDeviceAccountRepository deviceAccountRepository;
    private readonly IConfiguration configuration;
    private readonly ILogger<DeviceAccountService> logger;

    public DeviceAccountService(
        IDeviceAccountRepository repository,
        IConfiguration configuration,
        ILogger<DeviceAccountService> logger
    )
    {
        this.deviceAccountRepository = repository;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount)
    {
        // TODO: If the user attempts to connect with an id that is not in the database,
        // just create it for them instead of returning unauthorized. Use case: for when
        // I nuke the DB after fucking up a migration and the app keeps my old credentials

        if (deviceAccount.password is null)
        {
            throw new ArgumentNullException(paramName: deviceAccount.password);
        }

        DbDeviceAccount? dbDeviceAccount = await this.deviceAccountRepository.GetDeviceAccountById(
            deviceAccount.id
        );

        if (dbDeviceAccount is null)
            return false;

        return this.GetHashedPassword(deviceAccount.password) == dbDeviceAccount.HashedPassword;
    }

    public async Task<DeviceAccount> RegisterDeviceAccount()
    {
        string id = GenerateRandomString(16);
        string password = GenerateRandomString(40);
        string hashedPassword = this.GetHashedPassword(password);

        await this.deviceAccountRepository.AddNewDeviceAccount(id, hashedPassword);
        await this.deviceAccountRepository.CreateNewSavefile(id);
        await this.deviceAccountRepository.SaveChangesAsync();

        this.logger.LogInformation("Registered new account: DeviceAccount ID '{id}'", id);

        return new DeviceAccount(id, password);
    }

    private string GetHashedPassword(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        // Dynamic salt would be better.
        // But security is not a top priority for this application; it is unlikely to be publically hosted for mass use.
        byte[] saltBytes = Encoding.UTF8.GetBytes(
            this.configuration.GetValue<string>("HashSalt")
                ?? throw new NullReferenceException("Could not find salt from configuration file")
        );

        Rfc2898DeriveBytes pkbdf2 = new(passwordBytes, saltBytes, 10000, HashAlgorithmName.SHA256);
        byte[] hashBytes = pkbdf2.GetBytes(20);

        return Convert.ToBase64String(hashBytes);
    }

    private static string GenerateRandomString(int nChars)
    {
        // Not a great idea to use the standard RNG for making passwords, but again, security is not a big deal
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] stringChars = new char[nChars];
        Random random = new();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }
}

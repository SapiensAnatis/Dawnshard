using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Services.Game;

[Obsolete(ObsoleteReasons.BaaS)]
public class DeviceAccountService : IDeviceAccountService
{
    private readonly IDeviceAccountRepository deviceAccountRepository;
    private readonly ISavefileService savefileService;
    private readonly IConfiguration configuration;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<DeviceAccountService> logger;

    public DeviceAccountService(
        IDeviceAccountRepository repository,
        ISavefileService savefileService,
        IConfiguration configuration,
        IPlayerIdentityService playerIdentityService,
        ILogger<DeviceAccountService> logger
    )
    {
        this.deviceAccountRepository = repository;
        this.savefileService = savefileService;
        this.configuration = configuration;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public async Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount)
    {
        if (deviceAccount.password is null)
            throw new ArgumentNullException(paramName: deviceAccount.password);

        DbDeviceAccount? dbDeviceAccount = await this.deviceAccountRepository.GetDeviceAccountById(
            deviceAccount.id
        );

        if (dbDeviceAccount is null)
        {
            this.logger.LogInformation(
                "Foreign device account ID '{id}' received",
                deviceAccount.id
            );
            await this.RegisterDeviceAccount(deviceAccount.id, deviceAccount.password);
            return true;
        }
        else
        {
            return this.GetHashedPassword(deviceAccount.password) == dbDeviceAccount.HashedPassword;
        }
    }

    public async Task<DeviceAccount> RegisterDeviceAccount()
    {
        string id = GenerateRandomString(16);
        string password = GenerateRandomString(40);

        return await this.RegisterDeviceAccount(id, password);
    }

    private async Task<DeviceAccount> RegisterDeviceAccount(string id, string password)
    {
        if (await this.deviceAccountRepository.GetDeviceAccountById(id) is not null)
        {
            throw new ArgumentException(
                $"Could not register device account: ID {id} already exists"
            );
        }

        string hashedPassword = this.GetHashedPassword(password);

        await this.deviceAccountRepository.AddNewDeviceAccount(id, hashedPassword);
        using (IDisposable ctx = this.playerIdentityService.StartUserImpersonation(id))
        {
            await this.savefileService.Create();
        }

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

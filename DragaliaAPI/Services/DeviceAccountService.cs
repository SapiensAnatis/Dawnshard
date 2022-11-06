using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Services;

public class DeviceAccountService : IDeviceAccountService
{
    private readonly IDeviceAccountRepository _apiRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeviceAccountService> _logger;

    public DeviceAccountService(
        IDeviceAccountRepository repository,
        IConfiguration configuration,
        ILogger<DeviceAccountService> logger
    )
    {
        _apiRepository = repository;
        _configuration = configuration;
        _logger = logger;
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

        DbDeviceAccount? dbDeviceAccount = await _apiRepository.GetDeviceAccountById(
            deviceAccount.id
        );
        if (dbDeviceAccount is null)
            return false;

        string hashedPassword = GetHashedPassword(deviceAccount.password);

        return hashedPassword == dbDeviceAccount.HashedPassword;
    }

    public async Task<DeviceAccount> RegisterDeviceAccount()
    {
        string id = GenerateRandomString(16);
        string password = GenerateRandomString(40);
        string hashedPassword = GetHashedPassword(password);

        await _apiRepository.AddNewDeviceAccount(id, hashedPassword);
        await _apiRepository.CreateNewSavefile(id);

        _logger.LogInformation("Registered new account: DeviceAccount ID '{id}'", id);

        return new DeviceAccount(id, password);
    }

    private string GetHashedPassword(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        // Dynamic salt would be better.
        // But security is not a top priority for this application; it is unlikely to be publically hosted for mass use.
        byte[] saltBytes = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("HashSalt"));

        var pkbdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000);
        byte[] hashBytes = pkbdf2.GetBytes(20);

        return Convert.ToBase64String(hashBytes);
    }

    private string GenerateRandomString(int nChars)
    {
        // Not a great idea to use the standard RNG for making passwords, but again, security is not a big deal
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[nChars];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }
}

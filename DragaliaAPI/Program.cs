using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Models;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc().AddMvcOptions(option =>
{
    option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
    option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetValue<string>("RedisConnection"); // redis is the container name of the redis service. 6379 is the default port
    options.InstanceName = "DragaliaAPI";
});

builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IDeviceAccountService, DeviceAccountService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

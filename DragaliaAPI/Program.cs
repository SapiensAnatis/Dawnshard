using DragaliaAPI.Models;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc().AddMvcOptions(option =>
{
    option.OutputFormatters.Add(new DragaliaAPI.CustomMessagePackOutputFormatter(ContractlessStandardResolver.Options));
    option.InputFormatters.Add(new DragaliaAPI.CustomMessagePackInputFormatter(ContractlessStandardResolver.Options));
}).AddJsonOptions(option =>
    option.JsonSerializerOptions.IncludeFields = true
);

builder.Services.AddDbContext<DeviceAccountContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));


builder.Services
    .AddSingleton<ISessionService, SessionService>()
    .AddScoped<IDeviceAccountService, DeviceAccountService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

/*
app.Use(async (context, next) =>
{
    if (context.Request.ContentType == "application/octet-stream")
        context.Request.ContentType = "application/x-msgpack";

    await next(context);
}).Use(async (context, next) =>
{
    if (context.Response.ContentType == "application/x-msgpack")
        context.Response.ContentType = "application/octet-stream";

    await next(context);
});
*/

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
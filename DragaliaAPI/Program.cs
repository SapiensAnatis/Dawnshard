using DragaliaAPI.Models;
using DragaliaAPI.Models.Database;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc().AddMvcOptions(option =>
{
    // Must use ContractlessResolver because the DefaultResolver doesn't like serializing the generic BaseResponse<T> 
    // record, even when it is properly annotated with the MessagePackObject decorator.
    option.OutputFormatters.Add(new DragaliaAPI.CustomMessagePackOutputFormatter(ContractlessStandardResolver.Options));
    option.InputFormatters.Add(new DragaliaAPI.CustomMessagePackInputFormatter(ContractlessStandardResolver.Options));
});

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")), ServiceLifetime.Transient, ServiceLifetime.Transient);


builder.Services
    .AddSingleton<ISessionService, SessionService>()
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
    .AddTransient<IApiRepository, ApiRepository>();

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Migrating database...");

    try
    {
        serviceScope.ServiceProvider.GetRequiredService<ApiContext>().Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
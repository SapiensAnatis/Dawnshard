using DragaliaAPI.Models;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc().AddMvcOptions(option =>
{
    option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
    option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options));
}).AddJsonOptions(option =>
    option.JsonSerializerOptions.IncludeFields = true
);

builder.Services.AddDbContext<DeviceAccountContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));


builder.Services.AddSingleton<ISessionService, SessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
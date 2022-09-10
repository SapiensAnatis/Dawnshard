using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Models;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc().AddMvcOptions(option =>
{
    option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
    option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options));
});
builder.Services.AddSingleton<ILoginService, LoginService>();
builder.Services.AddSingleton<ISessionService, SessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Reflection;
using DragaliaAPI.Photon.StateManager.Models;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Host.UseSerilog(
    (context, config) =>
    {
        config.WriteTo.Console();
        config.MinimumLevel.Debug();
        config.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
    }
);

builder.Services
    .AddOptions<RedisOptions>()
    .BindConfiguration(nameof(RedisOptions))
    .Validate(x => x.KeyExpiryTimeMins > 0)
    .ValidateOnStart();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc(
        "v1",
        new()
        {
            Version = "v1",
            Title = "Photon State Manager",
            Description = "API for storing room state received from Photon webhooks."
        }
    );

    config.IncludeXmlComments(
        Path.Join(AppContext.BaseDirectory, "DragaliaAPI.Photon.StateManager.xml"),
        includeControllerXmlComments: true
    );

    config.IncludeXmlComments(Path.Join(AppContext.BaseDirectory, "DragaliaAPI.Photon.Dto.xml"));
});

ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(
    builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Missing required Redis connection string!")
);

builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Reflection;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Redis;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Host.UseSerilog(
    (context, config) =>
    {
        config.WriteTo.Console();

        SeqOptions seqOptions =
            builder.Configuration.GetSection(nameof(SeqOptions)).Get<SeqOptions>()
            ?? throw new NullReferenceException("Failed to get seq config!");

        if (seqOptions.Enabled)
            config.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.Key);

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

builder.Services
    .AddSingleton<IConnectionMultiplexer>(multiplexer)
    .AddScoped<IRedisService, RedisService>();

NReJSON.NReJSONSerializer.SerializerProxy = new SerializerProxy();

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

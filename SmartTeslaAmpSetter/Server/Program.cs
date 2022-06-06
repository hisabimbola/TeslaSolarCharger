using MQTTnet;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using SmartTeslaAmpSetter.Server.Contracts;
using SmartTeslaAmpSetter.Server.Scheduling;
using SmartTeslaAmpSetter.Server.Services;
using SmartTeslaAmpSetter.Server.Wrappers;
using SmartTeslaAmpSetter.Shared.Dtos;
using SmartTeslaAmpSetter.Shared.Dtos.Contracts;
using SmartTeslaAmpSetter.Shared.Dtos.Settings;
using SmartTeslaAmpSetter.Shared.TimeProviding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mqttFactory = new MqttFactory();
var mqttClient = mqttFactory.CreateMqttClient();


builder.Services
    .AddTransient<JobManager>()
    .AddTransient<ChargingValueJob>()
    .AddTransient<ConfigJsonUpdateJob>()
    .AddTransient<ChargeTimeUpdateJob>()
    .AddTransient<PvValueJob>()
    .AddTransient<JobFactory>()
    .AddTransient<IJobFactory, JobFactory>()
    .AddTransient<ISchedulerFactory, StdSchedulerFactory>()
    .AddTransient<IChargingService, ChargingService>()
    .AddTransient<IGridService, GridService>()
    .AddTransient<IConfigService, ConfigService>()
    .AddTransient<IConfigJsonService, ConfigJsonService>()
    .AddTransient<IDateTimeProvider, DateTimeProvider>()
    .AddTransient<IChargeTimeUpdateService, ChargeTimeUpdateService>()
    .AddTransient<ITelegramService, TelegramService>()
    .AddTransient<ITeslaService, TeslamateApiService>()
    .AddSingleton<ISettings, Settings>()
    .AddSingleton<IInMemoryValues, InMemoryValues>()
    .AddSingleton<IConfigurationWrapper, ConfigurationWrapper>()
    .AddSingleton(mqttClient)
    .AddTransient<MqttFactory>()
    .AddTransient<IMqttService, MqttService>()
    .AddTransient<IPvValueService, PvValueService>()
    ;

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (environment == "Development")
{
    builder.Configuration.AddJsonFile("appsettings.Development.json");
}


var app = builder.Build();

var telegramService = app.Services.GetRequiredService<ITelegramService>();
await telegramService.SendMessage("Application starting up");

var configJsonService = app.Services.GetRequiredService<IConfigJsonService>();

await configJsonService.AddCarIdsToSettings().ConfigureAwait(false);

var mqttHelper = app.Services.GetRequiredService<IMqttService>();

await mqttHelper.ConfigureMqttClient().ConfigureAwait(false);

var jobManager = app.Services.GetRequiredService<JobManager>();
jobManager.StartJobs();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();





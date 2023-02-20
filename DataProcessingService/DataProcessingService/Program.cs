using DataProcessingService;
using DataProcessingService.Interfaces;
using DataProcessingService.ServiceConfig;
using DataProcessingService.Services;
using Serilog;

var logPath = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logPath, "Logs", "log.txt"))
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        DataProcessingConfig options = configuration.GetSection(nameof(DataProcessingConfig)).Get<DataProcessingConfig>();

        services.AddSingleton(options);
        services.AddSingleton<IReadData, ReadDataService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

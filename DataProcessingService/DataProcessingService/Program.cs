using DataProcessingService;
using DataProcessingService.Interfaces;
using DataProcessingService.ServiceConfig;
using DataProcessingService.Services;
using Serilog;

//var logPath = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
var logpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logpath, "DataProcessingLogs", "log.txt"))
    .CreateLogger();

try
{
    IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        try
        {
            DataProcessingConfig options = configuration.GetSection(nameof(DataProcessingConfig)).Get<DataProcessingConfig>();

            services.AddSingleton(options);
        }
        catch (Exception ex)
        {
            Log.Fatal("Options went kaput");
        }

        services.AddSingleton<IReadData, ReadDataService>();
        services.AddSingleton<IPostData, GetPostDataService>();
        services.AddHostedService<Worker>();
    })
    .Build();

    await host.RunAsync();
} catch(Exception ex)
{
    Log.Fatal("No options availiable");

    return;
}

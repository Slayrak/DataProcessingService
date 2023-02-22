using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using DataProcessingService.ServiceConfig;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DataProcessingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DataProcessingConfig _options;
        private readonly IReadData _readData;
        private readonly IPostData _postData;
        private string metaLogPath;
        private MetaLogModel _logModel;
        private IHostApplicationLifetime _applicationLifetime;


        public Worker(ILogger<Worker> logger, DataProcessingConfig dataProcessingConfig, IReadData readData, IPostData postData, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _options = dataProcessingConfig;
            _readData = readData;
            _postData = postData;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var check = Directory.GetFiles(_options.InputDirectory);

                for (int i = 0; i < check.Length; i++)
                {
                    _logger.LogInformation($"Proceeding with file: {check[i]}" , DateTimeOffset.Now);

                    if (Path.GetExtension(check[0]) == ".txt" || Path.GetExtension(check[0]) == ".csv")
                    {
                        int smt = 0;

                        var result = new List<ReadDataModel>(); 
                            
                        var tuple = await _readData.ReadFile(check[i], result, smt);

                        _logger.LogInformation($"Parced file: {check[i]}", DateTimeOffset.Now);

                        result = tuple.Item1;

                        smt = tuple.Item2;

                        List<PostDataModel> posts = new List<PostDataModel>();

                        _postData.Categorize(result, posts);

                        var data = JsonConvert.SerializeObject(posts, Formatting.Indented);

                        _logger.LogInformation($"Converted data", DateTimeOffset.Now);

                        var directories = Directory.GetDirectories(_options.OutputDirectory);

                        directories = directories.Select(x => x.Substring(_options.OutputDirectory.Length + 1)).ToArray();

                        if (directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
                        {
                            var number = Directory.GetFiles(Path.Combine(_options.OutputDirectory,DateTime.Now.ToString("MM-dd-yyyy")));

                            var files = number.Select(x => x).Where(x => x.Contains("output")).ToList();

                            int orderNumber = files.Count + 1;
                            using (FileStream fs = File.Create(Path.Combine(_options.OutputDirectory, DateTime.Now.ToString("MM-dd-yyyy"), $"output{orderNumber}.json")))
                            {
                                byte[] info = new UTF8Encoding(true).GetBytes(data);

                                fs.Write(info, 0, info.Length);

                                _logger.LogInformation($"Record is written", DateTimeOffset.Now);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.Combine(_options.OutputDirectory, DateTime.Now.ToString("MM-dd-yyyy")));

                            using(FileStream fs = File.Create(Path.Combine(_options.OutputDirectory, DateTime.Now.ToString("MM-dd-yyyy"), "output1.json")))
                            {
                                byte[] info = new UTF8Encoding(true).GetBytes(data);

                                fs.Write(info, 0, info.Length);

                                _logger.LogInformation($"Record is written", DateTimeOffset.Now);
                            }

                            metaLogPath = Path.Combine(_options.OutputDirectory, DateTime.Now.ToString("MM-dd-yyyy"), "meta.log");
                            using (File.Create(metaLogPath))

                            _logModel = new MetaLogModel();
                        }

                        if(Path.GetExtension(check[0]) == ".csv")
                        {
                            smt -= 1;
                        }

                        if (result.Count != smt)
                        {
                            _logModel.invalid_files.Add(check[i]);
                            _logModel.found_errors += smt - result.Count;
                        }

                        _logModel.parsed_files += 1;
                        _logModel.parced_lines += smt;

                        var metadata = JsonConvert.SerializeObject(_logModel, Formatting.Indented);

                        using (FileStream fs = File.OpenWrite(metaLogPath))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(metadata);

                            fs.Write(info, 0, info.Length);

                            _logger.LogInformation($"meta.log recorded", DateTimeOffset.Now);
                        }

                        File.Delete(check[i]);

                        _logger.LogInformation($"{check[i]} deleted", DateTimeOffset.Now);

                    } 
                    else
                    {
                        continue;
                    }
                }
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            _logger.LogInformation("Service is starting", DateTimeOffset.Now);

            var directories = Directory.GetDirectories(_options.OutputDirectory);

            directories = directories.Select(x => x.Substring(_options.OutputDirectory.Length + 1)).ToArray();

            if (directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
            {

                var test = Path.Combine(_options.OutputDirectory, DateTime.Now.ToString("MM-dd-yyyy"));

                var number = Directory.GetFiles(test);

                var files = number.Select(x => x).Where(x => x.Contains("meta.log")).ToList();

                if(files.Count != 0)
                {
                    metaLogPath = files[0];

                    var meta = File.ReadAllText(metaLogPath);

                    _logModel = JsonConvert.DeserializeObject<MetaLogModel>(meta);

                    if(_logModel == null)
                    {
                        _logModel= new MetaLogModel();
                    }

                } else
                {
                    using (File.Create(Path.Combine(_options.OutputDirectory, DateTime.Now.ToString("MM-dd-yyyy"), "meta.log")))

                    metaLogPath = Path.Combine(_options.OutputDirectory, DateTime.Now.ToString("MM-dd-yyyy"), "meta.log");

                    _logModel = new MetaLogModel();
                }
            }
            else
            {
                _logModel = new MetaLogModel();
            }

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

            _logger.LogInformation("Shutting down service");

            _applicationLifetime.StopApplication();
            return base.StopAsync(cancellationToken);
        }
    }
}
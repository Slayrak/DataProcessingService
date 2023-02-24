using DataProcessingService.Helpers;
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
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var check = Directory.GetFiles(_options.InputDirectory);

                    for (int i = 0; i < check.Length; i++)
                    {
                        if (IsFileLockedHelper.IsFileLocked(new FileInfo(check[i])))
                        {
                            continue;
                        }

                        if (Path.GetExtension(check[0]) == ".txt" || Path.GetExtension(check[0]) == ".csv")
                        {
                            _logger.LogInformation($"Proceeding with file: {check[i]}", DateTimeOffset.Now);

                            int smt = 0;
                            var result = new List<ReadDataModel>();
                            var tuple = await _readData.ReadFile(check[i], result);

                            _logger.LogInformation($"Parced file: {check[i]}", DateTimeOffset.Now);

                            result = tuple.Item1;
                            smt = tuple.Item2;

                            List<PostDataModel> posts = new List<PostDataModel>();
                            _postData.Categorize(result, posts);
                            var data = JsonConvert.SerializeObject(posts, Formatting.Indented);

                            _logger.LogInformation($"Converted data", DateTimeOffset.Now);

                            CreateDirectoryHelper.CreateAfterMidnight(_options.OutputDirectory, data, _logger, ref metaLogPath, ref _logModel);

                            if (Path.GetExtension(check[i]) == ".csv")
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
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                _applicationLifetime.StopApplication();
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            if(!Directory.Exists(_options.InputDirectory))
            {
                Directory.CreateDirectory(_options.InputDirectory);

            }
            
            if(!Directory.Exists(_options.OutputDirectory))
            {
                Directory.CreateDirectory(_options.OutputDirectory);
            }

            _logger.LogInformation("Service is starting", DateTimeOffset.Now);

            _logger.LogInformation("Starting from: " + Directory.GetCurrentDirectory());

            CreateDirectoryHelper.CreateOnInit(ref metaLogPath, _options.OutputDirectory, ref _logModel);

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
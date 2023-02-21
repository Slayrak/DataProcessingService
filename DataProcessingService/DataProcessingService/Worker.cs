using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using DataProcessingService.ServiceConfig;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography.X509Certificates;

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


        public Worker(ILogger<Worker> logger, DataProcessingConfig dataProcessingConfig, IReadData readData, IPostData postData)
        {
            _logger = logger;
            _options = dataProcessingConfig;
            _readData = readData;
            _postData = postData;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var check = Directory.GetFiles(_options.InputDirectory);

                for (int i = 0; i < check.Length; i++)
                {
                    if (Path.GetExtension(check[0]) == ".txt")
                    {
                        int smt;

                        var result = _readData.ReadFile(check[i], out smt);
                        List<PostDataModel> posts = new List<PostDataModel>();

                        _postData.Categorize(result, posts);

                        var data = JsonConvert.SerializeObject(posts, Formatting.Indented);

                        var directories = Directory.GetDirectories(_options.OutputDirectory);

                        directories = directories.Select(x => x.Substring(_options.OutputDirectory.Length + 1)).ToArray();

                        if (directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
                        {
                            var number = Directory.GetFiles(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy"));

                            if (number.Length != 0)
                            {
                                var files = number.Select(x => x).Where(x => x.Contains("output")).ToList();

                                int orderNumber = files.Count + 1;
                                File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + $"output{orderNumber}.json");

                                File.WriteAllText(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + $"output{orderNumber}.json", data);
                            }
                            else
                            {
                                File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json");

                                File.WriteAllText(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json", data);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(_options.OutputDirectory + '/' + DateTime.Now.ToString("MM-dd-yyyy"));

                            File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json");

                            File.WriteAllText(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json", data);
                            File.WriteAllText(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "meta.log", data);

                            metaLogPath = _options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "meta.log";
                            File.Create(metaLogPath);

                            _logModel = new MetaLogModel();
                        }

                        if (result.Count != smt)
                        {
                            _logModel.invalid_files.Add(check[i]);
                            _logModel.found_errors += smt - result.Count;
                        }

                        _logModel.parsed_files += 1;
                        _logModel.parced_lines += smt;

                        var metadata = JsonConvert.SerializeObject(_logModel, Formatting.Indented);

                        File.WriteAllText(metaLogPath, metadata);

                        File.Delete(check[i]);

                    } else if (Path.GetExtension(check[0]) == ".csv")
                    {
                        int smt;
                        var result = _readData.ReadFile(check[i], out smt);

                        smt -= 1;

                        List<PostDataModel> posts = new List<PostDataModel>();

                        _postData.Categorize(result, posts);

                        var data = JsonConvert.SerializeObject(posts, Formatting.Indented);

                        var directories = Directory.GetDirectories(_options.OutputDirectory);

                        directories = directories.Select(x => x.Substring(_options.OutputDirectory.Length + 1)).ToArray();

                        if (directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
                        {
                            var number = Directory.GetFiles(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy"));

                            if (number.Length != 0)
                            {
                                var files = number.Select(x => x).Where(x => x.Contains("output")).ToList();

                                int orderNumber = files.Count + 1;
                                File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + $"output{orderNumber}.json");

                                File.WriteAllText(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + $"output{orderNumber}.json", data);
                            }
                            else
                            {
                                File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json");

                                File.WriteAllText(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json", data);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(_options.OutputDirectory + '/' + DateTime.Now.ToString("MM-dd-yyyy"));

                            File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json");

                            File.WriteAllText(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json", data);

                            metaLogPath = _options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "meta.log";

                            File.Create(metaLogPath);

                            _logModel = new MetaLogModel();
                        }

                        if (result.Count != smt)
                        {
                            _logModel.invalid_files.Add(check[i]);
                            _logModel.found_errors += smt - result.Count;
                        }

                        _logModel.parsed_files += 1;
                        _logModel.parced_lines += smt;

                        var metadata = JsonConvert.SerializeObject(_logModel, Formatting.Indented);
                        
                        File.WriteAllText(metaLogPath, metadata);

                        File.Delete(check[i]);

                    } else
                    {
                        continue;
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //var check = Directory.GetFiles(_options.InputDirectory, "*.txt");
            //string[] lines = System.IO.File.ReadAllLines(check[0]);

            //int smt;
            //var result = _readData.ReadFile(check[0], out smt);

            //List<PostDataModel> posts = new List<PostDataModel>();

            //_postData.Categorize(result, posts);

            //var data = JsonConvert.SerializeObject(posts, Formatting.Indented);

            //File.WriteAllText(_options.OutputDirectory + "/check.txt", data);

            //var directories = Directory.GetDirectories(_options.OutputDirectory);

            //directories = directories.Select(x => x.Substring(_options.OutputDirectory.Length + 1)).ToArray();

            ////File.Delete(check[0]);

            //if(directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
            //{
            //    var number = Directory.GetFiles(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy"));

            //    if (number.Length != 0)
            //    {
            //        var files = number.Select(x => x).Where(x => x.Contains("output")).ToList();

            //        int orderNumber = files.Count + 1;
            //        File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + $"output{orderNumber}.json");
            //    }
            //    else
            //    {
            //        File.Create(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/" + "output1.json");
            //    }
            //}
            //else
            //{
            //    Directory.CreateDirectory(_options.OutputDirectory + '/' + DateTime.Now.ToString("MM-dd-yyyy"));
            //}

            //List<string> result = new List<string>();

            //string toAdd = "";

            //for (int i = 0; i < lines[0].Length; i++)
            //{
            //    //|| lines[0][i] != '”' || lines[0][i] != '“'

            //    if (!lines[0][i].Equals('"') && lines[0][i] != '”' && lines[0][i] != '“')
            //    {
            //        toAdd += lines[0][i];
            //    } else
            //    {
            //        result.Add(toAdd);
            //        toAdd = "";
            //    }
            //}
            //result.Add(toAdd);

            //List<string> output = new List<string>();

            //for (int i = 0; i < result.Count; i++)
            //{
            //    string[] res;

            //    if(i == 1)
            //    {
            //        output.Add(result[i].Trim().Split(',')[0]);
            //    } else
            //    {
            //        res = result[i].Trim().Split(',');
            //        output.AddRange(res);
            //    }
            //}

            //output.RemoveAll(x => x == "");

            //if(output.Count != 7)
            //{
            //    var checkbool = false;
            //} else
            //{
            //    var checkbool = true;
            //    ReadDataModel record = new ReadDataModel();



            //    decimal payment;
            //    DateTime date;
            //    long acc;

            //    if (checkbool = Decimal.TryParse(output[3], out payment))
            //    {
            //        record.Payment = payment;
            //    } 
            //    else
            //    {
            //        checkbool = false;
            //        //return;
            //    }

            //    if(checkbool = DateTime.TryParse(output[4], out date))
            //    {
            //        record.DateTime = date.Date;
            //    } 
            //    else
            //    {
            //        checkbool = false;
            //        //return;
            //    }

            //    if (checkbool = long.TryParse(output[5], out acc))
            //    {
            //        record.AccountNumber = acc;
            //    } 
            //    else
            //    {
            //        checkbool = false;
            //        //return;
            //    }

            //    record.Name = output[0] + output[1];
            //    record.City = output[2];
            //    record.Service = output[6];

            //}

            var directories = Directory.GetDirectories(_options.OutputDirectory);

            directories = directories.Select(x => x.Substring(_options.OutputDirectory.Length + 1)).ToArray();

            if (directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
            {
                var number = Directory.GetFiles(_options.OutputDirectory + "/" + DateTime.Now.ToString("MM-dd-yyyy"));

                var files = number.Select(x => x).Where(x => x.Contains("meta.log")).ToList();

                metaLogPath = files[0];

                var meta = File.ReadAllText(metaLogPath);

                _logModel = JsonConvert.DeserializeObject<MetaLogModel>(meta);

            }
            else
            {
                _logModel = new MetaLogModel();
            }

            return base.StartAsync(cancellationToken);
        }
    }
}
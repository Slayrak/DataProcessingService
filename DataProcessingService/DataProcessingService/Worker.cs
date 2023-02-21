using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using DataProcessingService.ServiceConfig;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace DataProcessingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DataProcessingConfig _options;
        private readonly IReadData _readData;

        public Worker(ILogger<Worker> logger, DataProcessingConfig dataProcessingConfig, IReadData readData)
        {
            _logger = logger;
            _options = dataProcessingConfig;
            _readData = readData;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var check = Directory.GetFiles(_options.InputDirectory, "*.txt");

                for (int i = 0; i < check.Length; i++)
                {

                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var check = Directory.GetFiles(_options.InputDirectory, "*.txt");
            string[] lines = System.IO.File.ReadAllLines(check[0]);

            var result = _readData.ReadFile(check[0]);

            List<PostDataModel> posts = new List<PostDataModel>();

            if(result.Count != 0)
            {
                Dictionary<string, Dictionary<string, ServiceModel>> map = new Dictionary<string, Dictionary<string, ServiceModel>>();

                Dictionary<string, decimal> totalForCityDict= new Dictionary<string, decimal>();

                decimal totalForCity = 0;

                foreach(var item in result)
                {
                    if(map.ContainsKey(item.City))
                    {
                        var inner = new Dictionary<string, ServiceModel>();

                        if(map.TryGetValue(item.City, out inner))
                        {
                            if(inner.ContainsKey(item.Service))
                            {
                                var innerItem = inner.GetValueOrDefault(item.Service);

                                innerItem.Payers.Add(new PayerModel { 
                                    AccountNumber= item.AccountNumber, 
                                    Name= item.Name,
                                    Payment = item.Payment,
                                    Date = DateOnly.FromDateTime(item.DateTime)
                                });

                                innerItem.Name = item.Service;

                                innerItem.Total += item.Payment;

                                totalForCityDict[item.City] += item.Payment;

                                totalForCity += item.Payment;

                                inner[item.Service] = innerItem;
                            } 
                            else
                            {
                                ServiceModel innerItem = new ServiceModel();

                                innerItem.Payers.Add(new PayerModel
                                {
                                    AccountNumber = item.AccountNumber,
                                    Name = item.Name,
                                    Payment = item.Payment,
                                    Date = DateOnly.FromDateTime(item.DateTime)
                                });

                                innerItem.Name = item.Service;

                                innerItem.Total += item.Payment;

                                totalForCityDict[item.City] += item.Payment;

                                totalForCity += item.Payment;

                                inner.Add(item.Service, innerItem);
                            }

                            map[item.City] = inner;
                        }

                    } else
                    {
                        var inner = new Dictionary<string, ServiceModel>();

                        ServiceModel innerItem = new ServiceModel();

                        innerItem.Payers.Add(new PayerModel
                        {
                            AccountNumber = item.AccountNumber,
                            Name = item.Name,
                            Payment = item.Payment,
                            Date = DateOnly.FromDateTime(item.DateTime)
                        });

                        innerItem.Name = item.Service;

                        innerItem.Total += item.Payment;



                        totalForCity += item.Payment;

                        inner.Add(item.Service, innerItem);

                        totalForCityDict.Add(item.City, item.Payment);

                        map.Add(item.City, inner);
                    }
                }

                foreach(var item in map)
                {
                    var toPost = new PostDataModel();

                    toPost.City = item.Key;

                    foreach(var innerItem in item.Value)
                    {
                        toPost.Services.Add(innerItem.Value);
                    }

                    toPost.Total = totalForCityDict[item.Key];

                    posts.Add(toPost);

                }
                
            }

            var data = JsonConvert.SerializeObject(posts, Formatting.Indented);

            Console.WriteLine(data);

            File.WriteAllText("C:\\Users\\322TO\\Job\\DataProcessingService\\Output\\check.txt", data);

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



            return base.StartAsync(cancellationToken);
        }
    }
}
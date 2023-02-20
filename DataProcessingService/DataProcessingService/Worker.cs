using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using DataProcessingService.ServiceConfig;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics.CodeAnalysis;

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
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var check = Directory.GetFiles(_options.InputDirectory, "*.txt");
            string[] lines = System.IO.File.ReadAllLines(check[0]);

            _readData.ReadFile(check[0], new List<ReadDataModel>());

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
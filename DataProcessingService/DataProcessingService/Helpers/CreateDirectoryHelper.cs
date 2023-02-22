using DataProcessingService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Helpers
{
    public static class CreateDirectoryHelper
    {
        public static void CreateAfterMidnight(string outputPath, string data, ILogger _logger, ref string metaLogPath, ref MetaLogModel _logModel)
        {
            var directories = Directory.GetDirectories(outputPath);

            directories = directories.Select(x => x.Substring(outputPath.Length + 1)).ToArray();

            if (directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
            {
                var number = Directory.GetFiles(Path.Combine(outputPath, DateTime.Now.ToString("MM-dd-yyyy")));

                var files = number.Select(x => x).Where(x => x.Contains("output")).ToList();

                int orderNumber = files.Count + 1;
                using (FileStream fs = File.Create(Path.Combine(outputPath, DateTime.Now.ToString("MM-dd-yyyy"), $"output{orderNumber}.json")))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(data);

                    fs.Write(info, 0, info.Length);

                    _logger.LogInformation($"Record is written", DateTimeOffset.Now);
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(outputPath, DateTime.Now.ToString("MM-dd-yyyy")));

                using (FileStream fs = File.Create(Path.Combine(outputPath, DateTime.Now.ToString("MM-dd-yyyy"), "output1.json")))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(data);

                    fs.Write(info, 0, info.Length);

                    _logger.LogInformation($"Record is written", DateTimeOffset.Now);
                }

                metaLogPath = Path.Combine(outputPath, DateTime.Now.ToString("MM-dd-yyyy"), "meta.log");
                using (File.Create(metaLogPath))

                _logModel = new MetaLogModel();
            }
        }

        public static void CreateOnInit(ref string metaLogPath, string outputDir, ref MetaLogModel logModel)
        {
            var directories = Directory.GetDirectories(outputDir);

            directories = directories.Select(x => x.Substring(outputDir.Length + 1)).ToArray();

            if (directories.Contains(DateTime.Now.ToString("MM-dd-yyyy")))
            {

                var test = Path.Combine(outputDir, DateTime.Now.ToString("MM-dd-yyyy"));

                var number = Directory.GetFiles(test);

                var files = number.Select(x => x).Where(x => x.Contains("meta.log")).ToList();

                if (files.Count != 0)
                {
                    metaLogPath = files[0];

                    var meta = File.ReadAllText(metaLogPath);

                    logModel = JsonConvert.DeserializeObject<MetaLogModel>(meta);

                    if (logModel == null)
                    {
                        logModel = new MetaLogModel();
                    }

                }
                else
                {
                    using (File.Create(Path.Combine(outputDir, DateTime.Now.ToString("MM-dd-yyyy"), "meta.log")))

                        metaLogPath = Path.Combine(outputDir, DateTime.Now.ToString("MM-dd-yyyy"), "meta.log");

                    logModel = new MetaLogModel();
                }
            }
            else
            {
                logModel = new MetaLogModel();
            }
        }
    }
}

using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Services
{
    public class ReadDataService : IReadData
    {
        public async Task<Tuple<List<ReadDataModel>, int>> ReadFile(string path, List<ReadDataModel> result)
        {
            int counter = 0;

            result = new List<ReadDataModel>();

            //var check = await File.ReadAllLinesAsync(path);

            using (StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line;

                while ((line = await sr.ReadLineAsync()) != null)
                {
                    if (line != "")
                    {
                        var res = ReadMe(line);
                        counter++;

                        if (res.StateCheck != false)
                        {
                            result.Add(res);
                        }
                    }
                }
                sr.Close();

            }

                //foreach (var line in check)
                //{
                //    if (line != "")
                //    {
                //        var res = ReadMe(line);
                //        counter++;

                //        if (res.StateCheck != false)
                //        {
                //            result.Add(res);
                //        }
                //    }
                //}

            return new Tuple<List<ReadDataModel>, int>(result, counter);
        }

        public ReadDataModel ReadMe(string line)
        {
            List<string> result = new List<string>();

            string toAdd = "";

            for (int i = 0; i < line.Length; i++)
            {
                if (!line[i].Equals('"') && line[i] != '”' && line[i] != '“')
                {
                    toAdd += line[i];
                }
                else
                {
                    result.Add(toAdd);
                    toAdd = "";
                }
            }

            result.Add(toAdd);

            List<string> output = new List<string>();

            for (int i = 0; i < result.Count; i++)
            {
                string[] res;

                if (i == 1)
                {
                    output.Add(result[i].Trim().Split(',')[0]);
                }
                else
                {
                    res = result[i].Trim().Split(',');
                    output.AddRange(res);
                }
            }

            output.RemoveAll(x => x == "");

            bool checkbool = true;

            ReadDataModel record = new ReadDataModel();

            if (output.Count != 7)
            {
                record.StateCheck = false;
                return record;
            }
            else
            {
                decimal payment;
                DateTime date;
                long acc;

                if (!(checkbool = Decimal.TryParse(output[3], NumberStyles.Any, CultureInfo.InvariantCulture, out payment)))
                {
                    record.StateCheck = false;
                    return record;
                }

                if (!(checkbool = DateTime.TryParseExact(output[4], "yyyy-dd-MM", null, DateTimeStyles.AllowLeadingWhite, out date)))
                {
                    record.StateCheck = false;
                    return record; ;
                }

                if (!(checkbool = long.TryParse(output[5], out acc)))
                {
                    record.StateCheck = false;
                    return record;
                }

                record.AccountNumber = acc;
                record.DateTime = date.Date;
                record.Payment = payment;
                record.Name = output[0] + output[1];
                record.City = output[2];
                record.Service = output[6];
                record.StateCheck = true;
            }


            return record;
        }
    }
}

using DataProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Interfaces
{
    public interface IReadData
    {
        public void ReadFile(string path, List<ReadDataModel> result);
        //public bool ReadLine(ReadDataModel readDataModel, string line);
    }
}

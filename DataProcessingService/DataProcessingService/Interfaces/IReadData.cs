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
        public Task<Tuple<List<ReadDataModel>, int>> ReadFile(string path, List<ReadDataModel> result, int originalNumber);
    }
}

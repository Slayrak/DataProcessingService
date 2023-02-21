﻿using DataProcessingService.Models;
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
        public List<ReadDataModel> ReadFile(string path, out int originalNumber);
    }
}

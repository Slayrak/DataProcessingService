using DataProcessingService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Models
{
    public class ReadDataModel : IFactoryProduct
    {
        public string Name { get; set; }
        public string City { get; set; }

        public string Service { get; set; }
        public decimal Payment { get; set; }
        public DateTime DateTime { get; set; }

        public long AccountNumber { get; set; } 

        public bool StateCheck { get; set; }
    }
}

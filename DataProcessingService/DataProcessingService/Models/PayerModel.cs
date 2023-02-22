using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Models
{
    public class PayerModel
    {
        public string Name { get; set; }
        public decimal Payment { get; set; }
        public DateOnly Date { get; set; }

        public long  AccountNumber { get; set; }

    }
}

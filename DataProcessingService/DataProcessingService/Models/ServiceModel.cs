using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Models
{
    public class ServiceModel
    {
        public string Name { get; set; }
        public List<PayerModel> Payers { get; set; } = new List<PayerModel>();

        public decimal Total { get; set; } = 0;

    }
}

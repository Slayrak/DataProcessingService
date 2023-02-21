using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Models
{
    public class ServicesModel
    {
        public List<ServiceModel> Services { get; set;} = new List<ServiceModel>();

        public decimal Total { get; set;}
    }
}

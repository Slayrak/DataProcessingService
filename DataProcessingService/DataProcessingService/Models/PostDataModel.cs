using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Models
{
    public class PostDataModel
    {
        //public Dictionary<string, Dictionary<string, ServiceModel>> Cities { get; set; }

        public string City { get; set; }

        public List<ServiceModel> Services { get; set; } = new List<ServiceModel>();

        public decimal Total { get; set; }
    }
}

using DataProcessingService.AbstractClasses;
using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Helpers
{
    public class PostDataCreator : Creator
    {
        private readonly string _key;
        private readonly Dictionary<string, ServiceModel> _value;
        Dictionary<string, decimal> _totalForCityDict;
        public PostDataCreator(string key, Dictionary<string, ServiceModel> value, Dictionary<string, decimal> totalForCityDict) 
        {
            _key= key;
            _value= value;
            _totalForCityDict= totalForCityDict;
        }
        public override IFactoryProduct FactoryMethod()
        {
            var toPost = new PostDataModel();

            toPost.City = _key;

            foreach (var innerItem in _value)
            {
                toPost.Services.Add(innerItem.Value);
            }

            toPost.Total = _totalForCityDict[_key];

            return toPost;
        }

    }
}

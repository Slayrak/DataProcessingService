using DataProcessingService.Helpers;
using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Services
{
    public class GetPostDataService : IPostData
    {
        public void Categorize(List<ReadDataModel> rdm, List<PostDataModel> pdm)
        {
            if (rdm.Count != 0)
            {
                Dictionary<string, Dictionary<string, ServiceModel>> map = new Dictionary<string, Dictionary<string, ServiceModel>>();

                Dictionary<string, decimal> totalForCityDict = new Dictionary<string, decimal>();

                foreach (var item in rdm)
                {
                    if (map.ContainsKey(item.City))
                    {
                        var inner = new Dictionary<string, ServiceModel>();

                        if (map.TryGetValue(item.City, out inner))
                        {
                            if (inner.ContainsKey(item.Service))
                            {
                                var innerItem = inner.GetValueOrDefault(item.Service);

                                innerItem.Payers.Add(new PayerModel
                                {
                                    AccountNumber = item.AccountNumber,
                                    Name = item.Name,
                                    Payment = item.Payment,
                                    Date = DateOnly.FromDateTime(item.DateTime)
                                });

                                innerItem.Name = item.Service;

                                innerItem.Total += item.Payment;

                                totalForCityDict[item.City] += item.Payment;

                                inner[item.Service] = innerItem;
                            }
                            else
                            {
                                ServiceModel innerItem = new ServiceModel();

                                innerItem.Payers.Add(new PayerModel
                                {
                                    AccountNumber = item.AccountNumber,
                                    Name = item.Name,
                                    Payment = item.Payment,
                                    Date = DateOnly.FromDateTime(item.DateTime)
                                });

                                innerItem.Name = item.Service;

                                innerItem.Total += item.Payment;

                                totalForCityDict[item.City] += item.Payment;

                                inner.Add(item.Service, innerItem);
                            }

                            map[item.City] = inner;
                        }

                    }
                    else
                    {
                        var inner = new Dictionary<string, ServiceModel>();

                        ServiceModel innerItem = new ServiceModel();

                        innerItem.Payers.Add(new PayerModel
                        {
                            AccountNumber = item.AccountNumber,
                            Name = item.Name,
                            Payment = item.Payment,
                            Date = DateOnly.FromDateTime(item.DateTime)
                        });

                        innerItem.Name = item.Service;

                        innerItem.Total += item.Payment;

                        inner.Add(item.Service, innerItem);

                        totalForCityDict.Add(item.City, item.Payment);

                        map.Add(item.City, inner);
                    }
                }

                foreach (var item in map)
                {
                    var factory = new PostDataCreator(item.Key, item.Value, totalForCityDict);

                    pdm.Add((PostDataModel)factory.FactoryMethod());

                }

            }
        }
    }
}

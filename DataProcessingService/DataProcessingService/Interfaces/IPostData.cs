using DataProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Interfaces
{
    public interface IPostData
    {
        public void Categorize(List<ReadDataModel> rdm, List<PostDataModel> pdm); 
    }
}

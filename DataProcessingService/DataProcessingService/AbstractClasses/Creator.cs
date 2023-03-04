using DataProcessingService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.AbstractClasses
{
    public abstract class Creator
    {
        public abstract IFactoryProduct FactoryMethod();
    }
}

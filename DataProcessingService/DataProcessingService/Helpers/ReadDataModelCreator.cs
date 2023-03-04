using DataProcessingService.AbstractClasses;
using DataProcessingService.Interfaces;
using DataProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Helpers
{
    public class ReadDataModelCreator : Creator
    {
        private readonly List<string> _incomingString;
        public ReadDataModelCreator(List<string> incomingString) 
        {
            _incomingString = incomingString;
        }
        public override IFactoryProduct FactoryMethod() 
        {

            bool checkbool = true;

            ReadDataModel record = new ReadDataModel();

            if (_incomingString.Count != 7)
            {
                record.StateCheck = false;
                return record;
            }
            else
            {
                decimal payment;
                DateTime date;
                long acc;

                if (!(checkbool = Decimal.TryParse(_incomingString[3], NumberStyles.Any, CultureInfo.InvariantCulture, out payment)))
                {
                    record.StateCheck = false;
                    return record;
                }

                if (!(checkbool = DateTime.TryParseExact(_incomingString[4], "yyyy-dd-MM", null, DateTimeStyles.AllowLeadingWhite, out date)))
                {
                    record.StateCheck = false;
                    return record; ;
                }

                if (!(checkbool = long.TryParse(_incomingString[5], out acc)))
                {
                    record.StateCheck = false;
                    return record;
                }

                record.AccountNumber = acc;
                record.DateTime = date.Date;
                record.Payment = payment;
                record.Name = _incomingString[0] + _incomingString[1];
                record.City = _incomingString[2];
                record.Service = _incomingString[6];
                record.StateCheck = true;
            }


            return record;
        }
    }
}

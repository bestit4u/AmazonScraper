using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMSnotification
{
    public class PcmDataSet
    {
        public string productname { get; set; }
        public string brand { get; set; }
        public string price { get; set; }
        public string seller {get; set;}
        public string customerreview { get; set; }
        public string answeredquestion { get; set; }
        public PcmDataSet()
        {
            productname = string.Empty;
            brand = string.Empty;
            price = string.Empty;
            seller = string.Empty;
            customerreview = string.Empty;
            answeredquestion = string.Empty;
        }
    }
}

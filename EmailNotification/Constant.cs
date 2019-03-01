using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMSnotification
{
    public class Constant
    {
        public static bool bRun { get; set; }
        public static bool bCompleted { get; set; }
        public static int ParseToInt(string str)
        {
            int val = 0;
            int.TryParse(str, out val);
            return val;
        }
    }
}

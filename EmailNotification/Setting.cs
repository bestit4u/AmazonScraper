using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMSnotification
{
    public class Setting
    {
        private static Setting _instance = null;

        public static Setting Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Setting();
                }

                return _instance;
            }
        }

        public string username { get; set; }
        public string password { get; set; }
        public string captchaKey { get; set; }
        
        public Setting()
        {
            username = string.Empty;
            password = string.Empty;
            captchaKey = string.Empty;
        }
    }
}

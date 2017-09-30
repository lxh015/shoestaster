using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Code.SModel
{
    public class KeyM
    {
        public string Ip { get; set; }

        public string DateTime { get; set; }

        /// <summary>
        /// <example>订单：</example>
        /// </summary>
        public string KeyContext { get; set; }

        public KeyM(string ip)
        {
            this.Ip = ip;
            this.DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public KeyM(string ip,string dateTime)
        {
            this.Ip = ip;
            this.DateTime = dateTime;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Code
{
    public class Converts
    {
        /// <summary>
        /// 将数据转换成Bool值。（若为数字，偶数为False，奇数为True）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? StringToBool(string value)
        {
            bool? result = null;
            try
            {
                bool temp;
                if (bool.TryParse(value, out temp))
                {
                    result = temp;
                    return result;
                }
            }
            catch{}
            
            int tempInt = Convert.ToInt32(value);
            if (tempInt % 2 != 0)
                result = true;
            else
                result = false;

            return result;
        }

        public static T PaseEnum<T> (string value)where T: struct
        {
            if (string.IsNullOrEmpty(value))
                return new T();
            T tenum;
            Enum.TryParse<T>(value, false, out tenum);
            return tenum;
        }

        public static string EnToJsonStr(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public static T DeToJsonObj<T>(string value) where T:class
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }
    }
}

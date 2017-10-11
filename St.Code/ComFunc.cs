using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Code
{
    /// <summary>
    /// 通用函数组
    /// </summary>
    public class ComFunc
    {
        /// <summary>
        /// 随机数
        /// </summary>
        public static Random random = new Random();

        /// <summary>
        /// 随机生成大小写英文字母
        /// </summary>
        /// <param name="length">生成字符串长度</param>
        /// <returns></returns>
        public static string GetEnglistCodeString(int length = 4)
        {
            var total = DateTime.Now.Millisecond;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var randNum = random.Next(0, total * random.Next(1, 8));
                if (randNum % 2 == 0)
                    sb.Append((char)random.Next(65, 90));
                else
                    sb.Append((char)random.Next(97, 122));
            }
            return sb.ToString();
        }
    }
}

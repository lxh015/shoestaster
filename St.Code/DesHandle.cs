using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace St.Code
{
    public class DesHandle
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="myTxt"></param>
        /// <param name="desKey"></param>
        /// <returns></returns>
        public static string EnDes(string myTxt, string desKey)
        {
            try
            {
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    byte[] inputbytearray;
                    inputbytearray = Encoding.UTF8.GetBytes(myTxt);
                    
                    des.Key = ASCIIEncoding.ASCII.GetBytes(Md5(desKey).Substring(0, 8));
                    des.IV = ASCIIEncoding.ASCII.GetBytes(Md5(desKey).Substring(0, 8));

                    MemoryStream ms = new MemoryStream();
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputbytearray, 0, inputbytearray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }

                    string str = Convert.ToBase64String(ms.ToArray());
                    ms.Close();
                    return str;
                }
            }
            catch
            {
                return myTxt;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="desKey"></param>
        /// <returns></returns>
        public static string DeDes(string value, string desKey)
        {
            try
            {
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    byte[] inputbytearray;
                    inputbytearray = Convert.FromBase64String(value);
                    des.Key = ASCIIEncoding.ASCII.GetBytes(Md5(desKey).Substring(0, 8));
                    des.IV = ASCIIEncoding.ASCII.GetBytes(Md5(desKey).Substring(0, 8));
                    MemoryStream ms = new MemoryStream();
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputbytearray, 0, inputbytearray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }
                    string str = Encoding.UTF8.GetString(ms.ToArray());
                    ms.Close();
                    return str;
                }
            }
            catch
            {
                return value;
            }
        }
        
        public static string Md5(string value)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                //获取密文字节数组 
                byte[] byersult = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                //转换成字符串，32位 
                var strresult = BitConverter.ToString(byersult);
                //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉 
                strresult = strresult.Replace("-", "");
                return strresult;
            }
            catch
            {
                return value;
            }
        }
    }
}

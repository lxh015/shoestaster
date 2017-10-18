using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace St.Code.StException
{

    /// <summary>
    /// 表现异常
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(_Exception))]
    public class AppException:Exception
    {
        /// <summary>
        /// 静态内容
        /// </summary>
        private static readonly string contextInfo = "表现层异常";

        /// <summary>
        /// 表现异常
        /// </summary>
        public AppException() : base() { }

        /// <summary>
        /// 表现异常
        /// </summary>
        /// <param name="message">异常信息</param>
        public AppException(string message)
                : base($"{contextInfo}：{message}") { }

        /// <summary>
        /// 表现异常
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">异常源异常</param>
        public AppException(string message, Exception innerException)
                : base($"{contextInfo}：{message}", innerException) { }

        public override string ToString()
        {
            var temp = base.ToString();
            temp = contextInfo + temp;
            return temp;
        }
    }
}

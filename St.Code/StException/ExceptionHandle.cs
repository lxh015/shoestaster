using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace St.Code.StException
{
    public class ExceptionHandleAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            LogHandle.ILog log = new LogHandle.LogHandle();
            string message = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] 消息类型：{filterContext.Exception.GetType().Name}{Environment.NewLine}"
                + $"消息内容：{filterContext.Exception.Message}{Environment.NewLine}"
                + $"引发异常的方法：{filterContext.Exception.TargetSite}{Environment.NewLine}"
                + $"引发异常的对象：{filterContext.Exception.Source}{Environment.NewLine}"
                + $"异常目录：{filterContext.RouteData.GetRequiredString("controller")}{Environment.NewLine}"
                + $"异常文件：{filterContext.RouteData.GetRequiredString("action")}{Environment.NewLine}";
            log.Write(LogHandle.LogEnum.LogType.error, message);

            base.OnException(filterContext);
        }
    }
}

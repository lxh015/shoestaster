using St.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static St.Code.WebContextInfo;
using System.Web.Mvc.Filters;
using System.Web;
using St.Domain.Entity.SuperUser;

namespace St.Code
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 返回通用信息类
        /// </summary>
        public BaseJsonResult _baseResult = new BaseJsonResult();
        /// <summary>
        /// 日志链接
        /// </summary>
        public Code.LogHandle.ILog log = Ioc.GetService<St.Code.LogHandle.LogHandle>();

        /// <summary>
        /// 图片保存路径
        /// </summary>
        public readonly string ImgBasePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "ImageFiles\\");
        /// <summary>
        /// WebSet名称
        /// </summary>
        public readonly string webSetPath = "WebSet";
        
        #region 权限相关
        /// <summary>
        /// 检查用户授权
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            var goUrl = filterContext.HttpContext.Request.Url.Segments;
            var cache = HttpContext.Cache[webSetPath];
            if (cache == null)
            {
                SetWebSet(filterContext.HttpContext);

                AuthenLevel(filterContext);
            }
            else
            {
                AuthenLevel(filterContext, cache);
            }
        }

        /// <summary>
        /// 当WebSet信息过期后自动执行加载WebSet信息。使用Task以不影响主线程运行
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            Task taskInsertWebSet = new Task(() =>
            {
                var cache = HttpContext.Cache[webSetPath];
                if (cache == null)
                {
                    SetWebSet(filterContext.HttpContext);
                }
            });
            taskInsertWebSet.Start();
        }

        /// <summary>
        /// 设置网站各个控制器权限
        /// </summary>
        /// <param name="httpContext"></param>
        protected void SetWebSet(HttpContextBase httpContext)
        {
            if (null == httpContext.Cache.Get(webSetPath))
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                basePath = string.Format("{0}{1}", basePath, "BaseFile\\Level.txt");
                object _webSet;
                if (!System.IO.File.Exists(basePath))
                    return;
                else
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(basePath, Encoding.Default))
                    {
                        string context = sr.ReadToEnd();
                        _webSet = CreateWebSet.GetWebSet(context);
                    }
                }

                var pro = _webSet.GetType().GetProperties();

                httpContext.Cache.Add(webSetPath, _webSet, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }
        }

        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="filterContext"></param>
        protected void AuthenLevel(AuthenticationContext filterContext, params object[] webSetInfo)
        {
            var webSet = webSetInfo == null || webSetInfo.Length == 0 ?
                filterContext.HttpContext.Cache[webSetPath] : webSetInfo[0];

            var user = GetLoginUser(filterContext.HttpContext);
            if (user == null)
            {
                if (!filterContext.HttpContext.Request.Url.ToString().Contains("/Login"))
                    filterContext.Result = new RedirectResult("/Login");
                return;
            }
            int userLevel = Convert.ToInt32(user.Level);

            var urlSplit = filterContext.HttpContext.Request.Url.Segments;
            if (urlSplit.Length <= 1)
                return;
            var controlName = urlSplit[1].Replace("/", "");

            if (controlName.Contains("Home"))
                return;

            var properties = webSet.GetType().GetProperties();
            int level = Convert.ToInt32(LevelInfo.游客);

            try
            {
                var itemProperty = properties.Single(p => p.Name.Contains(controlName));
                level = Convert.ToInt32(itemProperty.GetValue(webSet));
            }
            catch (Exception ex)
            {
                WriteLog(LogHandle.LogEnum.LogType.operation, $"控制器操作权限异常：当前控制器名称：{controlName}；当前访问路径：{filterContext.HttpContext.Request.Url}；异常信息内容：{ex.Message}");
            }

            int intLevel = level;
            int intUserLv = Convert.ToInt32(user.Level);
            if (intLevel < intUserLv)
                filterContext.Result = new RedirectResult("/NoLevel");
        }

        #endregion

        #region 用户登录相关
        public readonly string desKey = "9j(8-Kdf";
        public readonly string tempCookieName = "tempCoko";

        /// <summary>
        /// 获取登录用户信息。可能为空
        /// </summary>
        /// <returns></returns>
        public SUser GetLoginUser(HttpContextBase httpContext)
        {
            if (httpContext.Request.Cookies.AllKeys.Contains(this.tempCookieName))
            {
                var cookieObj = httpContext.Request.Cookies[tempCookieName];
                if (cookieObj == null)
                    goto Error;

                string value = cookieObj.Value;
                if (string.IsNullOrEmpty(value))
                    goto Error;

                string deValue = DesHandle.DeDes(value, this.desKey);
                try
                {
                    SUser obj = Converts.DeToJsonObj<SUser>(deValue);
                    if (obj == null)
                        goto Error;
                    else
                        return obj;
                }
                catch
                {
                    goto Error;
                }
            }

            Error:
            return null;
        }
        #endregion

        #region 拓展方法
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void WriteLog(St.Code.LogHandle.LogEnum.LogType type, string message)
        {
            LogC logCs = new LogC(type, message);
            Task taskWriteLog = new Task((p) =>
            {
                var logc = p as LogC;
                log.Write(logc.Type, logc.Message);
            }, logCs);

            taskWriteLog.Start();
        }

        /// <summary>
        /// 日志转换类
        /// </summary>
        protected class LogC
        {
            public St.Code.LogHandle.LogEnum.LogType Type { get; set; }
            public string Message { get; set; }

            public LogC(LogHandle.LogEnum.LogType type, string message)
            {
                this.Type = type;
                this.Message = message;
            }
        }

        /// <summary>
        /// 获取GET数据
        /// </summary>
        /// <param name="Name">数据名称</param>
        /// <returns></returns>
        public string GetHttp(string Name)
        {
            return this.HttpContext.Request.QueryString[Name];
        }

        /// <summary>
        /// 获取POST数据
        /// </summary>
        /// <param name="Name">数据名称</param>
        /// <returns></returns>
        public string PostHttp(string Name)
        {
            return this.HttpContext.Request.Params[Name];
        }

        /// <summary>
        /// 将DateTime时间格式转换为Unix时间戳格式,毫秒
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>13位时间戳！</returns>
        public static long GetLongtime(System.DateTime time)
        {
            try
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                //double intResult = 0;
                //intResult = (time - startTime).TotalMilliseconds;
                //return intResult;
                long t = (time.Ticks - startTime.Ticks) / 10000;     //除10000调整为13位
                return t;
            }
            catch
            {
                return new long();
            }
        }
        #endregion

        /// <summary>
        /// 暂时没什么用
        /// </summary>
        /// <returns></returns>
        public async Task RSetBaseResult()
        {
            //Task taskReSet = new Task(() =>
            //  {
            //      this._baseResult.isSuccess = false;
            //      this._baseResult.returnMessage = string.Empty;
            //  });
            await Task.Delay(500);
        }
    }

    public static class Expand
    {
        //public static void CookieInsert(this HttpCookieCollection context, string name, string value)
        //{
        //    if (context.AllKeys.Contains(name))
        //    {

        //        context.Remove(name);
        //        context.Add(new HttpCookie(name, value));
        //    }
        //    else
        //        context.Add(new HttpCookie(name, value));
        //}
    }
}

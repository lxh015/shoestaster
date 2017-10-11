using St.Code;
using St.Domain.Entity.SuperUser;
using St.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace St.AdWeb.Controllers
{
    public class LoginController : BaseController
    {
        private readonly string CodeSessionName = "codeVale";
        private readonly string CodeSessionLastTime = "codeLastTime";
        private ISUserInterface UserService = Ioc.GetService<ISUserInterface>();
        
        /// <summary>
        /// 最大登录错误次数
        /// </summary>
        private readonly int maxErrorTime = 6;
        /// <summary>
        /// 最大验证码超时时间
        /// </summary>
        private readonly int maxTimeOut = 120000;

        // GET: Login
        public ActionResult Index()
        {
            var UserService = Ioc.GetService<ISUserInterface>();
            var query = new QueryExpression<SUser>();

            var count = UserService.GetByQuery(query);



            return View();
        }

        [HttpPost]
        public ActionResult Into()
        {
            LoginMessage message = new LoginMessage("");
            try
            {
                if (!CheckVCode(this.HttpContext))
                {
                    message.SetMessage("", false, "验证码信息错误！");
                    goto Next;
                }

                string userName = HttpContext.Request.Params["username"];
                string passWord = HttpContext.Request.Params["password"];

                if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(passWord))
                    RecordErrorLogin(this.HttpContext);

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
                    message.SetMessage(userName, false, "账号或密码不能为空！");
                else
                {
                    if (UserNameIsLock(userName))
                    {
                        message.SetMessage("", false, $"登录失败！由于您尝试过多次登录，您将暂时无法进行登录操作！");
                        goto Next;
                    }

                    bool result = LoginCheckHandle(this.HttpContext);

                    if (result)
                    {
                        SendSuccessToRequest(userName);
                        message.SetMessage(userName, true, "登录成功！");
                    }
                    else
                    {
                        int errorTime = HandleLoginErrorData(this.HttpContext, userName);
                        if (errorTime >= maxErrorTime)
                        {
                            if (errorTime == maxErrorTime)
                            {
                                WriteLog(St.Code.LogHandle.LogEnum.LogType.login, $"地址为：{HttpContext.Request.UserHostAddress}，尝试登录用户名为：{userName}，错误次数{errorTime}");
                                LoginUserLock(userName);
                            }
                            message.SetMessage("", false, $"登录失败！由于您尝试过多次登录，您将暂时无法进行登录操作！");
                        }
                        else
                        {
                            message.SetMessage("", false, $"登录失败！您还有{maxErrorTime - errorTime}次尝试登录！");
                        }
                    }
                }

                Next:
                return Json(message, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                message.SetMessage("", false, "登录失败！");
                return Json(message, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult LogOut()
        {
            RemoveLoginInfo(this.HttpContext);

            var cookieAll = HttpContext.Request.Cookies.AllKeys;
            foreach (var item in cookieAll)
            {
                HttpCookie citem = HttpContext.Request.Cookies[item];
                citem.Expires = DateTime.Now.AddSeconds(-60);
                HttpContext.Response.Cookies.Add(citem);
            }
            return View("Index");
        }


        public ActionResult RegisterSuperUser(int number = 0)
        {
            ViewBag.UserList = null;

            if (number < 0)
                return View();

            List<SUser> registerUserList = new List<SUser>();

            try
            {
                for (int i = 0; i < number; i++)
                {
                    SUser su = new SUser();

                    su.Name = ComFunc.GetEnglistCodeString(6);
                    su.NickName = su.Name;
                    su.isUse = true;
                    su.Level = (Domain.Entity.LevelInfo)2;
                    su.PassWord = "123456";
                    su.Stata = (Domain.Entity.AuditState)1;

                    UserService.Add(su);
                    registerUserList.Add(su);
                }
            }
            catch (Exception ex)
            {
                WriteLog(St.Code.LogHandle.LogEnum.LogType.operation, $"批量添加用户发生异常，{ex.Message}");
            }
            ViewBag.UserList = registerUserList;

            return View();
        }


        private bool CheckVCode(HttpContextBase httpContext)
        {
            var vCode = httpContext.Request.Params["vcode"];
            var vcTime = httpContext.Request.Params["vctime"];

            if (string.IsNullOrEmpty(vCode) || string.IsNullOrEmpty(vcTime))
                return false;

            var snVCode = httpContext.Cache[$"{CodeSessionName}_{vcTime}"].ToString();
            var snVCodeTime = httpContext.Cache[$"{CodeSessionLastTime}_{vcTime}"];
            if (snVCode == null || snVCodeTime == null)
                return false;

            DateTime vCodetTime = Convert.ToDateTime(snVCodeTime);
            var timeSpan = (DateTime.Now - vCodetTime).TotalMilliseconds;
            if (timeSpan < 0 || timeSpan > maxTimeOut)
                return false;

            if (vCode.ToLower() != snVCode.ToLower())
                return false;

            return true;
        }

        /// <summary>
        /// 发送登录成功的Token
        /// </summary>
        /// <param name="userName"></param>
        private void SendSuccessToRequest(string userName)
        {
            Guid guid = new Guid();
            guid = Guid.NewGuid();

            string key = $"{guid}_{userName}";
            string desKy = DesHandle.EnDes(key, "12345678");

            RemoveVCSession(this.HttpContext);

            HttpCookie successCookie = new HttpCookie(userName, desKy);
            successCookie.Expires = DateTime.Now.AddDays(1);
            successCookie.Shareable = false;
            HttpContext.Response.Cookies.Add(successCookie);
            HttpContext.Response.Cookies.Add(new HttpCookie("LoginUser", userName) { Expires = DateTime.Now.AddDays(1), Shareable = false });
        }

        /// <summary>
        /// 移除登录信息
        /// </summary>
        /// <param name="httpContext"></param>
        private void RemoveLoginInfo(HttpContextBase httpContext)
        {
            try
            {
                string userName = httpContext.Request.Cookies["LoginUser"].ToString();
                if (!string.IsNullOrEmpty(userName))
                {
                    var sessionKeys = httpContext.Session.Keys;
                    foreach (var item in sessionKeys)
                    {
                        if (item.ToString().Contains(userName))
                            httpContext.Session.Remove(item.ToString());
                    }

                    var cacheEnum = httpContext.Cache.GetEnumerator();
                    while (cacheEnum.MoveNext())
                    {
                        if (cacheEnum.Key.ToString().Contains(userName))
                            httpContext.Cache.Remove(cacheEnum.Key.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                WriteLog(St.Code.LogHandle.LogEnum.LogType.error, $"退出异常，{ex.Message}");
            }
        }

        /// <summary>
        /// 检查用户锁定信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        protected bool UserNameIsLock(string userName)
        {
            string lockName = $"{userName}Lock";
            if (HttpContext.Request.Cookies.AllKeys.Contains(lockName))
            {
                var endTime = Convert.ToDateTime(DesHandle.DeDes(HttpContext.Request.Cookies[lockName].Value, this.desKey));
                if (endTime > DateTime.Now)
                    return true;
                else
                {
                    RemoveLockUser(this.RemoveSuperPassword, userName);
                    return false;
                }
            }
            else
            {
                var cacheObj = HttpContext.Cache[lockName];
                if (cacheObj == null)
                    return false;
                else
                {
                    var value = Convert.ToDateTime(cacheObj);
                    if (value > DateTime.Now)
                        return true;
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// 锁定账户
        /// </summary>
        /// <param name="userName"></param>
        protected void LoginUserLock(string userName)
        {
            string lockName = $"{userName}Lock";
            if (HttpContext.Response.Cookies.AllKeys.Contains(lockName))
                return;
            string lockEndTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            HttpCookie cookie = new HttpCookie(lockName, DesHandle.EnDes(lockEndTime, desKey));
            HttpContext.Response.Cookies.Add(cookie);
            HttpContext.Cache.Add(lockName, lockEndTime, null, DateTime.Now.AddDays(1),
                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
        }

        new private readonly string desKey = "l111222&N";
        private readonly string RemoveSuperPassword = "l111222";

        /// <summary>
        /// 移除用户锁定信息
        /// </summary>
        /// <param name="password"></param>
        /// <param name="userName"></param>
        public void RemoveLockUser(string password, string userName)
        {
            if (password != RemoveSuperPassword)
                return;
            else
            {
                HttpCookie removeCookie = new HttpCookie($"{userName}Lock");
                removeCookie.Expires = DateTime.Now.AddDays(-1);

                HttpContext.Response.Cookies.Add(removeCookie);
                HttpContext.Request.Cookies.Add(removeCookie);

                HttpContext.Cache.Remove($"{userName}Lock");
                HttpContext.Session.Remove($"{userName}Lock");
            }
        }

        /// <summary>
        /// 设置错误登录次数
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        protected int HandleLoginErrorData(HttpContextBase httpContext, string userName)
        {
            var userSession = httpContext.Session[userName];
            if (userSession == null)
            {
                httpContext.Session.Add(userName, 1);
                return 1;
            }
            else
            {
                int errorTime = Convert.ToInt32(userSession);
                httpContext.Session.Add(userName, errorTime + 1);
                return errorTime + 1;
            }
        }

        /// <summary>
        /// 查询错误登录次数
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        protected int GetErrorTime(HttpContextBase httpContext, string userName)
        {
            var userSession = httpContext.Session[userName];
            if (null == userSession)
                return 0;
            else
                return Convert.ToInt32(userSession);
        }

        /// <summary>
        /// 检查用户登录名和密码。
        /// 同时注入用户加密信息。
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected bool LoginCheckHandle(HttpContextBase httpContext)
        {
            try
            {
                string userName = HttpContext.Request.Params["username"];
                string passWord = HttpContext.Request.Params["password"];

                var user = UserService.UserLogin(userName, passWord);
                if (user != null)
                {
                    string userJson = St.Code.Converts.EnToJsonStr(user);
                    HttpContext.Response.Cookies.Add(new HttpCookie(this.tempCookieName, DesHandle.EnDes(userJson, base.desKey)));
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                WriteLog(St.Code.LogHandle.LogEnum.LogType.login, ex.Message);
                return false;
            }
        }

        ///// <summary>
        ///// 记录异常登录
        ///// </summary>
        ///// <param name="httpContext"></param>
        //private void RecordLogin(HttpContextBase httpContext)
        //{
        //    var requestIp = httpContext.Request.UserHostAddress;
        //    var requestBrowser = httpContext.Request.Browser.Browser.ToString();

        //    string errorMessage = $"发现异常登录者，IP：{requestIp}；浏览器：{requestBrowser}";
        //    WriteLog(St.Code.LogHandle.LogEnum.LogType.error, errorMessage);
        //}

        /// <summary>
        /// 记录空用户名和密码登录
        /// </summary>
        /// <param name="httpContext"></param>
        protected void RecordErrorLogin(HttpContextBase httpContext)
        {
            Task taskRecordLogin = new Task((p) =>
            {
                var context = p as HttpContextBase;
                var requestIp = context.Request.UserHostAddress;
                var requestBrowser = context.Request.Browser.Browser.ToString();

                string errorMessage = $"发现异常登录者，IP：{requestIp}；浏览器：{requestBrowser}";
                log.Write(St.Code.LogHandle.LogEnum.LogType.error, errorMessage);
            }, httpContext);
            taskRecordLogin.Start();
        }

        public void Code(string randNum)
        {
            if (string.IsNullOrEmpty(randNum))
                return;
            if (HttpContext.Cache.Count != 0)
                RemoveVCSession(this.HttpContext);

            MakeCode makeCode = new MakeCode(4);
            makeCode.getImageValidate(CodeType.all);
            HttpContext.Cache.Insert($"{CodeSessionName}_{randNum}", makeCode.vcodeLog);
            HttpContext.Cache.Insert($"{CodeSessionLastTime}_{randNum}", DateTime.Now);
        }

        /// <summary>
        /// 清空验证码缓存数据
        /// </summary>
        /// <param name="httpContext"></param>
        private void RemoveVCSession(HttpContextBase httpContext)
        {
            Task taskRemoveSession = new Task(() =>
            {
                if (httpContext.Cache.Count == 0)
                    return;

                List<string> removeSessionName = new List<string>();

                foreach (var item in httpContext.Cache)
                {
                    if (item.ToString().Contains(CodeSessionName) || item.ToString().Contains(CodeSessionLastTime))
                        removeSessionName.Add(item.ToString());
                }

                if (removeSessionName.Count != 0)
                {
                    removeSessionName.ForEach(p =>
                    {
                        httpContext.Cache.Remove(p);
                    });
                }
            });
            taskRemoveSession.Start();
        }

        public class LoginMessage
        {
            public string message { get; private set; }

            public string username { get; private set; }

            public bool islogin { get; private set; }

            public string datetime { get; private set; }

            public LoginMessage(string username, bool islogin = false, string message = "登录失败！")
            {
                this.username = username;
                this.islogin = islogin;
                this.message = message;
                this.datetime = GetLongtime(DateTime.Now).ToString();
            }

            public void SetMessage(string userName, bool islogin, string message)
            {
                this.username = userName;
                this.islogin = islogin;
                this.message = message;
                this.datetime = GetLongtime(DateTime.Now).ToString();
            }
        }
    }
}
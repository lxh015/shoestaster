using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace St.Code
{
    /// <summary>
    /// 用于登录
    /// </summary>
    public class CKLG : AuthorizeAttribute
    {
        private readonly string specialCheck = "SpecialInfo";
        private readonly string specialKey = "(((((999";

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies.AllKeys.Contains(specialCheck))
            {
                var keyValue = filterContext.HttpContext.Request.Cookies[specialCheck].Value;
                if (!string.IsNullOrEmpty(keyValue))
                {
                    var deTimeValue = St.Code.DesHandle.DeDes(keyValue, specialKey);
                    try
                    {
                        var timeValue = Convert.ToDateTime(deTimeValue);
                        if (timeValue >= DateTime.Now)//若在登录验证在限定1小时内，不进行验证
                            return;
                    }
                    catch
                    {
                        
                    }
                }
            }

            var userNameCookie = filterContext.HttpContext.Request.Cookies["LoginUser"];
            if (userNameCookie == null)
                goto ErrorGo;

            string userName = userNameCookie.Value;
            var checkCookie = filterContext.HttpContext.Request.Cookies[userName];

            if (checkCookie == null)
                goto ErrorGo;

            string checkValue = checkCookie.Value;
            string deValue = DesHandle.DeDes(checkValue, "12345678");
            if (!deValue.Contains("_"))
                goto ErrorGo;

            string[] valueSplit = deValue.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (valueSplit.Length != 2)
                goto ErrorGo;

            if (valueSplit[1] != userName)
                goto ErrorGo;
            
            DateTime nextCheckTime = DateTime.Now.AddHours(1);
            string nextDesTime = St.Code.DesHandle.EnDes(nextCheckTime.ToString("yyyy-MM-dd HH:mm:ss"), specialKey);
            filterContext.HttpContext.Response.Cookies.Add(new System.Web.HttpCookie(specialCheck, nextDesTime));

            return;

            ErrorGo:
            filterContext.Result = new RedirectResult("/Login");
            // base.OnAuthorization(filterContext);
        }
    }
}

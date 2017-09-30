using St.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace St.AdWeb.Controllers
{
    [CKLG]
    public class HomeController : BaseController
    {
       // private St.Code.LogHandle.ILog log = Ioc.GetService<St.Code.LogHandle.LogHandle>();

        // GET: Home
        public ActionResult Index()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //SendCookieKey(this.HttpContext);
            //log.Write(Code.LogHandle.LogEnum.LogType.log, "test");

            stopWatch.Stop();

            ViewBag.UseTime = stopWatch.ElapsedMilliseconds;

            return View();
        }
    }
}
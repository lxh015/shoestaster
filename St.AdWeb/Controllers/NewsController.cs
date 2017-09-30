using St.Code;
using St.Domain.Entity.News;
using St.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static St.Code.WebContextInfo;

namespace St.AdWeb.Controllers
{
    public class NewsController : BaseController
    {
        private INewsMainInterface NewsMainService = Ioc.GetService<INewsMainInterface>();
        private INewsShowInterface NewsShowService = Ioc.GetService<INewsShowInterface>();

        #region 新闻主体
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewsMainList(int page = 0)
        {
            BaseListResult<NewsMain> Data = new BaseListResult<NewsMain>();
            try
            {
                List<NewsMain> dataList = new List<NewsMain>();
                Code.QueryExpression<NewsMain> query = new QueryExpression<NewsMain>(p => p.ID != 0);

                dataList = NewsMainService.QueryForPage(page, query);
                if (dataList.Count == 0)
                    Data.SetError();
                else
                    Data.SetData(dataList);
            }
            catch
            {
                Data.SetError();
            }
            return Json(Data, JsonRequestBehavior.DenyGet);
        }

        #region 编辑和添加
        public ActionResult NewsMainEidt(bool isAdd = true, int id = 0)
        {
            ViewBag.Type = isAdd;
            ViewBag.Id = id;

            ViewBag.NM = null;
            if (id != 0)
            {
                ViewBag.NM = NewsMainService.GetByIDForInclude(id, "newShow");
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> NewsMainEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var title = PostHttp("title");
                var isshow = PostHttp("isshow");
                var audit = PostHttp("audit");
                var context = PostHttp("context");
                var summary = PostHttp("summary");
                var imageArray = PostHttp("imagelist");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(title) || string.IsNullOrEmpty(isshow) ||
                    string.IsNullOrEmpty(audit))
                {
                    _baseResult.SetResult(false, "数据不完整！");
                    goto Ret;
                }

                bool isAdd = Convert.ToBoolean(type);
                var ID = Convert.ToInt32(id);

                bool Add = false;
                if (isAdd && ID == 0)
                    Add = true;
                else if (!isAdd && ID != 0)
                    Add = false;
                else
                {
                    _baseResult.SetResult(false, "操作类型错误！");
                    goto Ret;
                }

                bool result = false;
                if (Add)
                    result = AddNM(title, isshow, audit, summary, context, imageArray);
                else
                    result = EidtNM(ID, title, isshow, audit, summary, context, imageArray);

                _baseResult.SetResult(result, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
                goto Ret;
            }
            Ret:
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        private bool AddNM(string title, string isshow, string audit, string summary,
            string context, string imageArray)
        {
            NewsMain nm = new NewsMain();

            nm.Title = title;
            nm.Summary = summary;
            nm.Context = context;
            nm.IsShow = Convert.ToBoolean(isshow);
            nm.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);

            NewsMainService.Add(nm,imageArray);
            return true;
        }

        private bool EidtNM(int id, string title, string isshow, string audit, string summary,
            string context, string imageArray)
        {
            NewsMain nm = new NewsMain();

            nm.ID = id;
            nm.Title = title;
            nm.Summary = summary;
            nm.Context = context;
            nm.IsShow = Convert.ToBoolean(isshow);
            nm.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);

            NewsMainService.Modify(nm,imageArray);
            return true;
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> DeleteNewsMain(int id)
        {
            try
            {
                NewsMainService.Delete(id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }
        
        public ActionResult NewsImageList(int nid)
        {
            ViewBag.NM = NewsMainService.GetByIDForInclude(nid, "newShow");

            return View();
        }

        #endregion
    }
}
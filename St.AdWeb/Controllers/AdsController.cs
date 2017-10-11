using St.Code;
using St.Domain.Entity.AD;
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
    public class AdsController : BaseController
    {
        private IAdsInterface AdsService = Ioc.GetService<IAdsInterface>();


        #region 广告
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdsList(int page = 0)
        {
            BaseListResult<Ads> Data = new BaseListResult<Ads>();
            try
            {
                List<Ads> dataList = new List<Ads>();
                QueryExpression<Ads> query = new QueryExpression<Ads>();

                dataList = AdsService.QueryForPage(page, query);
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
        public ActionResult AdsEidt(bool isAdd = true, int id = 0)
        {
            ViewBag.Type = isAdd;
            ViewBag.Id = id;

            ViewBag.Ads = null;
            if (id != 0)
            {
                ViewBag.Ads = AdsService.GetByIDForInclude(id, "image");
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AdsEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var title = PostHttp("title");
                var linkUrl = PostHttp("linkurl");
                var audit = PostHttp("audit");
                var area = PostHttp("area");
                var image = PostHttp("image");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(title) ||
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
                    result = AddAds(title, linkUrl, audit, area, image);
                else
                    result = Eidt(ID, title, linkUrl, audit, area, image);

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

        private bool AddAds(string title, string linkUrl, string audit, string area, string image)
        {
            Ads ads = new Ads();

            ads.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);
            ads.Title = title;
            ads.LinkUrl = linkUrl;
            ads.Area = (AdsArea)Convert.ToInt32(area);

            AdsService.Add(ads, image);
            return true;
        }

        private bool Eidt(int id, string title, string linkUrl, string audit, string area, string image)
        {
            Ads ads = new Ads();

            ads.ID = id;
            ads.Title = title;
            ads.LinkUrl = linkUrl;
            ads.Area = (AdsArea)Convert.ToInt32(area);
            ads.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);

            AdsService.Modify(ads, image);
            return true;
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                AdsService.Delete(id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        public ActionResult test()
        {
            return View();
        }

        [HttpPost]
        public string TestPost()
        {
            var param = HttpContext.Request.Params;
            return "True";
        }
        #endregion
    }
}
using St.Code;
using St.Domain.Entity.SuperUser;
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
    [CKLG]
    public class SUserController : BaseController
    {
        private ISUserInterface SUserService = Ioc.GetService<ISUserInterface>();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SUserList(int page = 0)
        {
            BaseListResult<SUser> Data = new BaseListResult<SUser>();
            try
            {
                List<SUser> dataList = new List<SUser>();
                Code.QueryExpression<SUser> query = new QueryExpression<SUser>(p => p.ID != 0);

                dataList = SUserService.QueryForPage(page, query);
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
        public ActionResult SUserEidt(bool isAdd = true, int id = 0)
        {
            ViewBag.Type = isAdd;
            ViewBag.Id = id;

            ViewBag.SU = null;
            if (id != 0)
            {
                ViewBag.SU = SUserService.GetByID(id);
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SUserEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var name = PostHttp("name");
                var nickname = PostHttp("nickname");
                var password = PostHttp("password");
                var level = PostHttp("level");
                var audit = PostHttp("audit");
                var isuse = PostHttp("isuse");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(name) || string.IsNullOrEmpty(nickname) ||
                    string.IsNullOrEmpty(password))
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
                    result = AddSUser(name, nickname, password, audit, level, isuse);
                else
                    result = EidtSUser(ID, name, nickname, password, audit, level, isuse);

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

        private bool AddSUser(string name, string nickname, string password, string audit,
            string level, string isuse)
        {
            SUser su = new SUser();

            su.Name = name;
            su.NickName = nickname;
            var temp = St.Code.Converts.StringToBool(isuse);
            su.isUse = temp == null ? false : temp.Value;
            su.Level = (Domain.Entity.LevelInfo)Convert.ToInt32(level);
            su.PassWord = password;
            su.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);

            SUserService.Add(su);
            return true;
        }

        private bool EidtSUser(int id, string name, string nickname, string password, string audit,
            string level, string isuse)
        {
            SUser su = new SUser();

            su.ID = id;
            su.Name = name;
            su.NickName = nickname;
            var temp = St.Code.Converts.StringToBool(isuse);
            su.isUse = temp == null ? false : temp.Value;
            su.Level = (Domain.Entity.LevelInfo)Convert.ToInt32(level);
            su.PassWord = password;
            su.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);

            SUserService.Modify(su);
            return true;
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                SUserService.Delete(id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        public ActionResult WebSetting()
        {
            object webSet = HttpContext.Cache.Get(webSetPath);
            var pro = webSet.GetType().GetProperties();
            ViewBag.WebSet = webSet;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SaveWebSet()
        {
            try
            {
                string productLevel = PostHttp("productLevel");
                string newsLevel = PostHttp("newsLevel");
                string adsLevel = PostHttp("adsLevel");
                string imageLevel = PostHttp("imageLevel");
                string settingLevel = PostHttp("settingLevel");

                if (string.IsNullOrEmpty(productLevel) || string.IsNullOrEmpty(newsLevel) ||
                    string.IsNullOrEmpty(adsLevel) || string.IsNullOrEmpty(imageLevel) ||
                    string.IsNullOrEmpty(settingLevel))
                {
                    _baseResult.SetResult(false, "信息不完整！");
                    goto Ret;
                }

                dynamic webset = HttpContext.Cache.Get(webSetPath);

                if (webset == null)
                {

                }

                webset.ProductsLevel = (Domain.Entity.LevelInfo)Convert.ToInt32(productLevel);
                webset.NewsLevel = (Domain.Entity.LevelInfo)Convert.ToInt32(newsLevel);
                webset.AdsLevel = (Domain.Entity.LevelInfo)Convert.ToInt32(adsLevel);
                webset.ImagesLevel = (Domain.Entity.LevelInfo)Convert.ToInt32(imageLevel);
                webset.SettingLevel = (Domain.Entity.LevelInfo)Convert.ToInt32(settingLevel);

                HttpContext.Cache[webSetPath] = webset;
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作异常！");
            }

            Ret:
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }
    }
}
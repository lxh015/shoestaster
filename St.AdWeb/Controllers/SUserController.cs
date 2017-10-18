using St.Code;
using St.Domain.Entity.SuperUser;
using St.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public ActionResult SUserList(int page = 0, string search = "")
        {
            BaseListResult<SUser> Data = new BaseListResult<SUser>();
            try
            {
                List<SUser> dataList = new List<SUser>();
                Code.QueryExpression<SUser> query = new QueryExpression<SUser>();

                dataList = SUserService.QueryForPage(page, query);
                Data.DataSumCount = Convert.ToInt32(Math.Round(Convert.ToDouble(query.PageSumCount) / Convert.ToDouble(query.PageCountNumber)));
                Data.DataSumCount = Data.DataSumCount == 0 ? 1 : Data.DataSumCount;

                if (dataList.Count == 0)
                    Data.SetError();
                else
                    Data.SetData(dataList);
            }
            catch(Exception ex)
            {
                WriteLog(Code.LogHandle.LogEnum.LogType.operation, ex.Message);
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
                var failedreason = PostHttp("failedreason");

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
                    result = AddSUser(name, nickname, password, audit, level, isuse, failedreason);
                else
                    result = EidtSUser(ID, name, nickname, password, audit, level, isuse, failedreason);

                _baseResult.SetResult(result, "操作成功！");
            }
            catch (Exception ex)
            {
                WriteLog(Code.LogHandle.LogEnum.LogType.operation, ex.Message);
                _baseResult.SetResult(false, "操作失败！");
                goto Ret;
            }
            Ret:
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        private bool AddSUser(string name, string nickname, string password, string audit,
            string level, string isuse, string failedreason)
        {
            SUser su = new SUser();

            su.Name = name;
            su.NickName = nickname;
            var temp = St.Code.Converts.StringToBool(isuse);
            su.isUse = temp == null ? false : temp.Value;
            su.Level = (Domain.Entity.LevelInfo)Convert.ToInt32(level);
            su.PassWord = password;
            su.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);
            su.FailedReason = failedreason;

            SUserService.Add(su);
            return true;
        }

        private bool EidtSUser(int id, string name, string nickname, string password, string audit,
            string level, string isuse, string failedreason)
        {
            SUser su = new SUser();

            su = SUserService.GetByID(id);
            su.ID = id;
            su.Name = name;
            su.NickName = nickname;
            var temp = St.Code.Converts.StringToBool(isuse);
            su.isUse = temp == null ? false : temp.Value;
            su.Level = (Domain.Entity.LevelInfo)Convert.ToInt32(level);
            su.PassWord = password;
            su.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);
            su.FailedReason = su.Stata == Domain.Entity.AuditState.审核失败 ? failedreason : "";

            su.UpdateTime = DateTime.Now;
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
            catch(Exception ex)
            {
                WriteLog(Code.LogHandle.LogEnum.LogType.operation, ex.Message);
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
                dynamic webset = HttpContext.Cache.Get(webSetPath);
                if (webset == null)
                    webset = GetWebSet();

                Type wsType = webset.GetType();
                var wsProperties = wsType.GetProperties();
                var postArray = HttpContext.Request.Params.AllKeys;

                int mathLength = 0;
                wsProperties.ToList().ForEach(p =>
                {
                    if (postArray.Where(f => f.ToLower() == p.Name.ToLower()).Count() > 0)
                        mathLength += 1;
                });

                if (mathLength != wsProperties.Length)
                {
                    _baseResult.SetResult(false, "信息不完整！");
                    goto Ret;
                }

                foreach (var item in wsProperties)
                {
                    foreach (var post in postArray)
                    {
                        if (item.Name.ToLower() == post.ToLower())
                        {
                            try
                            {
                                string postVal = HttpContext.Request.Params[post];
                                var enumVal = (St.Domain.Entity.LevelInfo)Convert.ToInt32(Enum.Parse(typeof(St.Domain.Entity.LevelInfo), postVal));
                                item.SetValue(webset, enumVal);
                            }
                            catch (Exception ex)
                            {
                                WriteLog(Code.LogHandle.LogEnum.LogType.operation, $"设置WebSet异常，当参数:{post}，{ex.Message}");
                            }
                            break;
                        }
                    }
                }

                HttpContext.Cache[webSetPath] = webset;
                WriteWebSet();
                _baseResult.SetResult(true, "操作成功！");
            }
            catch(Exception ex)
            {
                WriteLog(Code.LogHandle.LogEnum.LogType.operation, ex.Message);
                _baseResult.SetResult(false, "操作异常！");
            }

            Ret:
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        private void WriteWebSet()
        {
            Task taskWriteNewWebSet = new Task(() =>
            {
                object webset = HttpContext.Cache[webSetPath];
                var propertis = webset.GetType().GetProperties();
                StringBuilder sb = new StringBuilder();
                foreach (var item in propertis)
                {
                    var attributeVal = item.CustomAttributes.First().ConstructorArguments[0].Value;
                    var properName = item.Name;
                    var properVal = Convert.ToInt32(item.GetValue(webset));
                    sb.Append($"{attributeVal}:{properName}:{properVal},");
                }

                string basePath = GetWebSetPath();

                System.IO.FileStream fs = new System.IO.FileStream(basePath, System.IO.FileMode.Create);
                var bytes = Encoding.Default.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Dispose();
                fs.Close();
            });
            taskWriteNewWebSet.Start();
        }
    }
}
using St.Code;
using St.Domain.Entity.Picture;
using St.Service;
using St.Specification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static St.Code.WebContextInfo;

namespace St.AdWeb.Controllers
{
    public class ImagesController : BaseController
    {
        private IImagesInterface ImageService = Ioc.GetService<IImagesInterface>();

        #region 图片
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> UploadImage()
        {
            try
            {
                var fileName = Request.Files["upfile"].FileName;//获取文件名；可能发生异常
                var fileLength = Request.Files["upfile"].ContentLength;//获取上传文件流的长度

                if (!fileName.Contains(".jpg") && !fileName.Contains(".bmp") && !fileName.Contains(".gif") &&
                    !fileName.Contains(".png") && !fileName.Contains(".jpeg"))
                {
                    _baseResult.SetResult(false, "非图片文件！");
                    goto Ret;
                }
                string[] nameArray = fileName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (nameArray.Length != 2)
                {
                    _baseResult.SetResult(false, "文件信息错误！");
                    goto Ret;
                }
                var saveName = string.Format("{0}.{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), nameArray[1]);
                string saveFilePath = string.Format("{0}\\{1}", ImgBasePath, saveName);

                FileStream fs = new FileStream(Path.Combine(saveFilePath), FileMode.Create);//创建文件流

                byte[] buffer = new byte[fileLength];//创建保存文件的数组
                int bys = Request.Files["upfile"].InputStream.Read(buffer, 0, fileLength);//读取文件
                fs.Write(buffer, 0, fileLength);//保存文件
                fs.Close();//关闭文件保存
                fs.Dispose();

                _baseResult.SetResult(true, saveName);
            }
            catch (Exception)
            {
                _baseResult.SetResult(false, "操作异常！");
            }
            Ret:
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult ImageList(int page = 0, string search = "")
        {
            BaseListResult<Images> Data = new BaseListResult<Images>();
            try
            {
                List<Images> dataList = new List<Images>();
                QueryExpression<Images> query = new QueryExpression<Images>();

                if (!string.IsNullOrEmpty(search))
                    query.AddExperssion(new ExpressionSpecification<Images>(p => p.Title.Contains(search)));
                
                dataList = ImageService.QueryForPage(page, query);
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
        public ActionResult ImageEidt(bool isAdd = true, int id = 0)
        {
            ViewBag.Type = isAdd;
            ViewBag.Id = id;

            ViewBag.IM = null;
            if (id != 0)
            {
                ViewBag.IM = ImageService.GetByID(id);
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ImageEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var title = PostHttp("name");
                var context = PostHttp("context");
                var filename = PostHttp("filename");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(title) || string.IsNullOrEmpty(filename))
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
                    result = AddIM(title, context, filename);
                else
                    result = EidtIM(ID, title, context, filename);

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

        private bool AddIM(string title, string context, string fileName)
        {
            if (!System.IO.File.Exists(string.Format("{0}{1}", base.ImgBasePath, fileName)))
                return false;

            Images im = new Images();
            im.Context = context;
            im.Path = fileName;
            im.Title = title;

            ImageService.Add(im);
            return true;
        }

        private bool EidtIM(int id, string title, string context, string fileName)
        {
            Images im = ImageService.GetByID(id);

            System.IO.File.Delete(string.Format("{0}{1}", base.ImgBasePath, im.Path));

            im.Path = fileName;
            im.Title = title;
            im.Context = context;

            ImageService.Modify(im);
            return true;
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> DeleteImage(int id)
        {
            try
            {
                ImageService.Delete(id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        public PartialViewResult ImageShow(bool isNeedShowImage = false)
        {
            ViewBag.allImage = ImageService.GetByQuery(new QueryExpression<Images>()).ToList();
            ViewBag.isNeedShowImage = isNeedShowImage;
            return PartialView();
        }
        #endregion
    }
}
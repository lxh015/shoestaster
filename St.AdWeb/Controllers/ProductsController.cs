using St.Code;
using St.Domain.Entity.Product;
using St.Service;
using St.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static St.Code.WebContextInfo;

namespace St.AdWeb.Controllers
{
    public class ProductsController : BaseController
    {
        private IProductClassInterface ProductClassService = Ioc.GetService<IProductClassInterface>();
        private IProductClassIntroductionInterface ProductClassIntroductionService = Ioc.GetService<IProductClassIntroductionInterface>();
        private IProductsInterface ProductService = Ioc.GetService<IProductsInterface>();
        private IImagesInterface ImageService = Ioc.GetService<IImagesInterface>();
        private IProductImagesInterface ProductImageService = Ioc.GetService<IProductImagesInterface>();

        #region 产品
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult ProductList(int page = 0)
        //{
        //    BaseListResult<Products> Data = new BaseListResult<Products>();
        //    try
        //    {
        //        List<Products> dataList = new List<Products>();
        //        Code.QueryExpression<Products> query = new QueryExpression<Products>(p => p.ID != 0);

        //        dataList = ProductService.QueryForPage(page, query);
        //        if (dataList.Count == 0)
        //            Data.SetError();
        //        else
        //            Data.SetData(dataList);
        //    }
        //    catch
        //    {
        //        Data.SetError();
        //    }
        //    return Json(Data, JsonRequestBehavior.DenyGet);
        //}

        [HttpPost]
        public ActionResult ProductQueryList(string search = "", int page = 0)
        {
            BaseListResult<Products> Data = new BaseListResult<Products>();
            try
            {
                List<Products> dataList = new List<Products>();
                QueryExpression<Products> query = new QueryExpression<Products>();

                if (!string.IsNullOrEmpty(search))
                {
                    double money = 0d;
                    bool result = double.TryParse(search, out money);

                    if (result)
                        query.AddExperssion(new ExpressionSpecification<Products>(p => p.minPrice <= money && p.maxPrice >= money));
                    else
                        query.AddExperssion(new ExpressionSpecification<Products>(p => p.Name.Contains(search) || p.ClassIntroduction.Contains(search)));
                }

                dataList = ProductService.QueryForPage(page, query);
                Data.DataSumCount = Convert.ToInt32(Math.Round(Convert.ToDouble(query.PageSumCount) / Convert.ToDouble(query.PageCountNumber)));

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
        public ActionResult ProductEidt(bool isAdd = true, int id = 0)
        {
            ViewBag.Type = isAdd;
            ViewBag.Id = id;

            ViewBag.Pt = null;
            if (id != 0)
            {
                ViewBag.Pt = ProductService.GetByIDForInclude(id, "productClass");
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ProductEidtEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var name = PostHttp("name");
                var isshow = PostHttp("isshow");
                var audit = PostHttp("audit");
                var minprice = PostHttp("minprice");
                var maxprice = PostHttp("maxprice");
                var pcId = PostHttp("pcId");
                var introduction = PostHttp("introduction");
                var context = PostHttp("context");
                var pciName = PostHttp("pciName");
                var imageArray = PostHttp("imageArray");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(name) || string.IsNullOrEmpty(isshow) ||
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
                    result = AddP(name, isshow, audit, minprice, maxprice, pcId, introduction, context, pciName, imageArray);
                else
                    result = EidtP(ID, name, isshow, audit, minprice, maxprice, pcId, introduction, context, pciName, imageArray);

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

        private bool AddP(string name, string isshow, string audit, string minprice,
            string maxprice, string pcId, string introduction, string context, string pciName, string imageArray)
        {
            Products pc = new Products();
            if (string.IsNullOrEmpty(pcId))
                return false;

            pc.ClassIntroduction = pciName;
            pc.Context = context;
            pc.Introduction = introduction;
            pc.isShow = Convert.ToBoolean(isshow);
            pc.maxPrice = Convert.ToDouble(maxprice);
            pc.minPrice = Convert.ToDouble(minprice);
            pc.Name = name;
            pc.productClass = ProductClassService.GetByID(Convert.ToInt32(pcId));
            pc.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);

            //if (!string.IsNullOrEmpty(imageArray))
            //{
            //    string[] array = imageArray.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //    if (array.Length != 0)
            //    {
            //        ISpecification<Domain.Entity.Picture.Images> specification = 
            //            new NoneSpecification<Domain.Entity.Picture.Images>();
            //        foreach (var item in array)
            //        {
            //            specification.And(new ExpressionSpecification<Domain.Entity.Picture.Images>(p => p.ID == Convert.ToInt32(item)));
            //        }

            //        List<Domain.Entity.Picture.Images> images = ImageService.GetByQuery(new QueryExpression<Domain.Entity.Picture.Images>(specification.GetExpression()));
            //    }
            //}

            ProductService.Add(pc);
            return true;
        }


        #region MyRegion
        object lockObj = new object();
        public void AddPForTest()
        {
            Console.WriteLine($"开始 当前时间为{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            for (int i = 0; i < 10000; i++)
            {
                lock (lockObj)
                {
                    Products pc = new Products();

                    pc.ClassIntroduction =ComFunc.GetEnglistCodeString(10);
                    pc.Context = ComFunc.GetEnglistCodeString(10);
                    pc.Introduction = ComFunc.GetEnglistCodeString(10);
                    pc.isShow = ComFunc.random.Next(1, 3) % 2 == 0 ? true : false;
                    pc.maxPrice = ComFunc.random.Next(1, 30000);
                    pc.minPrice = pc.maxPrice - ComFunc.random.Next(1, 111);
                    pc.Name = ComFunc.GetEnglistCodeString(4);
                    pc.productClass = ProductClassService.GetByID(1);
                    pc.Stata = (Domain.Entity.AuditState)Convert.ToInt32(1);

                    ProductService.Add(pc);
                    Console.WriteLine($"第{i}次 当前时间{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                }
            }
            Console.WriteLine($"结束 当前时间为{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        } 
        #endregion

        private bool EidtP(int id, string name, string isshow, string audit, string minprice,
            string maxprice, string pcId, string introduction, string context, string pciName, string imageArray)
        {
            Products pc = ProductService.GetByID(id);
            if (string.IsNullOrEmpty(pcId))
                return false;

            pc.Name = name;
            pc.isShow = Convert.ToBoolean(isshow);
            pc.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);
            pc.minPrice = Convert.ToDouble(minprice);
            pc.maxPrice = Convert.ToDouble(maxprice);
            pc.productClass = new ProductClass() { ID = Convert.ToInt32(pcId) };

            pc.Introduction = introduction;
            pc.Context = context;
            pc.ClassIntroduction = pciName;

            ProductService.Modify(pc);
            return true;
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                ProductService.Delete(id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 产品图片
        public ActionResult ProductImageEdit(int pid, int piId = 0, bool isAdd = true, int iid = 0)
        {
            ViewBag.Pid = pid;
            ViewBag.Type = isAdd;
            ViewBag.Id = piId;

            ViewBag.Pti = null;
            if (piId != 0)
            {
                if (iid == 0)
                    ViewBag.Pti = ProductImageService.GetByID(piId);
                else
                    ViewBag.Pti = ProductImageService.GetByIDForInclude(piId, "Image");
            }

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ProductImageEidtEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var name = PostHttp("name");
                var pid = PostHttp("pid");
                var image = PostHttp("image");
                var link = PostHttp("link");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
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
                    result = AddPI(name, pid, image, link);
                else
                    result = EidtPI(ID, name, pid, image, link);

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

        private bool AddPI(string name, string pid, string image, string link)
        {
            ProductImages pci = new ProductImages();
            if (string.IsNullOrEmpty(pid))
                return false;

            pci.Name = name;
            pci.Alink = link;
            pci.Image = new Domain.Entity.Picture.Images() { ID = Convert.ToInt32(image) };
            pci.product = new Products() { ID = Convert.ToInt32(pid) };

            ProductImageService.Add(pci);
            return true;
        }

        private bool EidtPI(int id, string name, string pid, string image, string link)
        {
            ProductImages pci = ProductImageService.GetByID(id);
            if (string.IsNullOrEmpty(pid))
                return false;

            pci.Name = name;
            pci.Image = new Domain.Entity.Picture.Images() { ID = Convert.ToInt32(image) };
            pci.Alink = link;
            pci.product = new Products() { ID = Convert.ToInt32(pid) };

            ProductImageService.Modify(pci);
            return true;
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProductImage(int pid, int id)
        {
            try
            {
                ProductImageService.DeleteImage(pid, id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        public ActionResult ProductsImageList(int pid)
        {
            ViewBag.PC = ProductService.GetByIDForInclude(pid, "productImages");

            return View();
        }

        public ActionResult ImageGet()
        {
            WebContextInfo.BaseListResult<ProductImages> result = new WebContextInfo.BaseListResult<ProductImages>();
            try
            {
                string idArray = PostHttp("pimageArray");
                if (string.IsNullOrEmpty(idArray))
                {
                    result.Result = false;
                    goto Ret;
                }

                result.Data = ProductImageService.GetProductImagesIncludeImage(idArray.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                result.Result = true;
            }
            catch
            {
                result.Result = false;
                goto Ret;
            }

            Ret:
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 类别
        public ActionResult ProductClassIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductClassList(int page = 0)
        {
            BaseListResult<ProductClass> Data = new BaseListResult<ProductClass>();
            try
            {
                List<ProductClass> dataList = new List<ProductClass>();
                Code.QueryExpression<ProductClass> query = new QueryExpression<ProductClass>();

                dataList = ProductClassService.QueryForPage(page, query);
                Data.DataSumCount = Convert.ToInt32(Math.Round(Convert.ToDouble(query.PageSumCount) / Convert.ToDouble(query.PageCountNumber)));

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
        public ActionResult ProductClassEidt(bool isAdd = true, int id = 0)
        {
            ViewBag.Type = isAdd;
            ViewBag.Id = id;

            ViewBag.PC = null;
            if (id != 0)
            {
                ViewBag.PC = ProductClassService.GetByID(id);
            }

            return View();
        }

        public async Task<ActionResult> ProductClassEidtEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var name = PostHttp("name");
                var isshow = PostHttp("isshow");
                var audit = PostHttp("audit");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(name) || string.IsNullOrEmpty(isshow) ||
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
                    result = AddPC(name, isshow, audit);
                else
                    result = EidtPC(ID, name, isshow, audit);

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

        private bool AddPC(string name, string isshow, string audit)
        {
            ProductClass pc = new ProductClass();
            pc.isShow = Convert.ToBoolean(isshow);
            pc.Name = name;
            pc.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);
            pc.productClassIntroduction = new List<ProductClassIntroduction>();

            ProductClassService.Add(pc);
            return true;
        }

        private bool EidtPC(int id, string name, string isshow, string audit)
        {
            ProductClass pc = new ProductClass();
            pc.ID = id;
            pc.isShow = Convert.ToBoolean(isshow);
            pc.Name = name;
            pc.Stata = (Domain.Entity.AuditState)Convert.ToInt32(audit);

            ProductClassService.Modify(pc);
            return true;
        }
        #endregion

        public PartialViewResult ProductClassShow()
        {
            ViewBag.PcList = ProductClassService.GetByQuery(new QueryExpression<ProductClass>()).ToList();

            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProductClass(int id)
        {
            try
            {
                ProductClassService.Delete(id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 类别描述
        public ActionResult ProductClassDescript()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductClassDescriptList(int page = 0)
        {
            BaseListResult<ProductClassIntroduction> Data = new BaseListResult<ProductClassIntroduction>();
            try
            {
                List<ProductClassIntroduction> dataList = new List<ProductClassIntroduction>();
                Code.QueryExpression<ProductClassIntroduction> query = new QueryExpression<ProductClassIntroduction>();

                dataList = ProductClassIntroductionService.QueryForPage(page, query);
                Data.DataSumCount = Convert.ToInt32(Math.Round(Convert.ToDouble(query.PageSumCount) / Convert.ToDouble(query.PageCountNumber)));

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
        public ActionResult ProductClassDescriptEidt(bool isAdd = true, int id = 0)
        {
            ViewBag.Type = isAdd;
            ViewBag.Id = id;


            ViewBag.PCI = null;
            if (id != 0)
            {
                ViewBag.PCI = ProductClassIntroductionService.GetByIDForInclude(id, "productClass");
            }

            return View();
        }


        public async Task<ActionResult> ProductClassDescriptEidtEnd()
        {
            try
            {
                var id = PostHttp("id");
                var type = PostHttp("type");
                var name = PostHttp("name");
                var pcid = PostHttp("pcid");
                var description = PostHttp("description");

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pcid) ||
                    string.IsNullOrEmpty(description))
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
                    result = ProductClassDescriptAddPC(name, pcid, description);
                else
                    result = ProductClassDescriptEidtPC(ID, name, pcid, description);

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

        private bool ProductClassDescriptAddPC(string name, string pcid, string description)
        {
            ProductClass pc = ProductClassService.GetByID(Convert.ToInt32(pcid));
            if (pc.productClassIntroduction == null)
                pc.productClassIntroduction = new List<ProductClassIntroduction>();
            pc.productClassIntroduction.Add(new ProductClassIntroduction() { Description = description, productClass = pc });

            ProductClassIntroductionService.AddPCI(pc);
            return true;
        }

        private bool ProductClassDescriptEidtPC(int id, string name, string pcid, string description)
        {
            ProductClass pc = ProductClassService.GetByIDForInclude(Convert.ToInt32(pcid), "productClassIntroduction");
            pc.productClassIntroduction.Single(p => p.ID == id).Description = description;
            //pci.Description = description;

            ProductClassIntroductionService.ModifyPCI(pc);
            return true;
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> DeleteProductClassDescript(int id)
        {
            try
            {
                ProductClassIntroductionService.Delete(id);
                _baseResult.SetResult(true, "操作成功！");
            }
            catch
            {
                _baseResult.SetResult(false, "操作失败！");
            }
            await RSetBaseResult();
            return Json(_baseResult, JsonRequestBehavior.DenyGet);
        }

        public PartialViewResult ProductClassDescriptShow(int classID)
        {
            QueryExpression<ProductClassIntroduction> queryShow = new QueryExpression<ProductClassIntroduction>();
            queryShow.AddExperssion(new ExpressionSpecification<ProductClassIntroduction>(p => p.productClass.ID == classID));
            ViewBag.PciList = ProductClassIntroductionService.GetByList(queryShow, "productClass").ToList();

            return PartialView();
        }

        #endregion
    }
}
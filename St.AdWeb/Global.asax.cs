using St.Code;
using St.Domain;
using St.Service;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace St.AdWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //ExceptionLogAttribute继承自HandleError，主要作用是将异常信息写入日志系统中
            GlobalFilters.Filters.Add(new St.Code.StException.ExceptionHandleAttribute());
            //默认的异常记录类
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            
            //IOC
            Ioc.RegisterInheritedTypes(typeof(ServiceBase<>).Assembly, typeof(ServiceBase<>));
            Ioc.RegisterInheritedTypes(typeof(St.Code.LogHandle.LogHandle).Assembly, typeof(St.Code.LogHandle.LogHandle));
            
            //EntityFramework预热
            using (var dbcontext = new TasterDbContext())
            {
                var objectContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
                var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                mappingCollection.GenerateViews(new List<EdmSchemaError>());
            }
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)//这里是允许本地访问启动监控,可不写
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}

using St.Code;
using St.Domain;
using St.Service;
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
    }
}

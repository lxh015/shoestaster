using St.Code;
using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IProductClassInterface : IServiceBase<ProductClass>, IQueryForPage<ProductClass>,IInclude<ProductClass>
    {

    }
}

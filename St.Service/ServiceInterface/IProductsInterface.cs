using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IProductsInterface: IServiceBase<Products>,IQueryForPage<Products>,IInclude<Products>,IDateBase<Products>
    {
        Products GetProductsImage(int id);
    }
}

using St.Code;
using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IProductClassIntroductionInterface : IServiceBase<ProductClassIntroduction>, IQueryForPage<ProductClassIntroduction>,IInclude<ProductClassIntroduction>
    {
        void AddPCI(ProductClass entity);

        void ModifyPCI(ProductClass entity);

        List<ProductClassIntroduction> GetByList(QueryExpression<ProductClassIntroduction> query, string include);
    }
}

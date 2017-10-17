using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.Product;
using System.Data.Entity;

namespace St.Service.Implementations
{
    public class ProductClassImplementation : ServiceBase<ProductClass>, IProductClassInterface
    {
        public List<ProductClass> QueryForPage(int Page, QueryExpression<ProductClass> Query)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<ProductClass>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions.GetExpression());
                Query.PageSumCount = pageQuery.Count();

                int skip = Page * Query.PageCountNumber;
                if (skip > Query.PageSumCount)
                    return new List<ProductClass>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Query.PageCountNumber).ToList();
            }
        }

        new public void Modify(ProductClass entity)
        {
            using (var db = this.NewDB())
            {
                for (int i = 0; i < entity.productClassIntroduction.Count; i++)
                {
                    var itemEntry = db.Entry<ProductClassIntroduction>(entity.productClassIntroduction[i]);
                    itemEntry.State = EntityState.Added;
                }

                var entry = db.Entry(entity);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ProductClass GetByIDForInclude(int id, string include)
        {
            using (var db = base.NewDB())
            {
                return db.Set<ProductClass>().AsNoTracking().Where(p=>p.ID==id).Include(include).Single(p => p.ID == id);
            }
        }
    }
}

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
    public class ProductClassIntroductionImplementation : ServiceBase<ProductClassIntroduction>, IProductClassIntroductionInterface
    {
        public List<ProductClassIntroduction> QueryForPage(int Page, QueryExpression<ProductClassIntroduction> Query)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<ProductClassIntroduction>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions.GetExpression());
                int skip = Page * Query.PageCountNumber;
                if (skip > pageQuery.Count())
                    return new List<ProductClassIntroduction>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Query.PageCountNumber).ToList();
            }
        }

        public void AddPCI(ProductClass entity)
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

        public void ModifyPCI(ProductClass entity)
        {
            using (var db = this.NewDB())
            {
                for (int i = 0; i < entity.productClassIntroduction.Count; i++)
                {
                    var itemEntry = db.Entry<ProductClassIntroduction>(entity.productClassIntroduction[i]);
                    itemEntry.State = EntityState.Modified;
                }

                var entry = db.Entry(entity);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ProductClassIntroduction GetByIDForInclude(int id, string include)
        {
            using (var db = base.NewDB())
            {
                return db.Set<ProductClassIntroduction>().AsNoTracking().Where(p => p.ID == id).Include(include).Single(p => p.ID == id);
            }
        }

        public List<ProductClassIntroduction> GetByList(QueryExpression<ProductClassIntroduction> query, string include)
        {
            using (var db=base.NewDB())
            {
                return db.Set<ProductClassIntroduction>().AsNoTracking()
                    .Include(include)
                    .Where(query.QueryExpressions.GetExpression()).ToList();
            }
        }
    }
}

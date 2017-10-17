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
    public class ProductsImplementation : ServiceBase<Products>, IProductsInterface
    {
        public Products GetByIDForInclude(int id, string include)
        {
            using (var db = base.NewDB())
            {
                return db.Set<Products>().AsNoTracking().Where(p => p.ID == id).Include(include).Single(p => p.ID == id);
            }
        }

        public Products GetByIDForInclude<TProperty>(int id, System.Linq.Expressions.Expression<Func<Products, TProperty>> expression)
        {
            using (var db = base.NewDB())
            {
                return db.Set<Products>().AsNoTracking().Where(p => p.ID == id).Include(expression).Single(p => p.ID == id);
            }
        }

        public List<Products> QueryForPage(int Page, QueryExpression<Products> Query)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<Products>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions.GetExpression());
                Query.PageSumCount = pageQuery.Count();

                int skip = Page * Query.PageCountNumber;
                if (skip > Query.PageSumCount)
                    return new List<Products>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Query.PageCountNumber).ToList();
            }
        }

        new public void Delete(int id)
        {
            using (var db=base.NewDB())
            {
                var entity = GetByIDForInclude(id, p => p.productClass);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        new public void Add(Products entity)
        {
            using (var db = base.NewDB())
            {
                SetData(entity);
                db.Entry(entity.productClass).State = EntityState.Unchanged;
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
            }
        }

        new public void Modify(Products entity)
        {
            using (var db = base.NewDB())
            {
                var old = db.Products.Single(p => p.ID == entity.ID);
                var oldPreproties = old.GetType().GetProperties();
                var newPreproties = entity.GetType().GetProperties();
                foreach (var olditem in oldPreproties)
                {
                    foreach (var item in newPreproties)
                    {
                        if (olditem.Name == item.Name)
                        {
                            olditem.SetValue(old, item.GetValue(entity));
                            break;
                        }
                    }
                }
                old.productClass = db.ProductClass.Single(p => p.ID == entity.productClass.ID);
                SetData(old, DateType.Update);
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public Products GetProductsImage(int id)
        {
            using (var db=base.NewDB())
            {
                return db.Set<Products>().AsNoTracking().Where(p => p.ID == id).Include(p => p.productImages).Single(p => p.ID == id);
            }
        }
        
        public void SetData(Products entity, DateType type = DateType.Add)
        {
            if (type == DateType.Add)
                entity.AddDateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
        }
    }
}

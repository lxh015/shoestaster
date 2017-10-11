using St.Code;
using St.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public abstract class ServiceBase<T> : IServiceBase<T> where T : St.Domain.Entity.BaseID, new()
    {
        protected TasterDbContext NewDB()
        {
            return new TasterDbContext();
        }

        public void Add(T entity)
        {
            using (var db = this.NewDB())
            {
                db.Entry<T>(entity).State = EntityState.Added;
                db.SaveChanges();
            }
        }

        protected void Add(T entity, TasterDbContext db)
        {
            db.Entry<T>(entity).State = EntityState.Added;
            db.SaveChanges();
        }

        public void Modify(T entity)
        {
            using (var db = this.NewDB())
            {
                var entry = db.Entry<T>(entity);
                //entry.CurrentValues.SetValues(entity);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        protected void Modify(T entity, TasterDbContext db)
        {
            var entry= db.Entry<T>(entity);
            db.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            using (var db = this.NewDB())
            {
                var entity = new T() { ID = id };
                db.Entry<T>(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }


        protected void Delete(int id, TasterDbContext db)
        {
            var entity = new T() { ID = id };
            db.Entry<T>(entity).State = EntityState.Deleted;
            db.SaveChanges();
        }

        public T GetByID(int id)
        {
            using (var db = this.NewDB())
            {
                return db.Set<T>().AsNoTracking().Single(p => p.ID == id);
            }
        }


        public List<T> GetByQuery(QueryExpression<T> query)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<T>().AsNoTracking().AsQueryable();
                return pageQuery.Where(query.QueryExpressions.GetExpression()).ToList();
                //var pageQuery = db.Set<T>().AsNoTracking().AsQueryable();
                //if (query.DeleteTag != 2)
                //    pageQuery = pageQuery.Where(p => p.DeleteTag == query.DeleteTag);
                //query.TotalRecordCount = pageQuery.Count();
                //return pageQuery.OrderBy(p => p.CreatedTime).Page(query.PageIndex, query.PageSize).ToList();
            }
        }
    }
}

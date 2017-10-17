using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.AD;
using System.Data.Entity;

namespace St.Service.Implementations
{
    public class AdsImplementation : ServiceBase<Ads>, IAdsInterface
    {
        public void Add(Ads ads, string imageId)
        {
            using (var db = base.NewDB())
            {
                var iid = Convert.ToInt32(imageId);
                ads.image = db.Images.AsNoTracking().Single(p => p.ID == iid);
                SetData(ads);
                db.Entry(ads.image).State = EntityState.Unchanged;
                db.Entry(ads).State = EntityState.Added;
                db.SaveChanges();
            }
        }

        public Ads GetByIDForInclude(int id, string include)
        {
            using (var db = base.NewDB())
            {
                return db.Ads.AsNoTracking().Where(p => p.ID == id).Include(include).First();
            }
        }

        public void Modify(Ads ads, string imageId)
        {
            using (var db = base.NewDB())
            {
                var old = db.Ads.Single(p => p.ID == ads.ID);
                St.Code.ValueClone.Clone(old, ads);
                int iid = Convert.ToInt32(imageId);
                old.image = db.Images.AsNoTracking().Single(p => p.ID == iid);
                SetData(old, DateType.Update);
                db.Entry(old.image).State = EntityState.Unchanged;
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public List<Ads> QueryForPage(int Page, QueryExpression<Ads> Query)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<Ads>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions.GetExpression());
                Query.PageSumCount = pageQuery.Count();
                int skip = Page * Query.PageCountNumber;
                if (skip > pageQuery.Count())
                    return new List<Ads>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Query.PageCountNumber).ToList();
            }
        }

        public void SetData(Ads entity, DateType type = DateType.Add)
        {
            if (type == DateType.Add)
                entity.AddDateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.AD;
using System.Data.Entity;

namespace St.Service.Implementations
{
    public class AdsImplementation :ServiceBase<Ads> ,IAdsInterface
    {
        public void Add(Ads ads, string imageId)
        {
            using (var db=base.NewDB())
            {
                var iid = Convert.ToInt32(imageId);
                ads.image= db.Images.AsNoTracking().Single(p => p.ID == iid);
                db.Entry(ads.image).State = EntityState.Unchanged;
                db.Entry(ads).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
        }

        public Ads GetByIDForInclude(int id, string include)
        {
            using (var db=base.NewDB())
            {
                return db.Ads.AsNoTracking().Where(p=>p.ID==id).Include(include).First();
            }
        }

        public void Modify(Ads ads, string imageId)
        {
            using (var db = base.NewDB())
            {
                var old = db.Ads.Single(p=>p.ID==ads.ID);
                St.Code.ValueClone.Clone(old, ads);
                int iid = Convert.ToInt32(imageId);
                old.image = db.Images.AsNoTracking().Single(p => p.ID == iid);
                db.Entry(old.image).State = EntityState.Unchanged;
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public List<Ads> QueryForPage(int Page, QueryExpression<Ads> Query, int Count = 15)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<Ads>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions);
                int skip = Page * Count;
                if (skip > pageQuery.Count())
                    return new List<Ads>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Count).ToList();
            }
        }
    }
}

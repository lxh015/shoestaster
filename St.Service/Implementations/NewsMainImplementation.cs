using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.News;
using System.Data.Entity;

namespace St.Service.Implementations
{
    public class NewsMainImplementation : ServiceBase<NewsMain>, INewsMainInterface
    {
        public void Add(NewsMain newsMain, string imageArray)
        {
            using (var db = base.NewDB())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    if (!String.IsNullOrEmpty(imageArray))
                    {
                        NewsShow ns = new NewsShow();
                        ns.newsMain = newsMain;
                        ns.Summary = newsMain.Summary;
                        ns.Title = newsMain.Title;

                        var IArray = imageArray.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        St.Domain.Entity.Picture.Images image = null;
                        foreach (var item in IArray)
                        {
                            try
                            {
                                image = db.Images.AsNoTracking().Where(p => p.Title == item).First();
                            }
                            catch
                            {
                            }
                        }
                        ns.images = image;

                        newsMain.newShow = ns;
                    }
                    db.Entry<NewsMain>(newsMain).State = EntityState.Added;
                    db.SaveChanges();

                    trans.Commit();
                }
            }
        }

        public NewsMain GetByIDForInclude(int id, string include)
        {
            using (var db = base.NewDB())
            {
                return db.Set<NewsMain>().AsNoTracking().Where(p => p.ID == id).Include(include).Include(p => p.newShow.images).Single(p => p.ID == id);
            }
        }

        public NewsMain GetByIDForInclude<TProperty>(int id, System.Linq.Expressions.Expression<Func<NewsMain, TProperty>> expression)
        {
            using (var db = base.NewDB())
            {
                return db.Set<NewsMain>().AsNoTracking().Where(p => p.ID == id).Include(expression).Single(p => p.ID == id);
            }
        }

        public void Modify(NewsMain newsMain, string imageArray)
        {
            using (var db = base.NewDB())
            {
                var old = db.NewsMain.Where(p => p.ID == newsMain.ID).Include(p => p.newShow).Include(p => p.newShow.images).First();

                old.Title = newsMain.Title;
                old.Summary = newsMain.Summary;
                old.Stata = newsMain.Stata;
                old.IsShow = newsMain.IsShow;
                old.Context = newsMain.Context;

                old.newShow.Summary = newsMain.Summary;
                old.newShow.Title = newsMain.Title;
                
                if (!string.IsNullOrEmpty(imageArray))
                {
                    string[] array = imageArray.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in array)
                    {
                        var intItem = Convert.ToInt32(item);
                        old.newShow.images = db.Images.Single(p => p.ID == intItem);
                    }
                }
                else
                    old.newShow.images = null;
                db.Entry(old.newShow.images).State = EntityState.Unchanged;
                db.Entry(old.newShow).State = EntityState.Modified;
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public List<NewsMain> QueryForPage(int Page, QueryExpression<NewsMain> Query, int Count = 15)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<NewsMain>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions);
                int skip = Page * Count;
                if (skip > pageQuery.Count())
                    return new List<NewsMain>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Count).ToList();
            }
        }


    }
}

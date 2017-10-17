using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.Picture;

namespace St.Service.Implementations
{
    public class ImagesImplementation : ServiceBase<Images>, IImagesInterface
    {
        public List<Images> QueryForPage(int Page, QueryExpression<Images> Query, int Count = 15)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<Images>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions.GetExpression());
                int skip = Page * Count;
                if (skip > pageQuery.Count())
                    return new List<Images>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Count).ToList();
            }
        }

        public List<Images> QueryForPage(int Page, QueryExpression<Images> Query)
        {
            using (var db = this.NewDB())
            {
                var pageQuery = db.Set<Images>().AsNoTracking().AsQueryable();
                return pageQuery.Where(Query.QueryExpressions.GetExpression()).ToList();
            }
        }

        new public void Add(Images eneity)
        {
            SetData(eneity);
            base.Add(eneity);
        }

        new public void Modify(Images entity)
        {
            SetData(entity, DateType.Update);
            base.Modify(entity);
        }

        public void SetData(Images entity, DateType type = DateType.Add)
        {
            if (type == DateType.Add)
                entity.AddDateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
        }
    }
}

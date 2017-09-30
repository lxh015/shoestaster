using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.News;

namespace St.Service.Implementations
{
    public class NewsShowImplementation : ServiceBase<NewsShow>, INewsShowInterface
    {
        public void Add(NewsShow entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public NewsShow GetByID(int Id)
        {
            throw new NotImplementedException();
        }

        public IList<NewsShow> GetByQuery(QueryExpression<NewsShow> query)
        {
            throw new NotImplementedException();
        }

        public void Modify(NewsShow entity)
        {
            throw new NotImplementedException();
        }
    }
}

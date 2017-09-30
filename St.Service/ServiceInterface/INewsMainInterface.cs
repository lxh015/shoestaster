using St.Domain.Entity.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface INewsMainInterface : IServiceBase<NewsMain>, IQueryForPage<NewsMain>, IInclude<NewsMain>
    {
        void Add(NewsMain newsMain, string imageArray);

        void Modify(NewsMain newsMain, string imageArray);
    }
}

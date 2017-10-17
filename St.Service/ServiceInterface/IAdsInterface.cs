using St.Domain.Entity.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IAdsInterface : IServiceBase<Ads>, IQueryForPage<Ads>, IInclude<Ads>, IDateBase<Ads>
    {
        void Add(Ads ads, string imageId);

        void Modify(Ads ads, string imageId);
    }
}

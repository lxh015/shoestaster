using St.Code;
using St.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IQueryForPage<T> where T : BaseID
    {
        List<T> QueryForPage(int Page, QueryExpression<T> Query);
    }
}

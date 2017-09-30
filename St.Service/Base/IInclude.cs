using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IInclude<T> where T:St.Domain.Entity.BaseID
    {
        T GetByIDForInclude(int id, string include);

      //  T GetByIDForInclude<TProperty>(int id, Expression<Func<T, TProperty>> expression);
    }
}

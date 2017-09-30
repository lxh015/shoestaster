using St.Code;
using St.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IServiceBase<T> where T : BaseID
    {
        void Add(T entity);

        void Modify(T entity);

        void Delete(int Id);

        T GetByID(int Id);

        List<T> GetByQuery(QueryExpression<T> query);
    }
}

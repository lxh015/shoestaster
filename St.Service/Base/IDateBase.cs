using St.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IDateBase<T> where T : BaseID, IDate
    {
        void SetData(T entity, DateType dateType = DateType.Add);
    }

    public enum DateType
    {
        Add,
        Update,
    }
}

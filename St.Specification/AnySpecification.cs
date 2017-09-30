using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace St.Specification
{
    public abstract class AnySpecification<T> : SpecificationBase<T>
    {
        public override Expression<Func<T, bool>> GetExpression()
        {
            return o => true;
        }
    }
}

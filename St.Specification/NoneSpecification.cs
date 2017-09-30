using System;
using System.Linq.Expressions;

namespace St.Specification
{
    public sealed class NoneSpecification<T> : SpecificationBase<T>
    {
        public override Expression<Func<T, bool>> GetExpression()
        {
            return o => true;
        }
    }
}

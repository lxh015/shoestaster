using System;
using System.Linq.Expressions;

namespace St.Specification
{

    public class OrSepcification<T> : CompositeSpecification<T>
    {
        public OrSepcification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        public override Expression<Func<T, bool>> GetExpression()
        {
            return Left.GetExpression().Or(Right.GetExpression());
        }
    }

}
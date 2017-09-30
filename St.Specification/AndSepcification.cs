using System;
using System.Linq.Expressions;

namespace St.Specification
{
    internal class AndSepcification<T> : CompositeSpecification<T>
    {
        public AndSepcification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        public override Expression<Func<T, bool>> GetExpression()
        {
            return Left.GetExpression().And(Right.GetExpression());
        }
    }
}
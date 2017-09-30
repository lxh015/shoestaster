using System;
using System.Linq.Expressions;

namespace St.Specification
{
    internal class NotSpecification<T> : SpecificationBase<T>
    {
        private ISpecification<T> _ispecification;

        public NotSpecification(ISpecification<T> specification)
        {
            this._ispecification = specification;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            var body = Expression.Not(this._ispecification.GetExpression().Body);

            return Expression.Lambda<Func<T, bool>>(body, this._ispecification.GetExpression().Parameters);
        }
    }
}
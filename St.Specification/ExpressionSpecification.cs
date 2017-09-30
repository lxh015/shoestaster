using System;
using System.Linq.Expressions;

namespace St.Specification
{
    public sealed class ExpressionSpecification<T> : SpecificationBase<T>
    {
        private Expression<Func<T, bool>> expression;

        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            this.expression = expression;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            return this.expression;
        }
    }
}
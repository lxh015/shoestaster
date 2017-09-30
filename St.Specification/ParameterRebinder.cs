using System.Collections.Generic;
using System.Linq.Expressions;

namespace St.Specification
{
    /// <summary>
    /// 代表的参数重新绑定用于重新绑定参数给定的表达式。这是解决方案的一部分,解决了参数的表达式问题时要使用Apworks实体框架规范
    /// </summary>
    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this._map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        internal static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression pexp)
        {
            ParameterExpression replacement;
            if (_map.TryGetValue(pexp, out replacement))
            {
                pexp = replacement;
            }

            return base.VisitParameter(pexp);
        }
    }
}

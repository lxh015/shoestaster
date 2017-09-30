using St.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace St.Code
{
    public class QueryExpression<T> where T : BaseID
    {
        public Expression<Func<T,bool>> QueryExpressions { get; private set; }

        public QueryExpression(Expression<Func<T, bool>> expression)
        {
            this.QueryExpressions = expression;
        }
    }
}

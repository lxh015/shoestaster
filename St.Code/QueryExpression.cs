﻿using St.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using St.Specification;

namespace St.Code
{
    public class QueryExpression<T> where T : BaseID
    {
        public ISpecification<T> QueryExpressions { get; private set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageCountNumber { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageSumCount { get; set; }

        public QueryExpression()
        {
            this.QueryExpressions = new ExpressionSpecification<T>(p => p.ID != 0);
            this.PageCountNumber = 15;
        }

        public QueryExpression(int pageCount):this()
        {
            this.PageCountNumber = pageCount;
        }

        public void AddExperssion(ExpressionSpecification<T> expression)
        {
            this.QueryExpressions = this.QueryExpressions.And(expression);
        }
    }
}

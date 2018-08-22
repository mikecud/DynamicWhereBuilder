using DynamicWhereBuilder.Models.QueryPart;
using System;
using System.Linq.Expressions;

namespace DynamicWhereBuilder.Models.Expression
{
    internal class ExpressionItem<T> : ExpressionBase<T>
    {
        internal ExpressionItem(Expression<Func<T, bool>> expression, LogicalOperator? logicalOperator = null)
        {
            this.Expression = expression;
            this.LogicalOperator = logicalOperator;
        }

        internal Expression<Func<T, bool>> Expression { get; private set; }
    }
}

using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Resources;
using System;
using System.Linq.Expressions;

namespace DynamicWhereBuilder.Extensions
{
    internal static class LogicalOperatorExtensions
    {
        internal static Func<Expression, Expression, Expression> ToExpressionMergingMethod(this LogicalOperator logicalOperator)
        {
            switch (logicalOperator)
            {
                case LogicalOperator.And:
                    return Expression.And;
                case LogicalOperator.Or:
                    return Expression.Or;
                default:
                    throw new NotImplementedException(string.Format(GeneralResources.Exception_UnknownLogicalOperator, logicalOperator));
            }
        }
    }
}

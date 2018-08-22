using DynamicWhereBuilder.Models.Expression;
using System;
using System.Linq.Expressions;

namespace DynamicWhereBuilder.Services.Interfaces
{
    internal interface IExpressionBuilder<T>
    {
        Expression<Func<T, bool>> Build(ExpressionCollection<T> expressionCollection);
    }
}

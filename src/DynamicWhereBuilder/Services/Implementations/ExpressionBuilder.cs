using DynamicWhereBuilder.Extensions;
using DynamicWhereBuilder.Libs;
using DynamicWhereBuilder.Models.Expression;
using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicWhereBuilder.Services.Implementations
{
    internal class ExpressionBuilder<T> : IExpressionBuilder<T>
    {
        public Expression<Func<T, bool>> Build(ExpressionCollection<T> expressionCollection)
        {
            Expression<Func<T, bool>> expression = null;

            foreach(var expressionBase in expressionCollection)
            {
                switch (expressionBase)
                {
                    case ExpressionItem<T> _:
                        expression = this.appendToExpression(expression, (expressionBase as ExpressionItem<T>).Expression, expressionBase.LogicalOperator);
                        break;
                    case ExpressionCollection<T> _:
                        var collectionExpressionBuilder = new ExpressionBuilder<T>();
                        var collectionExpression = collectionExpressionBuilder.Build(expressionBase as ExpressionCollection<T>);

                        expression = this.appendToExpression(expression, collectionExpression, expressionBase.LogicalOperator);
                        break;
                    default:
                        throw new NotImplementedException(string.Format(GeneralResources.Exception_UnknownSubtypeOfExpressionBase, expressionBase.GetType()));
                }
            }

            return expression;
        }

        private LogicalOperator? previousLogicalOperator { get; set; }

        private Expression<Func<T, bool>> appendToExpression(Expression<Func<T, bool>> expression, Expression<Func<T, bool>> expressionToAppend, LogicalOperator? nextLogicalOperator)
        {
            if (expression == null)
            {
                this.previousLogicalOperator = nextLogicalOperator;
                return expressionToAppend;
            }

            var parametersMap = expression.Parameters
                .Select((e, i) => new Tuple<ParameterExpression, ParameterExpression>(expressionToAppend.Parameters.ElementAt(i), e))
                .ToDictionary(x => x.Item1, x => x.Item2);

            var rebinderVisitor = new RebinderVisitor(parametersMap);
            var expressionToAppendBodyRebound = rebinderVisitor.RebindForExpression(expressionToAppend.Body);
            var combiningMethod = this.previousLogicalOperator.Value.ToExpressionMergingMethod();

            return Expression.Lambda<Func<T, bool>>(combiningMethod(expression.Body, expressionToAppendBodyRebound), expression.Parameters);
        }
    }
}

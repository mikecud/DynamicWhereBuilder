using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DynamicWhereBuilder.Libs
{
    internal class RebinderVisitor : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _parametersMap = new Dictionary<ParameterExpression, ParameterExpression>();

        public RebinderVisitor(Dictionary<ParameterExpression, ParameterExpression> parametersMap)
        {
            if (parametersMap == null)
            {
                throw new ArgumentNullException(nameof(parametersMap));
            }

            this._parametersMap = parametersMap;
        }

        public Expression RebindForExpression(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (this._parametersMap.ContainsKey(p))
            {
                p = this._parametersMap[p];
            }

            return base.VisitParameter(p);
        }
    }
}

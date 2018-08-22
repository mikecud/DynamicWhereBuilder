using System;
using System.Linq.Expressions;

namespace DynamicWhereBuilder.Models.Sequence
{
    internal class ExpressionSymbol<T> : ISequenceSymbol
    {
        public int QueryPartIndex { get; set; }

        internal Expression<Func<T, bool>> Expression { get; private set; }

        internal ExpressionSymbol(int queryIndex, Expression<Func<T, bool>> expression)
        {
            this.QueryPartIndex = queryIndex;
            this.Expression = expression;
        }
    }
}

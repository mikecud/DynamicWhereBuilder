using DynamicWhereBuilder.Models.QueryPart;
using System.Collections;
using System.Collections.Generic;

namespace DynamicWhereBuilder.Models.Expression
{
    internal class ExpressionCollection<T> : ExpressionBase<T>, IEnumerable<ExpressionBase<T>>
    {
        internal ExpressionCollection(LogicalOperator? logicalOperator = null)
        {
            this.LogicalOperator = logicalOperator;
        }

        private List<ExpressionBase<T>> list = new List<ExpressionBase<T>>();

        internal ExpressionBase<T> this[int index]
        {
            get { return this.list[index]; }
            set { this.list.Insert(index, value); }
        }

        internal void Add(ExpressionBase<T> expressionBase)
        {
            this.list.Add(expressionBase);
        }

        public IEnumerator<ExpressionBase<T>> GetEnumerator()
        {
            return this.list.GetEnumerator() as IEnumerator<ExpressionBase<T>>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

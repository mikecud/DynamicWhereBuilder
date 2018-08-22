using DynamicWhereBuilder.Models.QueryPart;

namespace DynamicWhereBuilder.Models.Expression
{
    internal abstract class ExpressionBase<T>
    {
        public LogicalOperator? LogicalOperator { get; internal protected set; }
    }
}

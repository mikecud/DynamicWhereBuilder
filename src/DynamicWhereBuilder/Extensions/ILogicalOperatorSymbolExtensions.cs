using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Resources;
using System;

namespace DynamicWhereBuilder.Extensions
{
    internal static class ILogicalOperatorSymbolExtensions
    {
        internal static LogicalOperator ToLogicalOperator(this ILogicalOperatorSymbol symbol)
        {
            switch (symbol)
            {
                case LogicalOperatorAndSymbol _:
                    return LogicalOperator.And;
                case LogicalOperatorOrSymbol _:
                    return LogicalOperator.Or;
                default:
                    throw new NotImplementedException(string.Format(GeneralResources.Exception_UnknownLogicalOperatorSymbol, symbol.GetType()));
            }
        }
    }
}

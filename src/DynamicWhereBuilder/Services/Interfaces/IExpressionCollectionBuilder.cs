using DynamicWhereBuilder.Models.Expression;
using DynamicWhereBuilder.Models.Sequence;
using System.Collections.Generic;

namespace DynamicWhereBuilder.Services.Interfaces
{
    internal interface IExpressionCollectionBuilder<T>
    {
        ExpressionCollection<T> Build(List<ISequenceSymbol> sequenceOfSymbols);
    }
}

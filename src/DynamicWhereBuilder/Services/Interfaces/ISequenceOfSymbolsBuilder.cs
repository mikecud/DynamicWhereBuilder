using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Models.Sequence;
using System.Collections.Generic;

namespace DynamicWhereBuilder.Services.Interfaces
{
    internal interface ISequenceOfSymbolsBuilder<T>
    {
        List<ISequenceSymbol> Build(IEnumerable<QueryPart<T>> queryParts);
    }
}

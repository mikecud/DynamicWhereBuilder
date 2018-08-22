using DynamicWhereBuilder.Models.QueryPart;
using System.Collections.Generic;

namespace DynamicWhereBuilder.Services.Interfaces
{
    internal interface IQueryPartsValidator<T>
    {
        void Validate(IEnumerable<QueryPart<T>> queryParts);
    }
}

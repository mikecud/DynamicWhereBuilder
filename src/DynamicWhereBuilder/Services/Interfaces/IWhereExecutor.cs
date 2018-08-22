using DynamicWhereBuilder.Models.QueryPart;
using System.Collections.Generic;
using System.Linq;

namespace DynamicWhereBuilder.Services.Interfaces
{
    internal interface IWhereExecutor<T>
    {
        IQueryable<T> Execute(IQueryable<T> query, IEnumerable<QueryPart<T>> queryParts);

        IEnumerable<T> Execute(IEnumerable<T> collection, IEnumerable<QueryPart<T>> queryParts);
    }
}

using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicWhereBuilder.Services.Implementations
{
    internal class QueryPartsValidator<T> : IQueryPartsValidator<T>
    {
        public void Validate(IEnumerable<QueryPart<T>> queryParts)
        {
            if (queryParts == null)
                throw new ArgumentNullException(nameof(queryParts));

            if (queryParts.Count() == 0)
                throw new ArgumentOutOfRangeException(nameof(queryParts), ValidationErrorResources.QueryPart_YouShouldProvideAtLeastOneQueryPart);

            if (queryParts.All(x =>
                 !x.InitialParenthesis.HasValue &&
                 x.Expression == null &&
                 !x.EndingParenthesis.HasValue &&
                 !x.LogicalOperator.HasValue
            ))
                throw new ArgumentException(ValidationErrorResources.QueryPart_YouShouldProvideAtLeastOneQueryPartWithASymbol, nameof(queryParts));
        }
    }
}

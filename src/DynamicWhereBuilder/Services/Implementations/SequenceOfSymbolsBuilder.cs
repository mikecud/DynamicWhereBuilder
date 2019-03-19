using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicWhereBuilder.Services.Implementations
{
    internal class SequenceOfSymbolsBuilder<T> : ISequenceOfSymbolsBuilder<T>
    {
        public List<ISequenceSymbol> Build(IEnumerable<QueryPart<T>> queryParts)
        {
            var sequence = new List<ISequenceSymbol>();

            for (var i = 0; i < queryParts.Count(); i++)
            {
                var queryPart = queryParts.ElementAt(i);

                #region Initial Parenthesis

                if (queryPart.InitialParenthesis.HasValue)
                {
                    switch (queryPart.InitialParenthesis.Value)
                    {
                        case Parenthesis.Open:
                            sequence.Add(new ParenthesisOpenSymbol(i));
                            break;
                        case Parenthesis.Close:
                            sequence.Add(new ParenthesisCloseSymbol(i));
                            break;
                        default: // should not happen
                            throw new NotImplementedException(string.Format(GeneralResources.Exception_UnexpectedTokenDuringAnalysisOfQueryParts, queryPart.InitialParenthesis.Value));
                    }
                }

                #endregion

                #region Expression

                if (queryPart.Expression != null)
                    sequence.Add(new ExpressionSymbol<T>(i, queryPart.Expression));

                #endregion

                #region Ending parenthesis

                if (queryPart.EndingParenthesis.HasValue)
                {
                    switch (queryPart.EndingParenthesis.Value)
                    {
                        case Parenthesis.Open:
                            sequence.Add(new ParenthesisOpenSymbol(i));
                            break;
                        case Parenthesis.Close:
                            sequence.Add(new ParenthesisCloseSymbol(i));
                            break;
                        default: // should not happen
                            throw new NotImplementedException(string.Format(GeneralResources.Exception_UnexpectedTokenDuringAnalysisOfQueryParts, queryPart.EndingParenthesis.Value));
                    }
                }

                #endregion

                #region Logical Operator

                if (queryPart.LogicalOperator.HasValue)
                {
                    switch (queryPart.LogicalOperator.Value)
                    {
                        case LogicalOperator.And:
                            sequence.Add(new LogicalOperatorAndSymbol(i));
                            break;
                        case LogicalOperator.Or:
                            sequence.Add(new LogicalOperatorOrSymbol(i));
                            break;
                        default:
                            throw new NotImplementedException(string.Format(GeneralResources.Exception_UnexpectedTokenDuringAnalysisOfQueryParts, queryPart.LogicalOperator.Value));
                    }
                }

                #endregion
            }

            return sequence;
        }
    }
}

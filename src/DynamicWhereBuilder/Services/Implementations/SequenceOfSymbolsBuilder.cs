using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Services.Interfaces;
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
                    if (queryPart.InitialParenthesis.Value == Parenthesis.Open)
                        sequence.Add(new ParenthesisOpenSymbol(i));
                    else if (queryPart.InitialParenthesis.Value == Parenthesis.Close)
                        sequence.Add(new ParenthesisCloseSymbol(i));
                }

                #endregion

                #region Expression

                if (queryPart.Expression != null)
                    sequence.Add(new ExpressionSymbol<T>(i, queryPart.Expression));

                #endregion

                #region Ending parenthesis

                if (queryPart.EndingParenthesis.HasValue)
                {
                    if (queryPart.EndingParenthesis.Value == Parenthesis.Open)
                        sequence.Add(new ParenthesisOpenSymbol(i));
                    else if (queryPart.EndingParenthesis.Value == Parenthesis.Close)
                        sequence.Add(new ParenthesisCloseSymbol(i));
                }

                #endregion

                #region Logical Operator

                if (queryPart.LogicalOperator.HasValue)
                {
                    if (queryPart.LogicalOperator.Value == LogicalOperator.And)
                        sequence.Add(new LogicalOperatorAndSymbol(i));
                    else if (queryPart.LogicalOperator.Value == LogicalOperator.Or)
                        sequence.Add(new LogicalOperatorOrSymbol(i));
                }

                #endregion
            }

            return sequence;
        }
    }
}

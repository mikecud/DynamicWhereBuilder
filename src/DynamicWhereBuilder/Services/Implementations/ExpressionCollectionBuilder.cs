using DynamicWhereBuilder.Extensions;
using DynamicWhereBuilder.Models.Expression;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicWhereBuilder.Services.Implementations
{
    internal class ExpressionCollectionBuilder<T> : IExpressionCollectionBuilder<T>
    {
        public ExpressionCollection<T> Build(List<ISequenceSymbol> sequenceOfSymbols)
        {
            var expressionCollection = new ExpressionCollection<T>();

            for (var i = 0; i < sequenceOfSymbols.Count(); i++)
            {
                var sequenceSymbol = sequenceOfSymbols.ElementAt(i);
                var expressionBaseToAdd = (ExpressionBase<T>)null;

                switch (sequenceSymbol)
                {
                    case ParenthesisOpenSymbol _:
                        var nextParenthesisCloseSymbolIndex = sequenceOfSymbols.FindIndex(i, x => x is ParenthesisCloseSymbol);
                        var subSequenceOfSymbols = sequenceOfSymbols.Skip(i + 1).Take(nextParenthesisCloseSymbolIndex - i).ToList(); // extracting subsequence of symbols inside parentheses

                        expressionBaseToAdd = Build(subSequenceOfSymbols);
                        i = nextParenthesisCloseSymbolIndex; // going to closing parenthesis position
                        break;
                    case ExpressionSymbol<T> _:
                        expressionBaseToAdd = new ExpressionItem<T>((sequenceSymbol as ExpressionSymbol<T>).Expression);
                        break;
                    default: // should not happen
                        // ParenthesisCloseSymbol
                        // LogicalOperatorAndSymbol
                        // LogicalOperatorOrSymbol
                        throw new NotImplementedException(string.Format(GeneralResources.Exception_UnexpectedTokenDuringAnalysisOfSequenceOfSymbols, sequenceSymbol.GetType()));
                }

                var nextLogicalOperator = sequenceOfSymbols.ElementAtOrDefault(++i) as ILogicalOperatorSymbol; // going to next position (after closing parenthesis or expression symbol)
                if (nextLogicalOperator != null)
                    expressionBaseToAdd.LogicalOperator = nextLogicalOperator.ToLogicalOperator();

                expressionCollection.Add(expressionBaseToAdd);
            }

            return expressionCollection;
        }

    }
}

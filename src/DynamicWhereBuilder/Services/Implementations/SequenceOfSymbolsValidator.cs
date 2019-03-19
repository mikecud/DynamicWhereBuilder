using DynamicWhereBuilder.Exceptions;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicWhereBuilder.Services.Implementations
{
    internal class SequenceOfSymbolsValidator<T> : ISequenceOfSymbolsValidator<T>
    {
        public void Validate(List<ISequenceSymbol> sequenceOfSymbols)
        {
            var parenthesisCount = 0;

            for (var i = 0; i < sequenceOfSymbols.Count(); i++)
            {
                var sequenceSymbol = sequenceOfSymbols.ElementAt(i);
                var nextSymbol = sequenceOfSymbols.ElementAtOrDefault(i + 1);

                switch (sequenceSymbol)
                {
                    case ILogicalOperatorSymbol _:
                        this.ValidateSymbol(sequenceSymbol as ILogicalOperatorSymbol, nextSymbol, i);
                        break;
                    case ExpressionSymbol<T> _:
                        this.ValidateSymbol(sequenceSymbol as ExpressionSymbol<T>, nextSymbol);
                        break;
                    case IParenthesisSymbol _:
                        this.ValidateSymbol(sequenceSymbol as IParenthesisSymbol, nextSymbol, i, ref parenthesisCount);
                        break;
                    default:
                        throw new NotImplementedException(string.Format(GeneralResources.Exception_UnexpectedTokenDuringAnalysisOfQueryParts, sequenceOfSymbols.GetType()));
                }
            }

            #region Parenthesis mismatch

            if (parenthesisCount != 0)
                throw new ParseException(ValidationErrorResources.SequenceOfSymbols_ParenthesisMismatch);

            #endregion
        }

        private Type[] allowedSymbolsAfterLogicalOperator = new[] { typeof(ExpressionSymbol<T>), typeof(ParenthesisOpenSymbol) };

        public void ValidateSymbol(ILogicalOperatorSymbol sequenceSymbol, ISequenceSymbol nextSymbol, int i)
        {
            if (i == 0)
                throw new ParseException(
                    ValidationErrorResources.SequenceOfSymbols_QueryCannotBeginWithLogicalOperator,
                    (sequenceSymbol as ISequenceSymbol).QueryPartIndex);

            if (!this.allowedSymbolsAfterLogicalOperator.Contains(nextSymbol?.GetType()))
                throw new ParseException(
                    ValidationErrorResources.SequenceOfSymbols_AfterLogicalOperatorThereMustBeOpeningParenthesisOrExpression,
                    (sequenceSymbol as ISequenceSymbol).QueryPartIndex);
        }

        private Type[] allowedSymbolsAfterExpression = new[] { typeof(LogicalOperatorAndSymbol), typeof(LogicalOperatorOrSymbol), typeof(ParenthesisCloseSymbol), null };

        public void ValidateSymbol(ExpressionSymbol<T> sequenceSymbol, ISequenceSymbol nextSymbol)
        {
            if (!this.allowedSymbolsAfterExpression.Contains(nextSymbol?.GetType()))
                throw new ParseException(
                    ValidationErrorResources.SequenceOfSymbols_AfterExpressionThereMustBeLogicalOperatorOrClosingParenthesis,
                    sequenceSymbol.QueryPartIndex);
        }

        private Type[] allowedSymbolsAfterParenthesisOpen = new[] { typeof(ExpressionSymbol<T>), typeof(ParenthesisOpenSymbol) };
        private Type[] allowedSymbolsAfterParenthesisClose = new[] { typeof(LogicalOperatorAndSymbol), typeof(LogicalOperatorOrSymbol), typeof(ParenthesisCloseSymbol), null };

        public void ValidateSymbol(IParenthesisSymbol sequenceSymbol, ISequenceSymbol nextSymbol, int i, ref int parenthesisCount)
        {
            switch (sequenceSymbol)
            {
                case ParenthesisOpenSymbol _:
                    parenthesisCount++;

                    if (!this.allowedSymbolsAfterParenthesisOpen.Contains(nextSymbol?.GetType()))
                        throw new ParseException(
                            ValidationErrorResources.SequenceOfSymbols_AfterOpeningParenthesisThereMustBeExpressionOrOpeningParenthesis,
                            (sequenceSymbol as ISequenceSymbol).QueryPartIndex);
                    break;
                case ParenthesisCloseSymbol _:
                    if (i == 0)
                        throw new ParseException(
                            ValidationErrorResources.SequenceOfSymbols_QueryCannotBeginWithClosingParenthesis,
                            (sequenceSymbol as ISequenceSymbol).QueryPartIndex);

                    parenthesisCount--;

                    if (!this.allowedSymbolsAfterParenthesisClose.Contains(nextSymbol?.GetType()))
                        throw new ParseException(
                            ValidationErrorResources.SequenceOfSymbols_AfterClosingParenthesisThereMustBeLogicalOperatorOrClosingParenthesis,
                            (sequenceSymbol as ISequenceSymbol).QueryPartIndex);
                    break;
                default: // should not happen
                    throw new ParseException(
                        string.Format(ValidationErrorResources.SequenceOfSymbols_UnknownParenthesisSymbolX, sequenceSymbol.GetType()),
                        (sequenceSymbol as ISequenceSymbol).QueryPartIndex);
            }
        }
    }
}

using DynamicWhereBuilder.Models.Sequence;
using System.Collections.Generic;

namespace DynamicWhereBuilder.Services.Interfaces
{
    internal interface ISequenceOfSymbolsValidator<T>
    {
        void Validate(List<ISequenceSymbol> sequenceOfSymbols);

        void ValidateSymbol(ILogicalOperatorSymbol sequenceSymbol, ISequenceSymbol nextSymbol, int i);

        void ValidateSymbol(ExpressionSymbol<T> sequenceSymbol, ISequenceSymbol nextSymbol);

        void ValidateSymbol(IParenthesisSymbol sequenceSymbol, ISequenceSymbol nextSymbol, int i, ref int parenthesisCount);
    }
}

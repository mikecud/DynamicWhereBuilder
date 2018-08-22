using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DynamicWhereBuilder.Services.Implementations
{
    internal class WhereExecutor<T> : IWhereExecutor<T>
    {
        private readonly IQueryPartsValidator<T> _queryPartsValidator;
        private readonly ISequenceOfSymbolsBuilder<T> _sequenceOfSymbolsBuilder;
        private readonly ISequenceOfSymbolsValidator<T> _sequenceOfSymbolsValidator;
        private readonly IExpressionCollectionBuilder<T> _expressionCollectionBuilder;
        private readonly IExpressionBuilder<T> _expressionBuilder;

        internal WhereExecutor(
            IQueryPartsValidator<T> queryPartsValidator,
            ISequenceOfSymbolsBuilder<T> sequenceOfSymbolsBuilder,
            ISequenceOfSymbolsValidator<T> sequenceOfSymbolsValidator,
            IExpressionCollectionBuilder<T> expressionCollectionBuilder,
            IExpressionBuilder<T> expressionBuilder)
        {
            this._queryPartsValidator = queryPartsValidator;
            this._sequenceOfSymbolsBuilder = sequenceOfSymbolsBuilder;
            this._sequenceOfSymbolsValidator = sequenceOfSymbolsValidator;
            this._expressionCollectionBuilder = expressionCollectionBuilder;
            this._expressionBuilder = expressionBuilder;
        }
        
        public IQueryable<T> Execute(IQueryable<T> query, IEnumerable<QueryPart<T>> queryParts)
        {
            this._queryPartsValidator.Validate(queryParts);

            var sequenceOfSymbols = this._sequenceOfSymbolsBuilder.Build(queryParts);
            this._sequenceOfSymbolsValidator.Validate(sequenceOfSymbols);

            var expressionCollection = this._expressionCollectionBuilder.Build(sequenceOfSymbols);
            var expression = this._expressionBuilder.Build(expressionCollection);

            return query.Where(expression);
        }

        public IEnumerable<T> Execute(IEnumerable<T> collection, IEnumerable<QueryPart<T>> queryParts)
        {
            return this.Execute(collection.AsQueryable(), queryParts).AsEnumerable();
        }
    }
}

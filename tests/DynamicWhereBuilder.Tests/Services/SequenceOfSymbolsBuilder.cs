using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Services.Implementations;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace DynamicWhereBuilder.Tests.Services
{
    public class SequenceOfSymbolsBuilderTestsFixture
    {
        internal ISequenceOfSymbolsBuilder<object> _sequenceOfSymbolsBuilder { get; private set; }

        public SequenceOfSymbolsBuilderTestsFixture()
        {
            this._sequenceOfSymbolsBuilder = new SequenceOfSymbolsBuilder<object>();
        }
    }

    public class SequenceOfSymbolsBuilderTests : IClassFixture<SequenceOfSymbolsBuilderTestsFixture>
    {
        private readonly SequenceOfSymbolsBuilderTestsFixture _fixture;

        public SequenceOfSymbolsBuilderTests(SequenceOfSymbolsBuilderTestsFixture fixture)
        {
            this._fixture = fixture;
        }

        public static IEnumerable<object[]> ListsOfQueryPartsWithCorrespondingSequencesOfSymbols =>
            new List<object[]>()
            {
                new object[]
                {
                    new List<QueryPart<object>>() { new QueryPart<object>(Parenthesis.Open, null, null, null) }.AsEnumerable(),
                    new List<ISequenceSymbol>() { new ParenthesisOpenSymbol(0) }
                },
                new object[]
                {
                    new List<QueryPart<object>>() { new QueryPart<object>(Parenthesis.Close, null, null, null) }.AsEnumerable(),
                    new List<ISequenceSymbol>() { new ParenthesisCloseSymbol(0) }
                },
                new object[]
                {
                    new List<QueryPart<object>>() { new QueryPart<object>(null, x => true, null, null) },
                    new List<ISequenceSymbol>() { new ExpressionSymbol<object>(0, x => true) }
                },
                new object[]
                {
                    new List<QueryPart<object>> { new QueryPart<object>(null, null, Parenthesis.Open, null) },
                    new List<ISequenceSymbol>() { new ParenthesisOpenSymbol(0) }
                },
                new object[]
                {
                    new List<QueryPart<object>> { new QueryPart<object>(null, null, Parenthesis.Close, null) },
                    new List<ISequenceSymbol>() { new ParenthesisCloseSymbol(0) }
                },
                new object[]
                {
                    new List<QueryPart<object>> { new QueryPart<object>(null, null, null, LogicalOperator.And) },
                    new List<ISequenceSymbol>() { new LogicalOperatorAndSymbol(0) }
                },
                new object[]
                {
                    new List<QueryPart<object>> { new QueryPart<object>(null, null, null, LogicalOperator.Or) },
                    new List<ISequenceSymbol>() { new LogicalOperatorOrSymbol(0) }
                }
            };

        [Theory]
        [MemberData(nameof(ListsOfQueryPartsWithCorrespondingSequencesOfSymbols))]
        internal void SequenceOfSymbolsBuilder_SequenceWithCorrectTypesOfSymbols(IEnumerable<QueryPart<object>> queryParts, List<ISequenceSymbol> sequenceOfExpectedSymbols)
        {
            // Act
            var sequenceOfActualSymbols = this._fixture._sequenceOfSymbolsBuilder.Build(queryParts);

            // Assert
            var elementInspectors = sequenceOfExpectedSymbols
                .Select(expectedSymbol => (Action<ISequenceSymbol>)(actualSymbol => Assert.IsType(expectedSymbol.GetType(), actualSymbol)))
                .ToArray();

            Assert.Collection(sequenceOfActualSymbols, elementInspectors);
        }

        [Fact]
        public void SequenceOfSymbolsBuilder_QueryPartWithExpression_TheSameQueryInExpressionSymbol()
        {
            // Arrange
            var expectedExpression = (Expression<Func<object, bool>>)(x => true);

            var queryPartsList = new List<QueryPart<object>>();
            queryPartsList.Add(new QueryPart<object>(null, expectedExpression, null, null));

            var queryParts = queryPartsList.AsEnumerable();

            // Act
            var sequenceOfSymbols = this._fixture._sequenceOfSymbolsBuilder.Build(queryParts);

            // Assert
            Assert.Collection(sequenceOfSymbols, elem =>
            {
                Assert.IsType<ExpressionSymbol<object>>(elem);
                Assert.Equal(expectedExpression, (elem as ExpressionSymbol<object>).Expression);
            });
        }

        [Fact]
        public void SequenceOfSymbolsBuilder_SequenceSymbolsHaveCorrectQueryPartIndex()
        {
            // Arrange
            var queryPartsList = new List<QueryPart<object>>();

            queryPartsList.Add(new QueryPart<object>(Parenthesis.Open, null, null, null));
            queryPartsList.Add(new QueryPart<object>(null, x => true, null, null));
            queryPartsList.Add(new QueryPart<object>(null, null, Parenthesis.Close, null));
            queryPartsList.Add(new QueryPart<object>(null, null, null, LogicalOperator.And));

            var queryParts = queryPartsList.AsEnumerable();

            // Act
            var sequenceOfSymbols = this._fixture._sequenceOfSymbolsBuilder.Build(queryParts);

            // Assert
            Assert.Collection(sequenceOfSymbols,
                elem => Assert.Equal(0, elem.QueryPartIndex),
                elem => Assert.Equal(1, elem.QueryPartIndex),
                elem => Assert.Equal(2, elem.QueryPartIndex),
                elem => Assert.Equal(3, elem.QueryPartIndex));
        }
    }
}

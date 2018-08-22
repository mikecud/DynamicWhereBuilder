using DynamicWhereBuilder.Models.Expression;
using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Implementations;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DynamicWhereBuilder.Tests.Services
{
    public class ExpressionCollectionBuilderTestsFixture
    {
        internal IExpressionCollectionBuilder<object> _expressionCollectionBuilder { get; private set; }

        public ExpressionCollectionBuilderTestsFixture()
        {
            this._expressionCollectionBuilder = new ExpressionCollectionBuilder<object>();
        }
    }

    public class ExpressionCollectionBuilderTests : IClassFixture<ExpressionCollectionBuilderTestsFixture>
    {
        private readonly ExpressionCollectionBuilderTestsFixture _fixture;

        public ExpressionCollectionBuilderTests(ExpressionCollectionBuilderTestsFixture fixture)
        {
            this._fixture = fixture;
        }

        public static IEnumerable<object[]> InvalidSymbolsForExpressionCollection =>
            new List<object[]>
            {
                new object[] { new ParenthesisCloseSymbol(0) },
                new object[] { new LogicalOperatorAndSymbol(0) },
                new object[] { new LogicalOperatorOrSymbol(0) }
            };

        [Theory]
        [MemberData(nameof(InvalidSymbolsForExpressionCollection))]
        internal void ExpressionCollectionBuilder_HitsInvalidSymbol_ThrowsException(ISequenceSymbol sequenceSymbol)
        {
            // Arrange
            var sequenceOfSymbols = new List<ISequenceSymbol>();
            sequenceOfSymbols.Add(sequenceSymbol);

            // Act & Assert
            var ex = Assert.Throws<NotImplementedException>(() => this._fixture._expressionCollectionBuilder.Build(sequenceOfSymbols));
            Assert.Equal(string.Format(GeneralResources.Exception_UnexpectedTokenDuringAnalysisOfSequenceOfSymbols, sequenceSymbol.GetType()), ex.Message);
        }

        public static IEnumerable<object[]> NumbersOfExpressionSymbolsWithLogicalOperator => new List<object[]>
        {
            new object[] { 1, null },
            new object[] { 2, new LogicalOperatorAndSymbol(0) },
            new object[] { 2, new LogicalOperatorOrSymbol(0) },
            new object[] { 4, new LogicalOperatorAndSymbol(0) },
            new object[] { 4, new LogicalOperatorOrSymbol(0) },
        };

        [Theory]
        [MemberData(nameof(NumbersOfExpressionSymbolsWithLogicalOperator))]
        internal void ExpressionCollectionBuilder_ExpressionSymbolsConcatenatedWithLogicalOperators_AllButLastExpressionItemsHaveLogicalOperator(
            int numberOfExpressionSymbols, ILogicalOperatorSymbol logicalOperator)
        {
            // Arrange
            var sequenceOfSymbols = new List<ISequenceSymbol>();

            for (var i = 0; i < numberOfExpressionSymbols; i++)
            {
                sequenceOfSymbols.Add(new ExpressionSymbol<object>(i, x => true));

                if (i < numberOfExpressionSymbols - 1)
                    sequenceOfSymbols.Add(logicalOperator as ISequenceSymbol);
            }

            // Act
            var expressionCollection = this._fixture._expressionCollectionBuilder.Build(sequenceOfSymbols);

            // Assert
            var elementInspectors = new List<Action<ExpressionBase<object>>>();

            // Element inspectors for all but last ExpressionItem
            for (var i = 0; i < numberOfExpressionSymbols - 1; i++)
            {
                elementInspectors.Add(elem => Assert.NotNull(elem.LogicalOperator));
            }

            // Element inspector for last ExpressionItem
            elementInspectors.Add(elem => Assert.Null(elem.LogicalOperator));

            Assert.Collection(expressionCollection, elementInspectors.ToArray());
        }

        [Fact]
        public void ExpressionCollectionBuilder_ContainsParentheses_SubcollectionCreated()
        {
            // Arrange
            var sequenceOfSymbols = new List<ISequenceSymbol>();

            sequenceOfSymbols.Add(new ParenthesisOpenSymbol(0));
            sequenceOfSymbols.Add(new ExpressionSymbol<object>(0, x => true));
            sequenceOfSymbols.Add(new ParenthesisCloseSymbol(0));

            // Act
            var expressionCollection = this._fixture._expressionCollectionBuilder.Build(sequenceOfSymbols);

            // Assert
            Assert.Collection(expressionCollection,
                elem1 => Assert.IsAssignableFrom<ExpressionCollection<object>>(elem1));
        }

        [Fact]
        public void ExpressionCollectionBuilder_ContainsParentheses_SubcollectionContainsProperExpressionItems()
        {
            // Arrange
            var sequenceOfSymbols = new List<ISequenceSymbol>();

            sequenceOfSymbols.Add(new ParenthesisOpenSymbol(0));
            sequenceOfSymbols.Add(new ExpressionSymbol<object>(0, x => true));
            sequenceOfSymbols.Add(new LogicalOperatorAndSymbol(0));
            sequenceOfSymbols.Add(new ExpressionSymbol<object>(1, x => true));
            sequenceOfSymbols.Add(new ParenthesisCloseSymbol(0));

            // Act
            var expressionCollection = this._fixture._expressionCollectionBuilder.Build(sequenceOfSymbols);
            var subCollection = expressionCollection.FirstOrDefault();

            // Assert
            Assert.NotNull(subCollection);
            Assert.IsAssignableFrom<ExpressionCollection<object>>(subCollection);

            Assert.Collection(subCollection as ExpressionCollection<object>,
                elem1 =>
                {
                    Assert.IsAssignableFrom<ExpressionItem<object>>(elem1);
                    Assert.Equal(LogicalOperator.And, (elem1 as ExpressionItem<object>).LogicalOperator);
                },
                elem2 =>
                {
                    Assert.IsAssignableFrom<ExpressionItem<object>>(elem2);
                    Assert.Null((elem2 as ExpressionItem<object>).LogicalOperator);
                });
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ExpressionCollectionBuilder_MultipleParenthesisPairs_MultipleSubCollectionsCreated(int numberOfParenthesisPairs)
        {
            // Arrange
            var sequencOfSymbols = new List<ISequenceSymbol>();

            for (var i = 0; i < numberOfParenthesisPairs; i++)
            {
                var subSequenceOfSymbols = new List<ISequenceSymbol>();

                subSequenceOfSymbols.Add(new ParenthesisOpenSymbol(i));
                subSequenceOfSymbols.Add(new ExpressionSymbol<object>(i, x => true));
                subSequenceOfSymbols.Add(new ParenthesisCloseSymbol(i));

                if (i < numberOfParenthesisPairs - 1)
                    subSequenceOfSymbols.Add(new LogicalOperatorAndSymbol(i));

                sequencOfSymbols.AddRange(subSequenceOfSymbols);
            }

            // Act
            var expressionCollection = this._fixture._expressionCollectionBuilder.Build(sequencOfSymbols);

            // Assert
            var elementInspectors = new List<Action<ExpressionBase<object>>>();

            // Element inspectors for all but last ExpressionItem
            for (var i = 0; i < numberOfParenthesisPairs - 1; i++)
            {
                elementInspectors.Add(elem =>
                {
                    Assert.IsAssignableFrom<ExpressionCollection<object>>(elem);
                    Assert.NotNull(elem.LogicalOperator);
                });
            }

            // Element inspector for last ExpressionItem
            elementInspectors.Add(elem =>
            {
                Assert.IsAssignableFrom<ExpressionCollection<object>>(elem);
                Assert.Null(elem.LogicalOperator);
            });

            Assert.Collection(expressionCollection, elementInspectors.ToArray());
        }

        [Fact]
        public void ExpressionCollectionBuilder_ParethesesPairInParenthesesPair_SubcollectionInSubcollection()
        {
            // Arrange
            var sequenceOfSymbols = new List<ISequenceSymbol>();

            sequenceOfSymbols.Add(new ParenthesisOpenSymbol(0));
            sequenceOfSymbols.Add(new ParenthesisOpenSymbol(0));
            sequenceOfSymbols.Add(new ExpressionSymbol<object>(1, x => true));
            sequenceOfSymbols.Add(new ParenthesisCloseSymbol(2));
            sequenceOfSymbols.Add(new ParenthesisCloseSymbol(2));

            // Act
            var expressionCollection = this._fixture._expressionCollectionBuilder.Build(sequenceOfSymbols);

            // Assert
            Assert.IsAssignableFrom<ExpressionCollection<object>>(expressionCollection[0]);
            Assert.IsAssignableFrom<ExpressionCollection<object>>((expressionCollection[0] as ExpressionCollection<object>)[0]);
        }
    }
}

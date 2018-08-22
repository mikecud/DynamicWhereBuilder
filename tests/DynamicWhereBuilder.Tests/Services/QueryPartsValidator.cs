using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Implementations;
using DynamicWhereBuilder.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DynamicWhereBuilder.Tests.Services
{
    public class QueryPartsValidatorTestsFixture
    {
        internal IQueryPartsValidator<object> _queryPartsValidator { get; private set; }

        public QueryPartsValidatorTestsFixture()
        {
            this._queryPartsValidator = new QueryPartsValidator<object>();
        }
    }

    public class QueryPartsValidatorTests : IClassFixture<QueryPartsValidatorTestsFixture>
    {
        private readonly QueryPartsValidatorTestsFixture _fixture;

        public QueryPartsValidatorTests(QueryPartsValidatorTestsFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void QueryPartsValidator_Validate_QueryPartsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>("queryParts", () => this._fixture._queryPartsValidator.Validate(null));
        }

        [Fact]
        public void QueryPartsValidator_Validate_QueryPartsEmpty_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var queryParts = new List<QueryPart<object>>().AsEnumerable();

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>("queryParts", () => this._fixture._queryPartsValidator.Validate(queryParts));
            Assert.StartsWith(ValidationErrorResources.QueryPart_YouShouldProvideAtLeastOneQueryPart, ex.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void QueryPartsValidator_Validate_QueryPartsAllWithNoSymbol_ThrowsArgumentException(int numberOfQueryParts)
        {
            // Arrange
            var queryPartsList = new List<QueryPart<object>>();

            for (var i = 0; i < numberOfQueryParts; i++)
            {
                queryPartsList.Add(new QueryPart<object>(null, null, null, null));
            }

            var queryParts = queryPartsList.AsEnumerable();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>("queryParts", () => this._fixture._queryPartsValidator.Validate(queryParts));
            Assert.StartsWith(ValidationErrorResources.QueryPart_YouShouldProvideAtLeastOneQueryPartWithASymbol, ex.Message);
        }

        [Theory]
        [InlineData(true, false, false, false)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, true, false, false)]
        [InlineData(false, true, true, false)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, true, true)]
        [InlineData(false, false, false, true)]
        [InlineData(true, true, true, true)]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, true)]
        [InlineData(true, true, true, false)]
        public void QueryPartsValidator_Validate_QueryPartsNotNullNotEmptyAndWithSymbols_DoesNotThrowException(
            bool initialParenthesisNotNull,
            bool expressionNotNull,
            bool endingParenthesisNotNull,
            bool logicalOperatorNotNull)
        {
            // Arrange
            var queryPartsList = new List<QueryPart<object>>();

            if (initialParenthesisNotNull)
            {
                queryPartsList.Add(new QueryPart<object>(Parenthesis.Open, null, null, null));
            }

            if (expressionNotNull)
            {
                queryPartsList.Add(new QueryPart<object>(null, x => true, null, null));
            }

            if (endingParenthesisNotNull)
            {
                queryPartsList.Add(new QueryPart<object>(null, null, Parenthesis.Close, null));
            }

            if (logicalOperatorNotNull)
            {
                queryPartsList.Add(new QueryPart<object>(null, null, null, LogicalOperator.And));
            }

            var queryParts = queryPartsList.AsEnumerable();

            // Act
            this._fixture._queryPartsValidator.Validate(queryParts);
        }
    }
}

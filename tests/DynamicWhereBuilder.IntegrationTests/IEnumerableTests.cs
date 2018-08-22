using DynamicWhereBuilder;
using DynamicWhereBuilder.Models.QueryPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace DynamicWhereBuilder.IntegrationTests
{
    public class IEnumerableTestClass
    {
        public IEnumerableTestClass(int id, string value)
        {
            this.Id = id;
            this.Value = value;
        }

        public int Id { get; private set; }

        public string Value { get; private set; }

        public override string ToString()
        {
            return $"(Id = {this.Id}, Value = \"{this.Value}\")";
        }
    }

    public class IEnumerableTestsFixture
    {
        public IEnumerable<IEnumerableTestClass> Enumerable { get; private set; }

        public IEnumerableTestsFixture()
        {
            this.Enumerable = new[]
            {
                new IEnumerableTestClass(1, "valid"),
                new IEnumerableTestClass(1, "invalid"),
                new IEnumerableTestClass(2, "valid"),
                new IEnumerableTestClass(2, "invalid")
            };
        }
    }

    public class IEnumerableTests : IClassFixture<IEnumerableTestsFixture>
    {
        private readonly IEnumerableTestsFixture _fixture;

        public IEnumerableTests(IEnumerableTestsFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void IEnumerable_SingleQueryPart_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Id == 1, null, null)); // x.Id == 1

            Func<IEnumerableTestClass, bool> lambda = x => x.Id == 1;

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_SingleQueryPartParentheses_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x => 
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 1, Parenthesis.Close, null)); // (x.Id == 1)

            Func<IEnumerableTestClass, bool> lambda = x => (x.Id == 1);

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoQueryParts_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Id == 1, null, LogicalOperator.And)); // x.Id == 1 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "valid", null, null)); // x.Value == "valid"

            Func<IEnumerableTestClass, bool> lambda = x => x.Id == 1 && x.Value == "valid";

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoQueryPartsParenthesesAroundAll_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.And)); // (x.Id == 1 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, null)); // x.Value == "valid")

            Func<IEnumerableTestClass, bool> lambda = x => (x.Id == 1 && x.Value == "valid");

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoQueryPartsParenthesesAroundParticularQueries_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 1, Parenthesis.Close, LogicalOperator.And)); // (x.Id == 1) &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Value == "valid", Parenthesis.Close, null)); // (x.Value == "valid")

            Func<IEnumerableTestClass, bool> lambda = x => (x.Id == 1) && (x.Value == "valid");

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoOrSubqueriesCombinedByAnd_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.Or)); // (x.Id == 1 ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.And)); // x.Value == "valid) &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.Or)); // (x.Id == 2 ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IEnumerableTestClass, bool> lambda = x => (x.Id == 1 || x.Value == "valid") && (x.Id == 2 || x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoOrSubqueriesCombinedByOr_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.Or)); // (x.Id == 1 ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.Or)); // x.Value == "valid) ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.Or)); // (x.Id == 2 ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IEnumerableTestClass, bool> lambda = x => (x.Id == 1 || x.Value == "valid") || (x.Id == 2 || x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoAndSubqueriesCombinedByAnd_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.And)); // (x.Id == 1 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.And)); // x.Value == "valid) &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IEnumerableTestClass, bool> lambda = x => (x.Id == 1 && x.Value == "valid") && (x.Id == 2 && x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoAndSubqueriesCombinedByOr_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.And)); // (x.Id == 1 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.Or)); // x.Value == "valid") ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IEnumerableTestClass, bool> lambda = x => (x.Id == 1 && x.Value == "valid") || (x.Id == 2 && x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_TwoQueriesWithDifferentParameterNames_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); 
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Id == 1, null, LogicalOperator.And)); // (x =>) x.Id == 1 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, y => y.Value == "invalid", null, null)); // // (y =>) y.Value == "invalid"

            Func<IEnumerableTestClass, bool> lambda = x => x.Id == 1 && x.Value == "invalid";

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IEnumerable_QueryInSubQuery_CorrectResultCollection()
        {
            var queryParts = new List<QueryPart<IEnumerableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Id == 1, null, LogicalOperator.Or)); // x.Id == 1 ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, null, null, null)); // (
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.Or)); // x.Value == "valid") ||
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IEnumerableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")
            queryParts.Add(new QueryPart<IEnumerableTestClass>(Parenthesis.Close, null, null, null)); // )

            Func<IEnumerableTestClass, bool> lambda = x => x.Id == 1 || ((x.Id == 2 && x.Value == "valid") || (x.Id == 2 && x.Value == "invalid"));

            // Act
            var queryPartsCollection = this._fixture.Enumerable.Where(queryParts);
            var lambdaCollection = this._fixture.Enumerable.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }
    }
}

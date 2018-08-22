using DynamicWhereBuilder.IntegrationTests.Db;
using DynamicWhereBuilder.Models.QueryPart;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DynamicWhereBuilder.IntegrationTests
{
    public class IQueryableTestsFixture : IDisposable
    {
        public TestContext TestContext { get; private set; }

        public IQueryableTestsFixture()
        {
            this.TestContext = new TestContext();
        }

        public void Dispose()
        {
            this.TestContext.Dispose();
        }
    }

    public class IQueryableTests : IClassFixture<IQueryableTestsFixture>
    {
        private readonly IQueryableTestsFixture _fixture;

        public IQueryableTests(IQueryableTestsFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void IQueryable_SingleQueryPart_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Id == 1, null, null)); // x.Id == 1

            Func<IQueryableTestClass, bool> lambda = x => x.Id == 1;

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_SingleQueryPartParentheses_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x => 
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 1, Parenthesis.Close, null)); // (x.Id == 1)

            Func<IQueryableTestClass, bool> lambda = x => (x.Id == 1);

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoQueryParts_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Id == 1, null, LogicalOperator.And)); // x.Id == 1 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "valid", null, null)); // x.Value == "valid"

            Func<IQueryableTestClass, bool> lambda = x => x.Id == 1 && x.Value == "valid";

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoQueryPartsParenthesesAroundAll_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.And)); // (x.Id == 1 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, null)); // x.Value == "valid")

            Func<IQueryableTestClass, bool> lambda = x => (x.Id == 1 && x.Value == "valid");

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoQueryPartsParenthesesAroundParticularQueries_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 1, Parenthesis.Close, LogicalOperator.And)); // (x.Id == 1) &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Value == "valid", Parenthesis.Close, null)); // (x.Value == "valid")

            Func<IQueryableTestClass, bool> lambda = x => (x.Id == 1) && (x.Value == "valid");

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoOrSubqueriesCombinedByAnd_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.Or)); // (x.Id == 1 ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.And)); // x.Value == "valid) &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.Or)); // (x.Id == 2 ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IQueryableTestClass, bool> lambda = x => (x.Id == 1 || x.Value == "valid") && (x.Id == 2 || x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoOrSubqueriesCombinedByOr_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.Or)); // (x.Id == 1 ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.Or)); // x.Value == "valid) ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.Or)); // (x.Id == 2 ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IQueryableTestClass, bool> lambda = x => (x.Id == 1 || x.Value == "valid") || (x.Id == 2 || x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoAndSubqueriesCombinedByAnd_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.And)); // (x.Id == 1 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.And)); // x.Value == "valid) &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IQueryableTestClass, bool> lambda = x => (x.Id == 1 && x.Value == "valid") && (x.Id == 2 && x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoAndSubqueriesCombinedByOr_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 1, null, LogicalOperator.And)); // (x.Id == 1 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.Or)); // x.Value == "valid") ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")

            Func<IQueryableTestClass, bool> lambda = x => (x.Id == 1 && x.Value == "valid") || (x.Id == 2 && x.Value == "invalid");

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_TwoQueriesWithDifferentParameterNames_CorrectResultCollection()
        {
            // Arrange
            var queryParts = new List<QueryPart<IQueryableTestClass>>();
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Id == 1, null, LogicalOperator.And)); // (x =>) x.Id == 1 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, y => y.Value == "invalid", null, null)); // // (y =>) y.Value == "invalid"

            Func<IQueryableTestClass, bool> lambda = x => x.Id == 1 && x.Value == "invalid";

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }

        [Fact]
        public void IQueryable_QueryInSubQuery_CorrectResultCollection()
        {
            var queryParts = new List<QueryPart<IQueryableTestClass>>(); // x =>
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Id == 1, null, LogicalOperator.Or)); // x.Id == 1 ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, null, null, null)); // (
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "valid", Parenthesis.Close, LogicalOperator.Or)); // x.Value == "valid") ||
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
            queryParts.Add(new QueryPart<IQueryableTestClass>(null, x => x.Value == "invalid", Parenthesis.Close, null)); // x.Value == "invalid")
            queryParts.Add(new QueryPart<IQueryableTestClass>(Parenthesis.Close, null, null, null)); // )

            Func<IQueryableTestClass, bool> lambda = x => x.Id == 1 || ((x.Id == 2 && x.Value == "valid") || (x.Id == 2 && x.Value == "invalid"));

            // Act
            var queryPartsCollection = this._fixture.TestContext.TestClasses.Where(queryParts);
            var lambdaCollection = this._fixture.TestContext.TestClasses.Where(lambda);

            // Assert
            Assert.Equal(lambdaCollection, queryPartsCollection);
        }
    }
}

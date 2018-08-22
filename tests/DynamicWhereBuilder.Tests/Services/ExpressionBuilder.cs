using DynamicWhereBuilder.Models.Expression;
using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Services.Implementations;
using DynamicWhereBuilder.Services.Interfaces;
using Xunit;

namespace DynamicWhereBuilder.Tests.Services
{
    public class ExpressionBuilderTestClass
    {
        public ExpressionBuilderTestClass(int id, string value)
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

    public class ExpressionBuilderTestsFixture
    {
        internal readonly IExpressionBuilder<ExpressionBuilderTestClass> ExpressionBuilder;

        public ExpressionBuilderTestsFixture()
        {
            this.ExpressionBuilder = new ExpressionBuilder<ExpressionBuilderTestClass>();
        }
    }

    public class ExpressionBuilderTests : IClassFixture<ExpressionBuilderTestsFixture>
    {
        private readonly ExpressionBuilderTestsFixture _fixture;

        public ExpressionBuilderTests(ExpressionBuilderTestsFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void ExpressionBuilder_TwoExpressionsCombinedWithOr_CorrectLambda()
        {
            // Arrange
            var expressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>();
            expressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(x => x.Id == 1, LogicalOperator.Or));
            expressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(y => y.Value == "valid"));

            // Act
            var lambda = this._fixture.ExpressionBuilder.Build(expressionCollection).ToString();

            // Assert
            Assert.Equal("x => ((x.Id == 1) Or (x.Value == \"valid\"))", lambda);
        }

        [Fact]
        public void ExpressionBuilder_TwoExpressionsCombinedWithAnd_CorrectLambda()
        {
            // Arrange
            var expressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>();
            expressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(x => x.Id == 1, LogicalOperator.And));
            expressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(y => y.Value == "valid"));

            // Act
            var lambda = this._fixture.ExpressionBuilder.Build(expressionCollection).ToString();

            // Assert
            Assert.Equal("x => ((x.Id == 1) And (x.Value == \"valid\"))", lambda);
        }

        [Fact]
        public void ExpressionBuilder_TwoSubExpressionsCombinedWithOr_CorrectLambda()
        {
            // Arrange
            var firstExpressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>(LogicalOperator.Or);
            firstExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(x => x.Id == 1, LogicalOperator.And));
            firstExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(y => y.Value == "valid"));

            var secondExpressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>();
            secondExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(x => x.Id == 2, LogicalOperator.And));
            secondExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(y => y.Value == "invalid"));

            var expressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>();
            expressionCollection.Add(firstExpressionCollection);
            expressionCollection.Add(secondExpressionCollection);

            // Act
            var lambda = this._fixture.ExpressionBuilder.Build(expressionCollection).ToString();

            // Assert
            Assert.Equal("x => (((x.Id == 1) And (x.Value == \"valid\")) Or ((x.Id == 2) And (x.Value == \"invalid\")))", lambda);
        }

        [Fact]
        public void ExpressionBuilder_TwoSubExpressionsCombinedWithAnd_CorrectLambda()
        {
            // Arrange
            var firstExpressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>(LogicalOperator.And);
            firstExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(x => x.Id == 1, LogicalOperator.Or));
            firstExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(y => y.Value == "valid"));

            var secondExpressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>();
            secondExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(x => x.Id == 2, LogicalOperator.Or));
            secondExpressionCollection.Add(new ExpressionItem<ExpressionBuilderTestClass>(y => y.Value == "invalid"));

            var expressionCollection = new ExpressionCollection<ExpressionBuilderTestClass>();
            expressionCollection.Add(firstExpressionCollection);
            expressionCollection.Add(secondExpressionCollection);

            // Act
            var lambda = this._fixture.ExpressionBuilder.Build(expressionCollection).ToString();

            // Assert
            Assert.Equal("x => (((x.Id == 1) Or (x.Value == \"valid\")) And ((x.Id == 2) Or (x.Value == \"invalid\")))", lambda);
        }
    }
}

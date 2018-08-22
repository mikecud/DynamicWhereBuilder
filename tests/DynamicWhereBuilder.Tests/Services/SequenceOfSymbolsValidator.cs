using DynamicWhereBuilder.Exceptions;
using DynamicWhereBuilder.Models.Sequence;
using DynamicWhereBuilder.Resources;
using DynamicWhereBuilder.Services.Implementations;
using DynamicWhereBuilder.Services.Interfaces;
using System.Collections.Generic;
using Xunit;

namespace DynamicWhereBuilder.Tests.Services
{
    public class SequenceOfSymbolsValidatorTestsFixture
    {
        internal readonly ISequenceOfSymbolsValidator<object> SequenceOfSymbolsValidator;

        public SequenceOfSymbolsValidatorTestsFixture()
        {
            this.SequenceOfSymbolsValidator = new SequenceOfSymbolsValidator<object>();
        }
    }

    public class SequenceOfSymbolsValidatorTests : IClassFixture<SequenceOfSymbolsValidatorTestsFixture>
    {
        private readonly SequenceOfSymbolsValidatorTestsFixture _fixture;

        public SequenceOfSymbolsValidatorTests(SequenceOfSymbolsValidatorTestsFixture fixture)
        {
            this._fixture = fixture;
        }

        #region Beggining symbols

        public static IEnumerable<object[]> InvalidBeginningSymbols =>
            new List<object[]>()
            {
                new object[] { new LogicalOperatorAndSymbol(0), ValidationErrorResources.SequenceOfSymbols_QueryCannotBeginWithLogicalOperator },
                new object[] { new LogicalOperatorOrSymbol(0), ValidationErrorResources.SequenceOfSymbols_QueryCannotBeginWithLogicalOperator },
                new object[] { new ParenthesisCloseSymbol(0), ValidationErrorResources.SequenceOfSymbols_QueryCannotBeginWithClosingParenthesis },
            };

        [Theory]
        [MemberData(nameof(InvalidBeginningSymbols))]
        internal void SequenceOfSymbolsBuilder_QueryBeginsWithInvalidSymbols_ThrowsException(ISequenceSymbol sequenceSymbol, string expectedExceptionMessage)
        {
            // Arrange
            var sequenceOfSymbols = new List<ISequenceSymbol>();
            sequenceOfSymbols.Add(sequenceSymbol);

            // Act & Assert
            var ex = Assert.Throws<ParseException>(() => this._fixture.SequenceOfSymbolsValidator.Validate(sequenceOfSymbols));
            Assert.StartsWith(expectedExceptionMessage, ex.Message);
        }

        public static IEnumerable<object[]> ValidBegginingSymbols =>
            new List<object[]>()
            {
                new ISequenceSymbol[] { new ParenthesisOpenSymbol(0), new ExpressionSymbol<object>(0, null), new ParenthesisCloseSymbol(0) },
                new[] { new ExpressionSymbol<object>(0, null), null, null },
            };

        [Theory]
        [MemberData(nameof(ValidBegginingSymbols))]
        internal void SequenceOfSymbolsBuilder_QueryBeginsWithValidSymbols_DoesNotThrowException(
            ISequenceSymbol sequenceSymbol,
            ISequenceSymbol nextSequenceSymbol1,
            ISequenceSymbol nextSequenceSymbol2)
        {
            // Arrange
            var sequenceOfSymbols = new List<ISequenceSymbol>();
            sequenceOfSymbols.Add(sequenceSymbol);

            if (nextSequenceSymbol1 != null)
            {
                sequenceOfSymbols.Add(nextSequenceSymbol1);
            }

            if (nextSequenceSymbol2 != null)
            {
                sequenceOfSymbols.Add(nextSequenceSymbol2);
            }

            // Act
            this._fixture.SequenceOfSymbolsValidator.Validate(sequenceOfSymbols);
        }

        #endregion

        #region ILogicalOperator

        public static IEnumerable<object[]> ILogicalOperatorInvalidProceeding =>
            new List<object[]>()
            {
                new object[] { new LogicalOperatorAndSymbol(0), new LogicalOperatorAndSymbol(0) },
                new object[] { new LogicalOperatorOrSymbol(0), new LogicalOperatorAndSymbol(0) },
                new object[] { new LogicalOperatorAndSymbol(0), new LogicalOperatorOrSymbol(0) },
                new object[] { new LogicalOperatorOrSymbol(0), new LogicalOperatorOrSymbol(0) },
                new object[] { new LogicalOperatorAndSymbol(0), new ParenthesisCloseSymbol(0) },
                new object[] { new LogicalOperatorOrSymbol(0), new ParenthesisCloseSymbol(0) },
                new object[] { new LogicalOperatorAndSymbol(0), null },
                new object[] { new LogicalOperatorOrSymbol(0), null }
            };

        [Theory]
        [MemberData(nameof(ILogicalOperatorInvalidProceeding))]
        internal void WhereBuilder_ValidateSymbolILogicalOperator_InvalidProceeding_ThrowsException(
            ILogicalOperatorSymbol symbol,
            ISequenceSymbol nextSymbol)
        {
            // Act && Assert
            var ex = Assert.Throws<ParseException>(() => this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(symbol, nextSymbol, 1));
            Assert.StartsWith(ValidationErrorResources.SequenceOfSymbols_AfterLogicalOperatorThereMustBeOpeningParenthesisOrExpression, ex.Message);
        }

        public static IEnumerable<object[]> ILogicalOperatorValidProceeding =>
            new List<object[]>()
            {
                new object[] { new LogicalOperatorAndSymbol(0), new ParenthesisOpenSymbol(0) },
                new object[] { new LogicalOperatorAndSymbol(0), new ExpressionSymbol<object>(0, null) }
            };

        [Theory]
        [MemberData(nameof(ILogicalOperatorValidProceeding))]
        internal void WhereBuilder_ValidateSymbolILogicalOperator_ValidProceeding_DoesnNotThrowException(ILogicalOperatorSymbol symbol, ISequenceSymbol nextSymbol)
        {
            // Act
            this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(symbol, nextSymbol, 1);
        }

        #endregion

        #region Expression

        public static IEnumerable<object[]> ExpressionInvalidProceeding =>
            new List<object[]>()
            {
                new object[] { new ParenthesisOpenSymbol(0) },
            };

        [Theory]
        [MemberData(nameof(ExpressionInvalidProceeding))]
        internal void WhereBuilder_ValidateSymbolExpression_InvalidProceeding_ThrowsException(ISequenceSymbol nextSymbol)
        {
            // Act && Assert
            var ex = Assert.Throws<ParseException>(() => this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(new ExpressionSymbol<object>(0, null), nextSymbol));
            Assert.StartsWith(ValidationErrorResources.SequenceOfSymbols_AfterExpressionThereMustBeLogicalOperatorOrClosingParenthesis, ex.Message);
        }

        public static IEnumerable<object[]> ExpressionValidProceeding =>
            new List<object[]>()
            {
                new object[] { new LogicalOperatorAndSymbol(0) },
                new object[] { new LogicalOperatorOrSymbol(0) },
                new object[] { new ParenthesisCloseSymbol(0) },
                new object[] { null },
            };

        [Theory]
        [MemberData(nameof(ExpressionValidProceeding))]
        internal void WhereBuilder_ValidateSymbolExpression_ValidProceeding_DoesnNotThrowException(ISequenceSymbol nextSymbol)
        {
            // Act
            this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(new ExpressionSymbol<object>(0, null), nextSymbol);
        }

        #endregion

        #region ParenthesisOpen

        public static IEnumerable<object[]> ParenthesisOpenInvalidProceeding =>
            new List<object[]>()
            {
                new object[] { new ParenthesisCloseSymbol(0) },
                new object[] { new LogicalOperatorAndSymbol(0) },
                new object[] { new LogicalOperatorOrSymbol(0) },
                new object[] { null }
            };

        [Theory]
        [MemberData(nameof(ParenthesisOpenInvalidProceeding))]
        internal void WhereBuilder_ValidateSymbolExpression_ParenthesisOpen_ThrowsException(ISequenceSymbol nextSymbol)
        {
            // Arrange
            var parenthesisCount = 0;

            // Act && Assert
            var ex = Assert.Throws<ParseException>(() => this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(new ParenthesisOpenSymbol(0), nextSymbol, 0, ref parenthesisCount));
            Assert.StartsWith(ValidationErrorResources.SequenceOfSymbols_AfterOpeningParenthesisThereMustBeExpressionOrOpeningParenthesis, ex.Message);
        }

        public static IEnumerable<object[]> ParenthesisOpenValidProceeding =>
            new List<object[]>()
            {
                new object[] { new ExpressionSymbol<object>(0, null) },
                new object[] { new ParenthesisOpenSymbol(0) }
            };

        [Theory]
        [MemberData(nameof(ParenthesisOpenValidProceeding))]
        internal void WhereBuilder_ValidateSymbolParenthesisOpen_ValidProceeding_DoesnNotThrowException(ISequenceSymbol nextSymbol)
        {
            // Arrange
            var parenthesisCount = 0;

            // Act
            this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(new ParenthesisOpenSymbol(0), nextSymbol, 0, ref parenthesisCount);
        }

        #endregion

        #region ParenthesisClose

        public static IEnumerable<object[]> ParenthesisCloseInvalidProceeding =>
            new List<object[]>()
            {
                new object[] { new ParenthesisOpenSymbol(0) },
                new object[] { new ExpressionSymbol<object>(0, null) },
            };

        [Theory]
        [MemberData(nameof(ParenthesisCloseInvalidProceeding))]
        internal void WhereBuilder_ValidateSymbolExpression_ParenthesisClose_ThrowsException(ISequenceSymbol nextSymbol)
        {
            // Arrange
            var parenthesisCount = 0;

            // Act && Assert
            var ex = Assert.Throws<ParseException>(() => this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(new ParenthesisCloseSymbol(0), nextSymbol, 1, ref parenthesisCount));
            Assert.StartsWith(ValidationErrorResources.SequenceOfSymbols_AfterClosingParenthesisThereMustBeLogicalOperatorOrClosingParenthesis, ex.Message);
        }

        public static IEnumerable<object[]> ParenthesisCloseValidProceeding =>
            new List<object[]>()
            {
                new object[] { new LogicalOperatorAndSymbol(0) },
                new object[] { new LogicalOperatorOrSymbol(0) },
                new object[] { new ParenthesisCloseSymbol(0) }
            };

        [Theory]
        [MemberData(nameof(ParenthesisCloseValidProceeding))]
        internal void WhereBuilder_ValidateSymbolParenthesisClose_ValidProceeding_DoesnNotThrowException(ISequenceSymbol nextSymbol)
        {
            // Arrange
            var parenthesisCount = 0;

            // Act
            this._fixture.SequenceOfSymbolsValidator.ValidateSymbol(new ParenthesisCloseSymbol(0), nextSymbol, 1, ref parenthesisCount);
        }

        #endregion
    }
}

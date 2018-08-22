using DynamicWhereBuilder.Exceptions;
using System;
using Xunit;

namespace DynamicWhereBuilder.Tests.Exceptions
{
    public class ParseExceptionTests
    {
        [Fact]
        public void ParseException_MessageOnly_OnlyShowsMessage()
        {
            // Arrange
            var expectedMessage = "message";
            var parseException = new ParseException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, parseException.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ParseException_MessageWithQueryIndex_ShowsMessageAndQueryIndexPart(int queryIndex)
        {
            // Arrange
            var expectedMessage = "message" + Environment.NewLine;
            expectedMessage += $"QueryPart index: {queryIndex}";

            var parseException = new ParseException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, parseException.Message);
        }
    }
}

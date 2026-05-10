using FluentAssertions;
using SnsTestReceiver.Api.Helpers;
using Xunit;

namespace SnsTestReceiver.Api.Tests.Helpers
{
    public class LogSanitizerTests
    {
        [Theory]
        [InlineData("GET", "GET")]
        [InlineData(null, null)]
        [InlineData("/api/messages", "/api/messages")]
        [InlineData("", "")]
        [InlineData("abc\u0000def\u001Fghi", "abcdefghi")]
        [InlineData("path\r\n?query=1", "path?query=1")]
        [InlineData("\tPOST\n", "POST")]
        public void SanitizeForLog_should_remove_control_characters(string input, string expected)
        {
            // Given
            // When
            var result = input.SanitizeForLog();
            
            // Then
            result.Should().Be(expected);
        }
    }
}

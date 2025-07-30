namespace Sunlix.NET.DDD.BaseTypes.Tests
{
    public class ErrorTests
    {
        [Fact]
        public void Error_should_be_initialized_correctly()
        {
            var sut = new Error("404", "Resource not found");

            sut.Code.Should().Be("404");
            sut.Message.Should().Be("Resource not found");
        }

        [Fact]
        public void Errors_with_same_code_should_be_equal()
        {
            var sut = new Error("404", "Resource not found");
            var other = new Error("404", "Not found");

            sut.Equals(other).Should().BeTrue();
        }

        [Fact]
        public void Errors_with_different_code_should_not_be_equal()
        {
            var sut = new Error("404", "Bad request");
            var other = new Error("400", "Bad request");

            sut.Equals(other).Should().BeFalse();
        }

        [Fact]
        public void Errors_with_null_code_should_be_equal()
        {
            var sut = new Error(null!, "Resource not found");
            var other = new Error(null!, "Bad request");

            sut.Equals(other).Should().BeTrue();
        }

        [Fact]
        public void Error_should_not_be_equal_to_null()
        {
            var sut = new Error(null!, "Resource not found");
            sut.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void ToString_should_return_correct_value()
        {
            var sut = new Error("404", "Resource not found");
            var result = sut.ToString();

            result.Should().Be("404: Resource not found");
        }
    }
}

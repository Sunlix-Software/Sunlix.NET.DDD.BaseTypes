namespace Sunlix.NET.DDD.BaseTypes.Tests
{
    public class UnitTests
    {
        [Fact]
        public void Units_should_always_be_equal()
        {
            var unit1 = Unit.value;
            var unit2 = Unit.value;

            unit1.Should().Be(unit2);
            (unit1 == unit2).Should().BeTrue();
        }
    }
}

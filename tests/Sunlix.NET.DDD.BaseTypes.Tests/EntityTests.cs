namespace Sunlix.NET.DDD.BaseTypes.Tests
{
    public class EntityTests
    {
        [Theory]
        [MemberData(nameof(GetValidEntitiesWithIds))]
        public void Entity_with_defined_id_should_be_initialized_correctly<TId>(TestEntity<TId> sut, TId id)
            where TId : IEquatable<TId>
        {
            sut.Id.Should().Be(id);
            sut.IsTransient().Should().BeFalse();
        }

        [Fact]
        public void Constructor_should_throw_exception_when_id_is_null()
        {
            Action act = () => new TestEntity<string>(null!);

            act.Should()
               .Throw<ArgumentException>()
               .WithMessage("Entity Id should not be null or default value.*")
               .And.ParamName.Should().Be("id");
        }

        [Fact]
        public void Constructor_should_throw_exception_when_id_is_default()
        {
            Action act = () => new TestEntity<int>(0);

            act.Should()
               .Throw<ArgumentException>()
               .WithMessage("Entity Id should not be null or default value.*")
               .And.ParamName.Should().Be("id");
        }

        [Theory]
        [MemberData(nameof(GetTransientEntitiesWithIds))]
        public void Transient_entity_should_be_initialized_correctly<TId>(TestEntity<TId> sut, TId id)
            where TId : IEquatable<TId>
        {
            sut.Id.Should().Be(id);
            sut.IsTransient().Should().BeTrue();
        }

        public class TestEntity<TId> : Entity<TId> where TId : IEquatable<TId>
        {
            public TestEntity() : base() { }
            public TestEntity(TId id) : base(id) { }
        }

        public static IEnumerable<object[]> GetValidEntitiesWithIds()
        {
            yield return new object[] { new TestEntity<int>(1), 1 };
            yield return new object[] { new TestEntity<string>("ae61412c-0bab-47f0-82f7-39520ede5639"), "ae61412c-0bab-47f0-82f7-39520ede5639" };
        }

        public static IEnumerable<object[]> GetDefaultIds()
        {
            yield return new object[] { 0 };
            yield return new object[] { (string)null! };
        }

        public static IEnumerable<object[]> GetTransientEntitiesWithIds()
        {
            yield return new object[] { new TestEntity<int>(), 0 };
            yield return new object[] { new TestEntity<string>(), null! };
        }

    }
}

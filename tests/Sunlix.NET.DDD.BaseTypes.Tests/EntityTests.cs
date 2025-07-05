namespace Sunlix.NET.DDD.BaseTypes.Tests
{
    public class EntityTests
    {
        [Theory]
        [MemberData(nameof(GetEntityWithResultTuples))]
        public void Entity_with_defined_id_should_be_initialized_correctly<TId>(TestEntity<TId> sut, TId id)
            where TId : IEquatable<TId>
        {
            sut.Id.Should().Be(id);
            sut.IsTransient.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GetTransientEntityWithResultTuples))]
        public void Transient_entity_should_be_initialized_correctly<TId>(TestEntity<TId> sut, TId id)
            where TId : IEquatable<TId>
        {
            sut.Id.Should().Be(id);
            sut.IsTransient.Should().BeTrue();
        }

        public class TestEntity<TId> : Entity<TId> where TId : IEquatable<TId>
        {
            public TestEntity() : base() { }
            public TestEntity(TId id) : base(id) { }
        }

        public static IEnumerable<object[]> GetEntityWithResultTuples()
        {
            yield return new object[] { new TestEntity<int>(1), 1 };
            yield return new object[] { new TestEntity<string>("ae61412c-0bab-47f0-82f7-39520ede5639"), "ae61412c-0bab-47f0-82f7-39520ede5639" };
        }

        public static IEnumerable<object[]> GetTransientEntityWithResultTuples()
        {
            yield return new object[] { new TestEntity<int>(), 0 };
            yield return new object[] { new TestEntity<string>(), null! };
            yield return new object[] { new TestEntity<int>(0), 0 };
            yield return new object[] { new TestEntity<string>(null!), null! };
        }

    }
}

namespace Sunlix.NET.DDD.BaseTypes.Tests
{
    public class IdEqualityComparerTests
    {
        [Fact]
        public void Entity_should_be_equal_to_itself()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");

            sut.Equals(entity, entity).Should().BeTrue();
        }

        [Fact]
        public void Entity_should_not_be_equal_to_null()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");

            sut.Equals(entity, null).Should().BeFalse();
        }

        [Fact]
        public void Entities_of_same_type_with_same_id_should_be_equal()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity1 = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");
            var entity2 = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");

            sut.Equals(entity1, entity2).Should().BeTrue();
        }

        [Fact]
        public void Entities_with_different_id_should_not_be_equal()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity1 = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");
            var entity2 = new TestEntity("ec52b5ee-3847-4612-9fe5-7f8c838f55c5");

            sut.Equals(entity1, entity2).Should().BeFalse();
        }

        [Fact]
        public void Transient_entities_should_be_considered_equal()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            sut.Equals(entity1, entity2).Should().BeTrue();
        }

        [Fact]
        public void Transient_entity_should_not_be_equal_to_non_transient_entity()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity1 = new TestEntity();
            var entity2 = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");

            sut.Equals(entity1, entity2).Should().BeFalse();
        }

        [Fact]
        public void Entities_of_different_types_should_not_be_equal()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity1 = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");
            var entity2 = new OtherTestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");

            sut.Equals(entity1, entity2).Should().BeFalse();
        }

        [Fact]
        public void Entities_with_same_unproxied_types_should_be_equal()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity1 = new TestEntity("ae61412c-0bab-47f0-82f7-39520ede5639");
            var entity2 = new TestEntityProxy<TestEntity>("ae61412c-0bab-47f0-82f7-39520ede5639");

            sut.Equals(entity1, entity2).Should().BeTrue();
        }

        [Fact]
        public void Entities_with_different_unproxied_types_should_not_be_equal()
        {
            var sut = Entity<string>.IdEqualityComparer;
            var entity1 = new TestEntityProxy<TestEntity>("ae61412c-0bab-47f0-82f7-39520ede5639");
            var entity2 = new TestEntityProxy<OtherTestEntity>("ae61412c-0bab-47f0-82f7-39520ede5639");

            sut.Equals(entity1, entity2).Should().BeFalse();
        }

        public class TestEntity : Entity<string>
        {
            public TestEntity() : base() { }
            public TestEntity(string id) : base(id) { }
        }

        public class OtherTestEntity : Entity<string>
        {
            public OtherTestEntity() : base() { }
            public OtherTestEntity(string id) : base(id) { }
        }

        public class TestEntityProxy<TEntity> : TestEntity
        {
            public TestEntityProxy() : base() { }
            public TestEntityProxy(string id) : base(id) { }
            protected override Type UnproxiedType => typeof(TEntity);
        }
    }
}

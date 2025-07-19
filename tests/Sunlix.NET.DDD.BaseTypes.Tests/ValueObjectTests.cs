
namespace Sunlix.NET.DDD.BaseTypes.Tests
{
    public class ValueObjectTests
    {
        [Fact]
        public void ValueObject_should_not_be_equal_to_null()
        {
            var sut = new TestValueObject(10, "test string");
            
            sut.Equals(null).Should().BeFalse();
            sut?.Equals((object?)null).Should().BeFalse();
        }

        [Fact]
        public void ValueObject_should_be_equal_to_itself()
        {
            var sut = new TestValueObject(10, "test string");
            
            sut.Equals(sut).Should().BeTrue();
            sut.Equals((object)sut).Should().BeTrue();
        }

        [Fact]
        public void ValueObjects_of_different_types_should_not_be_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new OtherTestValueObject(10, "test string");

            sut.Equals(other).Should().BeFalse();
            sut.Equals((object)other).Should().BeFalse();
        }

        [Fact]
        public void ValueObjects_with_different_values_should_not_be_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(100, "test string");

            sut.Equals(other).Should().BeFalse();
            sut.Equals((object)other).Should().BeFalse();
        }

        [Fact]
        public void ValueObjects_with_same_values_should_be_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(10, "test string");

            sut.Equals(other).Should().BeTrue();
            sut.Equals((object)other).Should().BeTrue();
        }

        [Fact]
        public void ValueObjects_with_same_values_and_same_unproxied_type_should_be_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObjectProxy(10, "test string");

            sut.Equals(other).Should().BeTrue();
            sut.Equals((object)other).Should().BeTrue();
        }

        [Fact]
        public void Equality_operator_should_return_true_when_both_value_objects_are_null()
        {
            ValueObject? left = null;
            ValueObject? right = null;

            (left == right).Should().BeTrue();
        }

        [Fact]
        public void Inequality_operator_should_return_false_when_both_value_objects_are_null()
        {
            ValueObject? left = null;
            ValueObject? right = null;

            (left != right).Should().BeFalse();
        }

        [Fact]
        public void Equality_operator_should_return_false_when_one_value_object_is_null()
        {
            var sut = new TestValueObject(10, "test string");
            
            (sut == null).Should().BeFalse();
            (null == sut).Should().BeFalse();
        }

        [Fact]
        public void Inequality_operator_should_return_true_when_one_value_object_is_null()
        {
            var sut = new TestValueObject(10, "test string");
            
            (sut != null).Should().BeTrue();
            (null != sut).Should().BeTrue();
        }

        [Fact]
        public void Equality_operator_should_return_true_when_value_objects_are_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(10, "test string");

            (sut == other).Should().BeTrue();
        }

        [Fact]
        public void Equality_operator_should_return_true_when_value_objects_of_same_unproxied_type_are_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObjectProxy(10, "test string");

            (sut == other).Should().BeTrue();
        }

        [Fact]
        public void Inequality_operator_should_return_false_when_value_objects_are_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(10, "test string");

            (sut != other).Should().BeFalse();
        }

        [Fact]
        public void Inequality_operator_should_return_false_when_value_objects_of_same_unproxied_type_are_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObjectProxy(10, "test string");

            (sut == other).Should().BeTrue();
        }

        [Fact]
        public void Equality_operator_should_return_false_when_value_objects_are_not_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(100, "test string");

            (sut == other).Should().BeFalse();
        }

        [Fact]
        public void Inequality_operator_should_return_true_when_value_objects_are_not_equal()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(100, "test string");
            
            (sut != other).Should().BeTrue();
        }

        [Fact]
        public void ValueObjects_with_same_values_should_have_same_hash_code()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(10, "test string");

            sut.GetHashCode().Should().Be(other.GetHashCode());
        }

        [Fact]
        public void ValueObjects_of_different_types_should_have_different_hash_codes()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new OtherTestValueObject(10, "test string");

            sut.GetHashCode().Should().NotBe(other.GetHashCode());
        }

        [Fact]
        public void ValueObjects_with_different_values_should_have_different_hash_codes()
        {
            var sut = new TestValueObject(10, "test string");
            var other = new TestValueObject(100, "test string");

            sut.GetHashCode().Should().NotBe(other.GetHashCode());
        }

        [Fact]
        public void GetHashCode_should_return_consistent_value()
        {
            var sut = new TestValueObject(10, "test string");
            var hash1 = sut.GetHashCode();
            var hash2 = sut.GetHashCode();

            hash1.Should().Be(hash2);
        }


        private abstract class TestValueObjectBase : ValueObject
        {
            public int IntProperty { get; }
            public string StringProperty { get; }

            public TestValueObjectBase(int intProperty, string stringProperty)
            {
                IntProperty = intProperty;
                StringProperty = stringProperty;
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return IntProperty;
                yield return StringProperty;
            }
        }

        private class TestValueObject : TestValueObjectBase
        {
            public TestValueObject(int intProperty, string stringProperty) : base(intProperty, stringProperty)
            {
            }
        }

        private class OtherTestValueObject : TestValueObjectBase
        {
            public OtherTestValueObject(int intProperty, string stringProperty) : base(intProperty, stringProperty)
            {
            }
        }

        private class TestValueObjectProxy : TestValueObjectBase
        {
            public TestValueObjectProxy(int intProperty, string stringProperty) : base(intProperty, stringProperty)
            {
            }

            protected override Type UnproxiedType => typeof(TestValueObject);
        }
    }
}

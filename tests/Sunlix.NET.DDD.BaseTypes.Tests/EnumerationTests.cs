namespace Sunlix.NET.DDD.BaseTypes.Tests
{
    public class EnumerationTests
    {
        [Fact]
        public void Constructor_should_throw_exception_when_value_is_negative()
        {
            Action act = () => new TestEnumeration(-1, "Test name");

            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid enumeration value: '-1'. Expected a non-negative value. (Parameter 'value')");
        }

        [Theory]
        [MemberData(nameof(GetNullOrWhiteSpaceNames))]
        public void Constructor_should_throw_exception_when_name_is_null_or_whitespace(string? name)
        {
            Action act = () => new TestEnumeration(0, name!);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Enumeration name should not be null, empty, or contain only whitespace. (Parameter 'name')");
        }

        [Fact]
        public void Enumeration_should_initialize_correctly()
        {
            var sut = TestEnumeration.Status1;

            sut.Value.Should().Be(1);
            sut.Name.Should().Be("Status1");
        }

        [Fact]
        public void GetAll_should_throw_exception_when_duplicate_value_exists()
        {
            Action act = () => TestEnumerationWithDuplicateValue.GetAll();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Duplicate enumeration value detected: '1'. Value must be unique.");
        }

        [Fact]
        public void GetAll_should_throw_exception_when_duplicate_name_exists()
        {
            Action act = () => TestEnumerationWithDuplicateName.GetAll();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Duplicate enumeration name detected: 'Status1'. Name must be unique.");
        }

        [Fact]
        public void GetAll_should_return_all_defined_enumerations()
        {
            var result = Enumeration<TestEnumeration>.GetAll();

            result.Should().Contain([TestEnumeration.Status1, TestEnumeration.Status2]);
            result.Should().HaveCount(2);
        }

        [Fact]
        public void Exists_should_return_true_when_value_exists()
        {
            var result = Enumeration<TestEnumeration>.Exists(1);
            result.Should().BeTrue();
        }

        [Fact]
        public void Exists_should_return_false_when_value_does_not_exist()
        {
            var result = Enumeration<TestEnumeration>.Exists(3);
            result.Should().BeFalse();
        }

        [Fact]
        public void Exists_should_return_true_when_name_exists()
        {
            var result = Enumeration<TestEnumeration>.Exists("Status1");
            result.Should().BeTrue();
        }

        [Fact]
        public void Exists_should_return_false_when_name_does_not_exist()
        {
            var result = Enumeration<TestEnumeration>.Exists("Status3");
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GetNullOrWhiteSpaceNames))]
        public void Exists_should_return_false_when_name_is_null_or_whitespace(string? name)
        {
            var result = Enumeration<TestEnumeration>.Exists(name!);
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GetValuesWithResults))]
        public void FromValue_should_return_enumeration_by_value(int value, TestEnumeration expextedResult)
        {
            var result = Enumeration<TestEnumeration>.FromValue(value);
            result.Should().Be(expextedResult);
        }

        [Fact]
        public void FromValue_should_throw_exception_when_value_does_not_exist()
        {
            Action act = () => Enumeration<TestEnumeration>.FromValue(3);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Enumeration value '3' is not valid for Sunlix.NET.DDD.BaseTypes.Tests.TestEnumeration.");
        }

        [Fact]
        public void FromValue_should_throw_exception_when_value_is_not_allowed()
        {
            Action act = () => Enumeration<TestEnumeration>.FromValue(-1);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Enumeration value '-1' is not valid for Sunlix.NET.DDD.BaseTypes.Tests.TestEnumeration.");
        }

        [Theory]
        [MemberData(nameof(GetValuesWithResults))]
        public void TryGetFromValue_should_set_enumeration_by_value(int value, TestEnumeration expectedEnumeration)
        {
            bool result = Enumeration<TestEnumeration>.TryGetFromValue(value, out var enumeration);

            result.Should().BeTrue();
            enumeration.Should().Be(expectedEnumeration);
        }

        [Fact]
        public void TryGetFromValue_should_set_null_when_value_does_not_exist()
        {
            bool result = Enumeration<TestEnumeration>.TryGetFromValue(3, out var enumeration);

            result.Should().BeFalse();
            enumeration.Should().BeNull();
        }

        [Fact]
        public void TryGetFromValue_should_set_null_when_value_is_not_allowed()
        {
            bool result = Enumeration<TestEnumeration>.TryGetFromValue(-1, out var enumeration);

            result.Should().BeFalse();
            enumeration.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(GetNamesWithResults))]
        public void FromName_should_return_enumeration_by_name(string name, TestEnumeration expextedResult)
        {
            var result = Enumeration<TestEnumeration>.FromName(name);
            result.Should().Be(expextedResult);
        }

        [Fact]
        public void FromName_should_throw_exception_when_name_does_not_exist()
        {
            Action act = () => Enumeration<TestEnumeration>.FromName("Status3");

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Enumeration name 'Status3' is not valid for Sunlix.NET.DDD.BaseTypes.Tests.TestEnumeration.");
        }

        [Theory]
        [MemberData(nameof(GetNullOrWhiteSpaceNames))]
        public void FromName_should_throw_exception_when_name_is_null_or_whitespace(string? name)
        {
            Action act = () => Enumeration<TestEnumeration>.FromName(name!);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"Enumeration name '{name}' is not valid for Sunlix.NET.DDD.BaseTypes.Tests.TestEnumeration.");
        }

        [Theory]
        [MemberData(nameof(GetNamesWithResults))]
        public void TryGetFromName_should_set_enumeration_by_name(string name, TestEnumeration expectedEnumeration)
        {
            bool result = Enumeration<TestEnumeration>.TryGetFromName(name, out var enumeration);

            result.Should().BeTrue();
            enumeration.Should().Be(expectedEnumeration);
        }

        [Fact]
        public void TryGetFromName_should_set_null_when_name_does_not_exist()
        {
            bool result = Enumeration<TestEnumeration>.TryGetFromName("Status3", out var enumeration);

            result.Should().BeFalse();
            enumeration.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(GetNullOrWhiteSpaceNames))]
        public void TryGetFromName_should_set_null_when_name_is_null_or_whitespace(string? name)
        {
            bool result = Enumeration<TestEnumeration>.TryGetFromName(name!, out var enumeration);

            result.Should().BeFalse();
            enumeration.Should().BeNull();
        }

        [Fact]
        public void Enumeration_should_be_equal_to_itself()
        {
            TestEnumeration.Status1
                .Equals(TestEnumeration.Status1)
                .Should().BeTrue();
        }

        [Fact]
        public void Enumerations_with_same_values_should_be_equal()
        {
            TestEnumeration.Status1
                .Equals(new TestEnumeration(1, "Different name"))
                .Should().BeTrue();
        }

        [Fact]
        public void Enumeration_should_not_be_equal_to_enumeration_with_different_value()
        {
            TestEnumeration.Status1
                .Equals(TestEnumeration.Status2)
                .Should().BeFalse();
        }

        [Fact]
        public void Enumeration_should_not_be_equal_to_null()
        {
            TestEnumeration.Status1
                .Equals(null)
                .Should().BeFalse();
        }

        [Fact]
        public void Enumeration_should_be_equal_to_itself_as_object()
        {
            TestEnumeration.Status1
                .Equals((object)TestEnumeration.Status1)
                .Should().BeTrue();
        }

        [Fact]
        public void Enumerations_with_same_values_should_be_equal_as_objects()
        {
            TestEnumeration.Status1
                .Equals((object)new TestEnumeration(1, "Different name"))
                .Should().BeTrue();
        }

        [Fact]
        public void Enumeration_should_not_be_equal_to_enumeration_with_different_value_as_object()
        {
            TestEnumeration.Status1
                .Equals((object)TestEnumeration.Status2)
                .Should().BeFalse();
        }

        [Fact]
        public void Enumeration_should_not_be_equal_to_null_as_object()
        {
            TestEnumeration.Status1
                .Equals((object)null!)
                .Should().BeFalse();
        }

        [Fact]
        public void Enumeration_should_not_be_equal_to_enumeration_of_different_type()
        {
            TestEnumeration.Status1
                .Equals((object)OtherTestEnumeration.Status1)
                .Should().BeFalse();
        }

        [Fact]
        public void Equality_operator_should_return_true_for_enumerations_with_same_values()
        {
            (TestEnumeration.Status1 == new TestEnumeration(1, "Different name"))
                .Should().BeTrue();
        }

        [Fact]
        public void Equality_operator_should_return_false_for_enumerations_with_different_values()
        {
            (TestEnumeration.Status1 == TestEnumeration.Status2)
                .Should().BeFalse();
        }

        [Fact]
        public void Equality_operator_should_return_false_for_enumerations_of_different_types()
        {
            (TestEnumeration.Status1 == OtherTestEnumeration.Status1)
                .Should().BeFalse();
        }

        [Fact]
        public void Equality_operator_should_return_true_when_both_operands_are_null()
        {
            TestEnumeration left = null!;
            TestEnumeration right = null!;
            (left == right).Should().BeTrue();
        }

        [Fact]
        public void Equality_operator_should_return_false_when_one_operand_is_null()
        {
            TestEnumeration? left = null!;
            (left == TestEnumeration.Status1).Should().BeFalse();
            (TestEnumeration.Status1 == left).Should().BeFalse();
        }

        [Fact]
        public void Inequality_operator_should_return_false_for_enumerations_with_same_values()
        {
            (TestEnumeration.Status1 != new TestEnumeration(1, "Different name"))
                .Should().BeFalse();
        }

        [Fact]
        public void Inequality_operator_should_return_true_for_enumerations_with_different_values()
        {
            (TestEnumeration.Status1 != TestEnumeration.Status2)
                .Should().BeTrue();
        }

        [Fact]
        public void Inequality_operator_should_return_true_for_enumerations_of_different_types()
        {
            (TestEnumeration.Status1 != OtherTestEnumeration.Status1)
                .Should().BeTrue();
        }

        [Fact]
        public void Inequality_operator_should_return_false_when_both_operands_are_null()
        {
            TestEnumeration left = null!;
            TestEnumeration right = null!;
            (left != right).Should().BeFalse();
        }

        [Fact]
        public void Inequality_operator_should_return_true_when_one_operand_is_null()
        {
            TestEnumeration? left = null!;
            (left != TestEnumeration.Status1).Should().BeTrue();
            (TestEnumeration.Status1 != left).Should().BeTrue();
        }

        [Fact]
        public void Enumerations_with_same_value_should_have_same_hash_code()
        {
            TestEnumeration.Status1.GetHashCode()
                .Should().Be(new TestEnumeration(1, "Different name")
                .GetHashCode());
        }

        [Fact]
        public void Enumerations_with_different_values_should_have_different_hash_codes()
        {
            TestEnumeration.Status1.GetHashCode()
                .Should().NotBe(TestEnumeration.Status2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_should_return_consistent_value()
        {
            var sut = TestEnumeration.Status1;
            var hash1 = sut.GetHashCode();
            var hash2 = sut.GetHashCode();

            hash1.Should().Be(hash2);
        }

        [Fact]
        public void CompareTo_should_return_zero_when_values_are_equal()
        {
            TestEnumeration.Status1
                .CompareTo(new TestEnumeration(1, "Different name"))
                .Should().Be(0);
        }

        [Fact]
        public void CompareTo_should_return_negative_when_compared_to_higher_value()
        {
            TestEnumeration.Status1
                .CompareTo(TestEnumeration.Status2)
                .Should().BeNegative();
        }

        [Fact]
        public void CompareTo_should_return_positive_when_compared_to_lower_value()
        {
            TestEnumeration.Status2
                .CompareTo(TestEnumeration.Status1)
                .Should().BePositive();
        }

        [Fact]
        public void CompareTo_should_return_positive_when_other_is_null()
        {
            TestEnumeration? nullEnumeration = null;
            TestEnumeration.Status1
                .CompareTo(nullEnumeration)
                .Should().BePositive();
        }

        [Fact]
        public void CompareTo_object_should_return_zero_when_values_are_equal()
        {
            TestEnumeration.Status1
                .CompareTo((object)new TestEnumeration(1, "Different name"))
                .Should().Be(0);
        }

        [Fact]
        public void CompareTo_object_should_return_negative_when_compared_to_higher_value()
        {
            TestEnumeration.Status1
                .CompareTo((object)TestEnumeration.Status2)
                .Should().BeNegative();
        }

        [Fact]
        public void CompareTo_object_should_return_positive_when_compared_to_lower_value()
        {
            TestEnumeration.Status2
                .CompareTo((object)TestEnumeration.Status1)
                .Should().BePositive();
        }

        [Fact]
        public void CompareTo_object_should_return_positive_when_other_is_null()
        {
            object? nullEnumeration = null;
            TestEnumeration.Status1
                .CompareTo(nullEnumeration)
                .Should().BePositive();
        }

        [Fact]
        public void CompareTo_object_should_throw_exception_when_compared_to_different_enumeration_type()
        {
            var sut = TestEnumeration.Status1;
            var other = OtherTestEnumeration.Status1;

            Action act = () => sut.CompareTo(other);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Object type mismatch. Ensure the correct type is used. (Parameter 'obj')");
        }

        public static IEnumerable<object[]> GetNullOrWhiteSpaceNames()
        {
            yield return new object[] { null! };
            yield return new object[] { "" };
            yield return new object[] { "  " };
        }

        public static IEnumerable<object[]> GetValuesWithResults()
        {
            yield return new object[] { 1, TestEnumeration.Status1 };
            yield return new object[] { 2, TestEnumeration.Status2 };
        }

        public static IEnumerable<object[]> GetNamesWithResults()
        {
            yield return new object[] { "Status1", TestEnumeration.Status1 };
            yield return new object[] { "Status2", TestEnumeration.Status2 };
        }
    }

    public class TestEnumeration : Enumeration<TestEnumeration>
    {
        public static readonly TestEnumeration Status1 = new TestEnumeration(1, nameof(Status1));
        public static readonly TestEnumeration Status2 = new TestEnumeration(2, nameof(Status2));

        public TestEnumeration(int value, string name) : base(value, name) { }
    }

    public class OtherTestEnumeration : Enumeration<OtherTestEnumeration>
    {
        public static readonly OtherTestEnumeration Status1 = new OtherTestEnumeration(1, nameof(Status1));
        public static readonly OtherTestEnumeration Status2 = new OtherTestEnumeration(2, nameof(Status2));

        public OtherTestEnumeration(int value, string name) : base(value, name) { }
    }

    public class TestEnumerationWithDuplicateValue : Enumeration<TestEnumerationWithDuplicateValue>
    {
        public static readonly TestEnumerationWithDuplicateValue Status1 = new TestEnumerationWithDuplicateValue(1, nameof(Status1));
        public static readonly TestEnumerationWithDuplicateValue Status2 = new TestEnumerationWithDuplicateValue(1, nameof(Status2));

        public TestEnumerationWithDuplicateValue(int value, string name) : base(value, name) { }
    }

    public class TestEnumerationWithDuplicateName : Enumeration<TestEnumerationWithDuplicateName>
    {
        public static readonly TestEnumerationWithDuplicateName Status1 = new TestEnumerationWithDuplicateName(1, nameof(Status1));
        public static readonly TestEnumerationWithDuplicateName Status2 = new TestEnumerationWithDuplicateName(2, nameof(Status1));

        public TestEnumerationWithDuplicateName(int value, string name) : base(value, name) { }
    }
}

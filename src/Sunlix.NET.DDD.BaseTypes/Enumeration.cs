using System.Reflection;

namespace Sunlix.NET.DDD.BaseTypes
{
    public abstract class Enumeration<T> : ValueObject, IComparable, IComparable<Enumeration<T>>
        where T : Enumeration<T>
    {
        private static readonly Lazy<IReadOnlyList<T>> EnumerationsLazy = new(ValidateAndGetEnumerations);

        private static readonly Lazy<IReadOnlyDictionary<int, T>> EnumerationValuesLazy = new(()
            => EnumerationsLazy.Value.ToDictionary(x => x.Value));

        private static readonly Lazy<IReadOnlyDictionary<string, T>> EnumerationNamesLazy = new(()
            => EnumerationsLazy.Value.ToDictionary(x => x.Name));

        private static IReadOnlyDictionary<int, T> EnumerationValues => EnumerationValuesLazy.Value;
        private static IReadOnlyDictionary<string, T> EnumerationNames => EnumerationNamesLazy.Value;

        public int Value { get; init; }
        public string Name { get; init; }

        protected Enumeration(int value, string name)
        {
            Validate(value, name);
            Value = value;
            Name = name;
        }

        public static T FromValue(int value)
        {
            if (value < 0)
                throw new ArgumentException($"Invalid enumeration value: '{value}'. Expected a non-negative value.", nameof(value));

            return TryGetFromValue(value, out T? enumeration)
                ? enumeration!
                : throw new InvalidOperationException($"Enumeration value '{value}' is not valid for {typeof(T)}.");
        }

        public static bool TryGetFromValue(int value, out T? enumeration)
        {
            if (value < 0)
            {
                enumeration = null;
                return false;
            }

            return EnumerationValues.TryGetValue(value, out enumeration);
        }

        public static T FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Enumeration name should not be null, empty, or contain only whitespace.", nameof(name));

            return TryGetFromName(name, out T? enumeration)
                ? enumeration!
                : throw new InvalidOperationException($"Enumeration name '{name}' is not valid for {typeof(T)}.");
        }

        public static bool TryGetFromName(string name, out T? enumeration)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                enumeration = null;
                return false;
            }
            return EnumerationNames.TryGetValue(name, out enumeration);
        }

        public static bool Exists(int value) => EnumerationValues.ContainsKey(value);

        public static bool Exists(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return EnumerationNames.ContainsKey(name);
        }

        public static IEnumerable<T> GetAll() => EnumerationValues.Values;

        public override int GetHashCode() => HashCode.Combine(UnproxiedType, Value);

        public int CompareTo(Enumeration<T>? other) => other is null ? 1 : Value.CompareTo(other.Value);

        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is not Enumeration<T> other)
                throw new ArgumentException($"Object type mismatch. Ensure the correct type is used.", nameof(obj));

            return CompareTo(other);
        }

        public override string ToString() => Name;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        private static IReadOnlyList<T> GetEnumerations()
        {
            var enumerationType = typeof(T);
            return enumerationType
                .GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly)
                .Where(f => enumerationType.IsAssignableFrom(f.FieldType))
                .Select(f => f.GetValue(null))
                .Cast<T>()
                .ToList().AsReadOnly();
        }

        private static IReadOnlyList<T> ValidateAndGetEnumerations()
        {
            var enumerationsList = GetEnumerations();
            if (enumerationsList.GroupBy(x => x.Value).FirstOrDefault(g => g.Count() > 1) is { } duplicateValue)
                throw new InvalidOperationException($"Duplicate enumeration value detected: '{duplicateValue.Key}'. Value must be unique.");

            if (enumerationsList.GroupBy(x => x.Name).FirstOrDefault(g => g.Count() > 1) is { } duplicateName)
                throw new InvalidOperationException($"Duplicate enumeration name detected: '{duplicateName.Key}'. Name must be unique.");

            return enumerationsList;
        }

        private static void Validate(int value, string name)
        {
            if (value < 0)
                throw new ArgumentException($"Invalid enumeration value: '{value}'. Expected a non-negative value.", nameof(value));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Enumeration name should not be null, empty, or contain only whitespace.", nameof(name));
        }
    }
}

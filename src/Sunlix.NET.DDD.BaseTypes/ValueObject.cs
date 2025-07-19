namespace Sunlix.NET.DDD.BaseTypes
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        private int? _cachedHashCode;

        public bool Equals(ValueObject? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (UnproxiedType != other.UnproxiedType) return false;
            return SatisfiesStructuralEquality(other);
        }

        public override bool Equals(object? obj)
            => obj is ValueObject other && Equals(other);

        public override int GetHashCode()
        {
            if (_cachedHashCode.HasValue) return _cachedHashCode.Value;
            
            _cachedHashCode = GetEqualityComponents()
                .Prepend(UnproxiedType)
                .Aggregate(new HashCode(), (hash, component) =>
                {
                    hash.Add(component);
                    return hash;
                }).ToHashCode();
            return _cachedHashCode.Value;
        }

        public static bool operator ==(ValueObject? left, ValueObject? right)
            => left is null ? right is null : left.Equals(right);
        public static bool operator !=(ValueObject? left, ValueObject? right)
            => !(left == right);

        protected virtual Type UnproxiedType => GetType();

        protected abstract IEnumerable<object> GetEqualityComponents();

        private bool SatisfiesStructuralEquality(ValueObject other)
            => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
}

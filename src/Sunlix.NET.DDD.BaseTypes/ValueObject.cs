namespace Sunlix.NET.DDD.BaseTypes
{
    /// <include file="XmlDocs/ValueObject.xml" path="doc/members/member[@name='T:ValueObject']/*" />
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        private int? _cachedHashCode;

        /// <include file="XmlDocs/ValueObject.xml" path="doc/members/member[@name='P:UnproxiedType']/*" />
        protected virtual Type UnproxiedType => GetType();

        /// <include file="XmlDocs/ValueObject.xml" path="doc/members/member[@name='M:Equals(ValueObject)']/*" />
        public virtual bool Equals(ValueObject? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (UnproxiedType != other.UnproxiedType) return false;
            return SatisfiesStructuralEquality(other);
        }

        /// <include file="XmlDocs/ValueObject.xml" path="doc/members/member[@name='M:Equals(System.Object)']/*" />
        public override bool Equals(object? obj)
            => obj is ValueObject other && Equals(other);

        /// <include file="XmlDocs/ValueObject.xml" path="doc/members/member[@name='M:GetHashCode']/*" />
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

        /// <include file="XmlDocs/ValueObject.xml" path="doc/members/member[@name='M:GetEqualityComponents']/*" />
        protected abstract IEnumerable<object> GetEqualityComponents();

        public static bool operator ==(ValueObject? left, ValueObject? right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(ValueObject? left, ValueObject? right)
            => !(left == right);

        private bool SatisfiesStructuralEquality(ValueObject other)
            => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
}

namespace Sunlix.NET.DDD.BaseTypes
{
    /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='T:Sunlix.NET.DDD.BaseTypes.Entity`1']/*" />
    public abstract class Entity<TId> where TId : IEquatable<TId>
    {
        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Entity`1.#ctor']/*" />
        protected Entity() { }

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Entity`1.#ctor(`0)']/*" />
        protected Entity(TId id) => Id = id;

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='P:Sunlix.NET.DDD.BaseTypes.Entity`1.Id']/*" />
        public virtual TId? Id { get; protected set; }

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='P:Sunlix.NET.DDD.BaseTypes.Entity`1.UnproxiedType']/*" />
        protected virtual Type UnproxiedType => GetType();

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='P:Sunlix.NET.DDD.BaseTypes.Entity`1.IdEqualityComparer']/*" />
        public static IEqualityComparer<Entity<TId>> IdEqualityComparer { get; } = new IdComparer();

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='P:Sunlix.NET.DDD.BaseTypes.Entity`1.IsTransient']/*" />
        public virtual bool IsTransient => EqualityComparer<TId>.Default.Equals(Id, default);

        private static bool SatisfiesIdentifierEquality(TId? id, TId? otherId) =>
            id is null ? otherId is null : id.Equals(otherId);

        private sealed class IdComparer : IEqualityComparer<Entity<TId>>
        {
            public bool Equals(Entity<TId>? x, Entity<TId>? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;

                return x.UnproxiedType == y.UnproxiedType
                       && SatisfiesIdentifierEquality(x.Id, y.Id);
            }

            public int GetHashCode(Entity<TId>? obj)
            {
                if (obj is null) return 0;
                return HashCode.Combine(obj.UnproxiedType, obj.Id);
            }
        }
    }
}

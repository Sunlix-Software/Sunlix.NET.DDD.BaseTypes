namespace Sunlix.NET.DDD.BaseTypes
{
    /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='T:Entity']/*" />
    public abstract class Entity<TId> where TId : IEquatable<TId>
    {
        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='M:Entity.#ctor']/*" />
        protected Entity() { }

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='M:Entity.#ctor(`0)']/*" />
        protected Entity(TId id) => Id = id;

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='P:Entity.Id']/*" />
        public virtual TId? Id { get; protected set; }

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='P:UnproxiedType']/*" />
        protected virtual Type UnproxiedType => GetType();

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='P:IdEqualityComparer']/*" />
        public static IEqualityComparer<Entity<TId>> IdEqualityComparer =>
            EqualityComparer<Entity<TId>>.Create((x, y) =>
            x is null ? y is null : y is not null
            && x.UnproxiedType == y.UnproxiedType
            && SatisfiesIdentifierEquality(x.Id, y.Id));

        /// <include file="XmlDocs/Entity.xml" path="doc/members/member[@name='M:IsTransient']/*" />
        public virtual bool IsTransient() => EqualityComparer<TId>.Default.Equals(Id, default);

        private static bool SatisfiesIdentifierEquality(TId? id, TId? otherId) =>
            id is null ? otherId is null : id.Equals(otherId);
    }
}

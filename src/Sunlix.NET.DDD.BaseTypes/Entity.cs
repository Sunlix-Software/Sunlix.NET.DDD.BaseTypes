namespace Sunlix.NET.DDD.BaseTypes
{
    public abstract class Entity<TId> where TId : IEquatable<TId>
    {
        protected Entity() { }
        protected Entity(TId id) => Id = id;
        public virtual TId? Id { get; protected set; }

        public virtual bool IsTransient => EqualityComparer<TId>.Default.Equals(Id, default);

        protected virtual Type UnproxiedType => GetType();

        public static IEqualityComparer<Entity<TId>> IdEqualityComparer =>
            EqualityComparer<Entity<TId>>.Create((x, y) =>
            x is null ? y is null : y is not null
            && x.UnproxiedType == y.UnproxiedType
            && SatisfiesIdentifierEquality(x.Id, y.Id));

        private static bool SatisfiesIdentifierEquality(TId? id, TId? otherId) =>
            id is null ? otherId is null : id.Equals(otherId);
    }
}

namespace Sunlix.NET.DDD.BaseTypes
{
    /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='T:Unit']/*" />
    public readonly struct Unit : IEquatable<Unit>
    {
        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='F:value']/*" />
        public static readonly Unit value;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Equals(Unit)']/*" />
        public bool Equals(Unit other) => true;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Equals(System.Object)']/*" />
        public override bool Equals(object? obj) => obj is Unit;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:GetHashCode']/*" />
        public override int GetHashCode() => 0;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:ToString']/*" />
        public override string ToString() => "()";

        public static bool operator ==(Unit left, Unit right) => left.Equals(right);
        public static bool operator !=(Unit left, Unit right) => !(left == right);
    }

    /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='T:UnitExtensions']/*" />
    public static class UnitExtensions
    {
        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:AsUnitTask(System.Threading.Tasks.Task)']/*" />
        public static async Task<Unit> AsUnitTask(this Task task)
        {
            await task.ConfigureAwait(false);
            return Unit.value;
        }

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:AsUnitFunc``1(System.Action{``0})']/*" />
        public static Func<TResult, Unit> AsUnitFunc<TResult>(this Action<TResult> action)
        {
            return result =>
            {
                action(result);
                return Unit.value;
            };
        }

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:AsUnitFunc(System.Action)']/*" />
        public static Func<Unit> AsUnitFunc(this Action action)
        {
            return () =>
            {
                action();
                return Unit.value;
            };
        }
    }
}

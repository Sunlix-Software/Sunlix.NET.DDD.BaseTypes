namespace Sunlix.NET.DDD.BaseTypes
{
    /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='T:Sunlix.NET.DDD.BaseTypes.Unit']/*" />
    public readonly struct Unit : IEquatable<Unit>
    {
        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='F:Sunlix.NET.DDD.BaseTypes.Unit.value']/*" />
        public static readonly Unit value;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Unit.Equals(Unit)']/*" />
        public bool Equals(Unit other) => true;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Unit.Equals(System.Object)']/*" />
        public override bool Equals(object? obj) => obj is Unit;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Unit.GetHashCode']/*" />
        public override int GetHashCode() => 0;

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Unit.ToString']/*" />
        public override string ToString() => "()";

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Unit.op_Equality(Sunlix.NET.DDD.BaseTypes.Unit,Sunlix.NET.DDD.BaseTypes.Unit)']/*" />
        public static bool operator ==(Unit left, Unit right) => left.Equals(right);

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Unit.op_Inequality(Sunlix.NET.DDD.BaseTypes.Unit,Sunlix.NET.DDD.BaseTypes.Unit)']/*" />
        public static bool operator !=(Unit left, Unit right) => !(left == right);
    }

    /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='T:Sunlix.NET.DDD.BaseTypes.UnitExtensions']/*" />
    public static class UnitExtensions
    {
        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.UnitExtensions.AsUnitTask(System.Threading.Tasks.Task)']/*" />
        public static async Task<Unit> AsUnitTask(this Task task)
        {
            await task.ConfigureAwait(false);
            return Unit.value;
        }

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.UnitExtensions.AsUnitFunc``1(System.Action{``0})']/*" />
        public static Func<TResult, Unit> AsUnitFunc<TResult>(this Action<TResult> action)
        {
            return result =>
            {
                action(result);
                return Unit.value;
            };
        }

        /// <include file="XmlDocs/Unit.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.UnitExtensions.AsUnitFunc(System.Action)']/*" />
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

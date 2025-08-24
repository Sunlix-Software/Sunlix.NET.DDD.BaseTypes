namespace Sunlix.NET.DDD.BaseTypes
{
    /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='T:Sunlix.NET.DDD.BaseTypes.Error']/*" />
    public class Error : ValueObject
    {
        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='P:Sunlix.NET.DDD.BaseTypes.Error.Code']/*" />
        public string Code { get; }

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='P:Sunlix.NET.DDD.BaseTypes.Error.Message']/*" />
        public string Message { get; }

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Error.#ctor(System.String,System.String)']/*" />
        public Error(string code, string message) => (Code, Message) = (code, message);

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Error.GetEqualityComponents']/*" />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='M:Sunlix.NET.DDD.BaseTypes.Error.ToString']/*" />
        public override string ToString() => string.Format($"{Code}: {Message}");
    }
}

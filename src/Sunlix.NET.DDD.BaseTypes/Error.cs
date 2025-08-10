namespace Sunlix.NET.DDD.BaseTypes
{
    /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='T:Error']/*" />
    public class Error : ValueObject
    {
        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='P:Code']/*" />
        public string Code { get; }

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='P:Message']/*" />
        public string Message { get; }

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='M:Error.#ctor(System.String,System.String)']/*" />
        public Error(string code, string message) => (Code, Message) = (code, message);

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='M:GetEqualityComponents']/*" />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

        /// <include file="XmlDocs/Error.xml" path="doc/members/member[@name='M:ToString']/*" />
        public override string ToString() => string.Format($"{Code}: {Message}");
    }
}

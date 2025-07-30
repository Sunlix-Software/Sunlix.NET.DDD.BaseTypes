namespace Sunlix.NET.DDD.BaseTypes
{
    public class Error : ValueObject
    {
        public string Code { get; }
        public string Message { get; }
        public Error(string code, string message) => (Code, Message) = (code, message);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

        public override string ToString() => string.Format($"{Code}: {Message}");
    }
}

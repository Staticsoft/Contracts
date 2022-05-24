namespace Staticsoft.Contracts.Tests
{
    public class AuthenticationFake : Authentication
    {
        public string Value { get; set; }

        public AuthenticationHeaders Get()
            => new()
            {
                Name = "Authentication",
                Value = Value
            };
    }
}

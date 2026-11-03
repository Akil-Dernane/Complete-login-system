namespace Complete_Login_System.Exceptions.Claim
{
    public class InvalidClaimValueException : ApiException
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;

        public InvalidClaimValueException(string type, string value) : base($"Invalid claim value {value } for claim type {type}")
        {
        }
    }
}

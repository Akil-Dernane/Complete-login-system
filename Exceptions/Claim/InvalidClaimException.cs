namespace Complete_Login_System.Exceptions.Claim
{
    public class InvalidClaimException : ApiException
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;
        public InvalidClaimException(string type) : base($"Invalid Claim type {type}")
        {}
    }
}

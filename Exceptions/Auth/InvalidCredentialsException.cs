namespace Complete_Login_System.Exceptions.Auth
{
    public class InvalidCredentialsException : ApiException
    {
        public override int StatusCode => StatusCodes.Status401Unauthorized;
        public InvalidCredentialsException() : base("Invalid email or password.")
        {
        }
    }
}

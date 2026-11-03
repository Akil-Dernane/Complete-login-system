namespace Complete_Login_System.Exceptions
{
    public sealed class ConflictException(string message) 
        : ApiException(message)
    {
        public override int StatusCode => StatusCodes.Status409Conflict;
    }
}

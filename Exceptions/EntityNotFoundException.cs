namespace Complete_Login_System.Exceptions
{
    public class EntityNotFoundException(string message) : ApiException(message)
    {
        public override int StatusCode => StatusCodes.Status404NotFound;
    }
}

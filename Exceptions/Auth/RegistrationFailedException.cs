using Complete_Login_System.Exceptions.Interfaces;

namespace Complete_Login_System.Exceptions.Auth
{
    public class RegistrationFailedException : ApiException, IHasErrors
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;
        public IReadOnlyList<string> Errors { get; }
        public RegistrationFailedException(IEnumerable<string> errors) : base("User registration failed.")
        {
            Errors = errors.ToArray();
        }


    }
}

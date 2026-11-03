using Complete_Login_System.Exceptions.Interfaces;

namespace Complete_Login_System.Exceptions.Auth
{
    public class PasswordChangeFailedException : ApiException, IHasErrors
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;
        public IReadOnlyList<string> Errors { get; }
        public PasswordChangeFailedException(IEnumerable<string> errors) : base("Failed to change password.")
        {
            Errors = errors.ToArray();
        }

    }
}

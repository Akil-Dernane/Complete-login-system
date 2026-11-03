namespace Complete_Login_System.Exceptions.User
{
    public class UserNotFoundException : EntityNotFoundException
    {
        public UserNotFoundException(Guid userId) : base($"User: {userId} was not found.")
        {
        }
    }
}

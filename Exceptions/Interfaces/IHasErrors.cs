namespace Complete_Login_System.Exceptions.Interfaces
{
    public interface IHasErrors
    {
        IReadOnlyList<string> Errors { get; }
    }
}

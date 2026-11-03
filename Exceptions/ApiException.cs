namespace Complete_Login_System.Exceptions
{
    public abstract class ApiException : Exception
    {
        protected ApiException(string message) : base(message) { }

        public abstract int StatusCode { get; }
    }
}

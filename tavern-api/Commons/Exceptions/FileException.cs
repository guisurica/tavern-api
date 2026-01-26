namespace tavern_api.Commons.Exceptions;

public class FileException : Exception
{
    public FileException() : base() { }
    public FileException(string message) : base(message) { }
    public FileException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

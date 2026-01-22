namespace tavern_api.Commons.Exceptions;

public class DomainException : Exception
{
    public DomainException() : base()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception ex) : base(message, ex)
    {
    }
}

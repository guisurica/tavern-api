namespace tavern_api.Commons.Exceptions;

public class InfrastructureException : Exception
{
    public InfrastructureException() : base()
    {
    }

    public InfrastructureException(string message) : base(message)
    {
    }

    public InfrastructureException(string message, Exception ex) : base(message, ex)
    {
    }
}

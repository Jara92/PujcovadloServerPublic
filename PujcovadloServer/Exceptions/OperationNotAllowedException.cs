namespace PujcovadloServer.Exceptions;

public class OperationNotAllowedException : Exception
{
    public OperationNotAllowedException() : base()
    {
    }

    public OperationNotAllowedException(string message) : base(message)
    {
    }
}
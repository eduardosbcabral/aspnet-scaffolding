namespace Scaffolding.Models;

internal class ExceptionContainer
{
    public Exception Exception { get; set; }

    public ExceptionContainer(Exception exception)
    {
        Exception = exception;
    }
}
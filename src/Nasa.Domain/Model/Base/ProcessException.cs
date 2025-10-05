namespace Nasa.Domain.Model;

public class ProcessException : Exception
{
    public ProcessException() : base() { }
    public ProcessException(string message) : base(message) { }
    public ProcessException(string message, Exception inner) : base(message, inner) { }
}
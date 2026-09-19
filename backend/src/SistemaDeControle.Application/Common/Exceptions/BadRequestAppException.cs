namespace SistemaDeControle.Application.Common.Exceptions;

public class BadRequestAppException : Exception
{
    public BadRequestAppException(string message) : base(message) { }
}

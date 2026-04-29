namespace UrlShortener.Application.Exceptions;

public class InvalidUrlException : Exception
{
    public InvalidUrlException(string message) : base(message) { }
}
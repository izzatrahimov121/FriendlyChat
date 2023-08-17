namespace Chat.MVC.Exceptions;

public class IncorrectFileFormatException : Exception
{
    public IncorrectFileFormatException(string? message) : base(message)
    {
    }
}

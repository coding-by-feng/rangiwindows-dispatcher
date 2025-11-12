namespace KiwiTools.Exceptions;

/// <summary>
/// Base exception for all Kiwi specific errors. Mirrors the Java KiwiException type.
/// </summary>
public class KiwiException : Exception
{
    public KiwiException(string errorCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}

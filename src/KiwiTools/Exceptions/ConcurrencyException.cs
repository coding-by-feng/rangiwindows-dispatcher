namespace KiwiTools.Exceptions;

public sealed class ConcurrencyException : KiwiException
{
    public ConcurrencyException(string message, Exception? innerException = null)
        : base(ErrorCode.ConcurrencyConflict, message, innerException)
    {
    }
}

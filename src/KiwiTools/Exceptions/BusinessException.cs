namespace KiwiTools.Exceptions;

public sealed class BusinessException : KiwiException
{
    public BusinessException(string message, string errorCode = ErrorCode.ValidationFailed, Exception? innerException = null)
        : base(errorCode, message, innerException)
    {
    }
}

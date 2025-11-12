namespace KiwiTools.Exceptions;

public sealed class NotFoundException : KiwiException
{
    public NotFoundException(string message, Exception? innerException = null)
        : base(ErrorCode.ResourceNotFound, message, innerException)
    {
    }
}

namespace KiwiTools.Results;

/// <summary>
/// Represents the outcome of a service operation.
/// Inspired by the Java Result objects in the legacy kiwi-tools module.
/// </summary>
/// <param name="Success">Flag that indicates the request completed without an error.</param>
/// <param name="ErrorCode">Machine friendly code that uniquely identifies an error.</param>
/// <param name="Message">Human friendly error message.</param>
/// <param name="Payload">Optional payload returned by the operation.</param>
public sealed record OperationResult<T>(bool Success, string? ErrorCode, string? Message, T? Payload)
{
    /// <summary>
    /// Creates a successful result with an optional payload and message.
    /// </summary>
    public static OperationResult<T> Ok(T? payload = default, string? message = null) =>
        new(true, null, message, payload);

    /// <summary>
    /// Creates a failure result with the provided code and message.
    /// </summary>
    public static OperationResult<T> Fail(string errorCode, string message) =>
        new(false, errorCode, message, default);

    /// <summary>
    /// Creates a failure result from an exception.
    /// </summary>
    public static OperationResult<T> FromException(Exception exception, string? errorCode = null)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return new(false, errorCode ?? exception.GetType().Name, exception.Message, default);
    }
}

/// <summary>
/// Non generic operation result that is frequently used for controller endpoints.
/// </summary>
public sealed record OperationResult(bool Success, string? ErrorCode, string? Message)
{
    public static OperationResult Ok(string? message = null) => new(true, null, message);

    public static OperationResult Fail(string errorCode, string message) => new(false, errorCode, message);

    public static OperationResult FromException(Exception exception, string? errorCode = null)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return new(false, errorCode ?? exception.GetType().Name, exception.Message);
    }
}

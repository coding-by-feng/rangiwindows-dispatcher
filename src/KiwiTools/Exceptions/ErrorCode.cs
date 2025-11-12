namespace KiwiTools.Exceptions;

/// <summary>
/// Standardised error codes aligned with the historical Kiwi Java module.
/// </summary>
public static class ErrorCode
{
    public const string Unknown = "KIWI-000";
    public const string ValidationFailed = "KIWI-100";
    public const string ResourceNotFound = "KIWI-404";
    public const string ConcurrencyConflict = "KIWI-409";
    public const string Unauthorized = "KIWI-401";
    public const string Forbidden = "KIWI-403";
}

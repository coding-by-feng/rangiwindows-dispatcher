namespace KiwiTools.Validation;

/// <summary>
/// Lightweight validation helpers roughly equivalent to the Guard utility in the Java code base.
/// </summary>
public static class Guard
{
    public static void AgainstNull<T>(T? value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    public static void AgainstNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{parameterName}' cannot be null or whitespace.", parameterName);
        }
    }

    public static void AgainstOutOfRange<T>(T value, T min, T max, string parameterName)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Parameter '{parameterName}' must be between {min} and {max}.");
        }
    }
}

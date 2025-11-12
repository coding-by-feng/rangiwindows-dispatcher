using System.Diagnostics;
using System.Threading;

namespace KiwiTools.Diagnostics;

/// <summary>
/// Provides ambient storage for correlation identifiers similar to MDC in the Java stack.
/// </summary>
public static class CorrelationContext
{
    private static readonly AsyncLocal<Stack<string>> CorrelationStack = new();

    public static string? CurrentId => CorrelationStack.Value is { Count: > 0 } stack ? stack.Peek() : Activity.Current?.TraceId.ToString();

    public static IDisposable Push(string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        var stack = CorrelationStack.Value ??= new Stack<string>();
        stack.Push(correlationId);

        return new PopOnDispose(stack);
    }

    private sealed class PopOnDispose : IDisposable
    {
        private readonly Stack<string> _stack;
        private bool _disposed;

        public PopOnDispose(Stack<string> stack) => _stack = stack;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_stack.Count > 0)
            {
                _stack.Pop();
            }

            _disposed = true;
        }
    }
}

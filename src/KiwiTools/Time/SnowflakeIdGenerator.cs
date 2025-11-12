using System.Threading;

namespace KiwiTools.Time;

/// <summary>
/// Port of the Twitter Snowflake identifier used in the legacy kiwi-tools Java module.
/// Supports 41-bit timestamp, 10-bit worker id, and 12-bit sequence.
/// </summary>
public sealed class SnowflakeIdGenerator
{
    private const long Twepoch = 1577836800000L; // 2020-01-01T00:00:00Z
    private const int WorkerIdBits = 10;
    private const int SequenceBits = 12;

    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);

    private readonly object _lock = new();
    private readonly long _workerId;

    private long _lastTimestamp = -1L;
    private long _sequence;

    public SnowflakeIdGenerator(long workerId)
    {
        if (workerId is < 0 or > MaxWorkerId)
        {
            throw new ArgumentOutOfRangeException(nameof(workerId), workerId, $"Worker id must be between 0 and {MaxWorkerId}.");
        }

        _workerId = workerId;
    }

    public long NextId()
    {
        lock (_lock)
        {
            var timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException($"System clock moved backwards. Refusing to generate id for {_lastTimestamp - timestamp}ms.");
            }

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    timestamp = WaitForNextMillis(timestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Twepoch) << (WorkerIdBits + SequenceBits)) |
                   (_workerId << SequenceBits) |
                   _sequence;
        }
    }

    private static long GetCurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private long WaitForNextMillis(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            Thread.SpinWait(1);
            timestamp = GetCurrentTimestamp();
        }

        return timestamp;
    }
}

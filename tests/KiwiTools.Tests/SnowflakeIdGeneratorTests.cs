using System.Collections.Generic;
using KiwiTools.Time;
using Xunit;

namespace KiwiTools.Tests;

public class SnowflakeIdGeneratorTests
{
    [Fact]
    public void NextId_ReturnsUniqueValues()
    {
        var generator = new SnowflakeIdGenerator(1);

        var ids = new HashSet<long>();
        for (var i = 0; i < 1000; i++)
        {
            ids.Add(generator.NextId());
        }

        Assert.Equal(1000, ids.Count);
    }

    [Fact]
    public void NextId_IsMonotonicallyIncreasing()
    {
        var generator = new SnowflakeIdGenerator(1);

        var previous = generator.NextId();
        for (var i = 0; i < 1000; i++)
        {
            var next = generator.NextId();
            Assert.True(next > previous);
            previous = next;
        }
    }
}

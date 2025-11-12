using KiwiTools.Diagnostics;
using KiwiTools.Time;
using Microsoft.Extensions.DependencyInjection;

namespace KiwiTools.DependencyInjection;

/// <summary>
/// Helper methods to wire kiwi-tools services into the ASP.NET Core dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKiwiTools(this IServiceCollection services, long snowflakeWorkerId = 0)
    {
        services.AddSingleton(new SnowflakeIdGenerator(snowflakeWorkerId));
        services.AddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();
        return services;
    }
}

/// <summary>
/// Accessor abstraction to make correlation identifiers testable.
/// </summary>
public interface ICorrelationIdAccessor
{
    string? GetCurrentId();
}

internal sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
    public string? GetCurrentId() => CorrelationContext.CurrentId;
}

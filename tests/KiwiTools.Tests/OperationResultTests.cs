using KiwiTools.Exceptions;
using KiwiTools.Results;
using Xunit;

namespace KiwiTools.Tests;

public class OperationResultTests
{
    [Fact]
    public void Ok_SetsSuccessToTrue()
    {
        var result = OperationResult.Ok("Completed");

        Assert.True(result.Success);
        Assert.Null(result.ErrorCode);
        Assert.Equal("Completed", result.Message);
    }

    [Fact]
    public void Fail_SetsErrorCode()
    {
        var result = OperationResult.Fail(ErrorCode.Unknown, "Failed");

        Assert.False(result.Success);
        Assert.Equal(ErrorCode.Unknown, result.ErrorCode);
    }

    [Fact]
    public void FromException_UsesExceptionDetails()
    {
        var exception = new BusinessException("Validation failed");

        var result = OperationResult.FromException(exception);

        Assert.False(result.Success);
        Assert.Equal(nameof(BusinessException), result.ErrorCode);
        Assert.Equal("Validation failed", result.Message);
    }
}

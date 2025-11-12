# Kiwi Tools for .NET

The legacy `kiwi-tools` module from the Java microservice stack is now available as a .NET 8 class library.  
This document explains the design of the .NET port, how it maps to the original Java concepts, and how to consume it from ASP.NET Core microservices.

## Package overview

```
src/
└── KiwiTools/
    ├── Diagnostics/
    │   └── CorrelationContext.cs
    ├── DependencyInjection/
    │   └── ServiceCollectionExtensions.cs
    ├── Exceptions/
    │   ├── BusinessException.cs
    │   ├── ConcurrencyException.cs
    │   ├── ErrorCode.cs
    │   ├── KiwiException.cs
    │   └── NotFoundException.cs
    ├── Results/
    │   ├── OperationResult.cs
    │   └── PagedResult.cs
    ├── Security/
    │   └── PasswordHasher.cs
    ├── Time/
    │   └── SnowflakeIdGenerator.cs
    └── Validation/
        └── Guard.cs
```

The project targets **.NET 8** (`net8.0`) and enables nullable reference types.  The `KiwiTools` package has a single external dependency on `Microsoft.Extensions.DependencyInjection.Abstractions` so that it can provide registration helpers.

## Mapping from the Java module

| Java component (legacy)           | .NET equivalent                              | Notes |
|----------------------------------|-----------------------------------------------|-------|
| `Result<T>` / `Result`           | `OperationResult<T>` / `OperationResult`      | Exposes static factory members for success, failure, and exception translation. |
| `PageResult<T>`                  | `PagedResult<T>`                              | Adds convenience properties (`TotalPages`, `HasNextPage`, `HasPreviousPage`). |
| `KiwiException` hierarchy        | `KiwiException` plus `BusinessException`, `NotFoundException`, `ConcurrencyException` | Preserves the `errorCode` concept used by the Java services. |
| `ErrorCode` constants            | `ErrorCode` static class                      | Uses the same identifiers (e.g., `KIWI-404`). |
| MDC / correlation utilities      | `CorrelationContext` + `ICorrelationIdAccessor` | Uses `AsyncLocal` to match the thread-local behaviour. |
| BCrypt hashing helpers           | `PasswordHasher`                              | Uses PBKDF2 with SHA-256, configurable iteration count, and constant time verification. |
| Snowflake ID generator           | `SnowflakeIdGenerator`                        | Same bit layout (41-bit timestamp, 10-bit worker, 12-bit sequence). |
| Guard utility                    | `Guard` static class                          | Provides null/whitespace/out-of-range checks used across services. |
| Spring `@Configuration` helpers  | `AddKiwiTools` extension method               | Registers DI services for Snowflake and correlation ID access. |

## Usage in ASP.NET Core microservices

1. **Reference the library**: include the `KiwiTools` project in your solution or consume the compiled package.
2. **Register shared services**:

   ```csharp
   using KiwiTools.DependencyInjection;

   var builder = WebApplication.CreateBuilder(args);

   builder.Services.AddKiwiTools(snowflakeWorkerId: 3);
   ```

   The registration adds a singleton `SnowflakeIdGenerator` and an `ICorrelationIdAccessor` that wraps the ambient `CorrelationContext`.

3. **Wrapping controller responses**:

   ```csharp
   using KiwiTools.Results;

   [HttpGet("/orders/{orderId}")]
   public OperationResult<OrderDto> GetOrder(Guid orderId)
   {
       var order = _orderService.Get(orderId);
       return order is null
           ? OperationResult<OrderDto>.Fail(ErrorCode.ResourceNotFound, $"Order {orderId} not found")
           : OperationResult<OrderDto>.Ok(order);
   }
   ```

4. **Generating identifiers**:

   ```csharp
   using KiwiTools.Time;

   public class OrderFactory
   {
       private readonly SnowflakeIdGenerator _idGenerator;

       public OrderFactory(SnowflakeIdGenerator idGenerator) => _idGenerator = idGenerator;

       public Order Create(CreateOrderRequest request)
       {
           var id = _idGenerator.NextId();
           return new Order(id, request.CustomerId, request.Items);
       }
   }
   ```

5. **Hashing passwords**:

   ```csharp
   using KiwiTools.Security;

   var hash = PasswordHasher.HashPassword("p@ssw0rd");
   var isValid = PasswordHasher.Verify("p@ssw0rd", hash);
   ```

6. **Validation**:

   ```csharp
   using KiwiTools.Validation;

   Guard.AgainstNull(request, nameof(request));
   Guard.AgainstNullOrWhiteSpace(request.Email, nameof(request.Email));
   Guard.AgainstOutOfRange(request.Quantity, min: 1, max: 100, nameof(request.Quantity));
   ```

## Error handling strategy

* All custom exceptions inherit from `KiwiException` and carry a stable `ErrorCode`.
* `OperationResult.FromException` can convert unexpected errors into a serialisable response object.
* The separation between `BusinessException`, `NotFoundException`, and `ConcurrencyException` mirrors the Java service contract so existing error mapping middleware can be ported easily.

## Testing

The `tests/KiwiTools.Tests` project demonstrates how to test the utilities with xUnit:

* `PasswordHasherTests` ensures the PBKDF2 implementation behaves as expected.
* `SnowflakeIdGeneratorTests` verifies uniqueness and monotonic ordering of IDs.
* `OperationResultTests` covers the response helpers.

The tests are ready to run with `dotnet test` once the .NET SDK is installed.

## Migration checklist from Java to .NET

1. Replace usages of `Result<T>` / `Result` with the corresponding `OperationResult` types.
2. Swap Java exception types (`BusinessException`, `NotFoundException`, etc.) for the .NET equivalents.
3. Update any manual MDC handling to use `CorrelationContext.Push`/`CurrentId` within middleware or message handlers.
4. Replace BCrypt password hashing calls with `PasswordHasher`.
5. Share the same Snowflake worker identifiers across JVM and .NET services if both are deployed side-by-side.
6. Leverage `Guard` helpers when porting service layer validations to maintain parity with defensive checks in the Java code.

## Roadmap considerations

* Introduce structured logging enrichers for Serilog/NLog using `CorrelationContext`.
* Provide middleware packages (`KiwiTools.AspNetCore`) for automatic correlation ID management.
* Offer a package for message queue consumers integrating with the diagnostics context.


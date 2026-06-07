# Thushar.Logging

A lightweight, structured, and developer-friendly logging library for .NET applications.

## Features

* Simple setup and configuration
* Structured logging support
* Multiple log levels
* Exception logging
* Dependency Injection integration
* Built on top of Microsoft.Extensions.Logging

## Installation

Install the package from NuGet:

```bash
dotnet add package Thushar.Logging
```

## Quick Start

### Register Normal Logging

```csharp
builder.Services.AddLogger();
```

### Register Structured Logging

```csharp
builder.Services.AddLogger(options => 
{ 
    options.UseStructuredLogging() 
});
```

### Inject and Use

```csharp
public class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger logger)
    {
        _logger = logger;
    }

    public void ProcessOrder(Guid orderId)
    {
        _logger.LogInformation(
            "Processing order {OrderId}",
            orderId);
    }
}
```

### TraceId injection

- Implement this only the room method.
- Example: lambda - FunctionHandler, Console - Program.cs

```csharp
public void FunctionHandler(ILambdaContext context)
{
    var traceId = Guid.NewGuid().ToString();
    using (Logger.BeginTraceScope(traceId))
    {
        Logger.LogInformation("Hello from Lambda! {data}", "some data");
        Logger.LogError(new Exception("sfdasd"), "Hello from Lambda!");
    }
}
```

## Log Levels

### Information

```csharp
_logger.LogInformation(
    "Application started");
```

### Warning

```csharp
_logger.LogWarning(
    "Disk space is running low");
```

### Error

```csharp
_logger.LogError(
    "Failed to process payment");
```

### Exception

```csharp
try
{
    ProcessPayment();
}
catch (Exception ex)
{
    _logger.LogError(
        ex,
        "Payment processing failed");
}
```

## Supported Frameworks

| Version | Framework |
| ------- | --------- |
| 8.x.x   | .NET 8    |

## Versioning

Thushar.Logging follows semantic versioning.

* Patch versions contain bug fixes.
* Minor versions contain new features.
* Major versions may contain breaking changes.

## Contributing

Issues, feature requests, and pull requests are welcome.

## License

This project is licensed under the MIT License.

## Contact

In case of any doubt, feature request pin them on Github Issues:

Instagram: [@iam.thushar](https://www.instagram.com/iam.thushar)
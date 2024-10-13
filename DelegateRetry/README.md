# RetryService

Use this SDK when you have a method that you want to fire but want to ensure that it is executed. The SDK guarantees that
the method will be retried for a maximum number of attempts until it succeeds or fails (the failure is not an error but
actually a valid result). If an error/exception occurs, the SDK will ensure a retry.

## Integration Guide

To use the `DelegateRetry.Services` DLL in your ASP.NET Core application, follow these steps:

### 1. Set Up Your `appsettings.json`

Configure your application settings in `appsettings.json`. Make sure to include the necessary sections for your retry
options, connection strings, and any other required configurations. Example:

```json

{
   "RetryOptions": {
      "WorkerOptions": {
         "HandlerName": "RetryHandler",
         "WorkerType": "FixedDelay",
         "IsActive": true,
         "DelayInSeconds": 2,
         "ErrorDelayInSeconds": 4
      },
      "StorageOptions": {
         "ConnectionString": "Server=localhost;Database=JobUtils;User Id=SA;Password=YourStrongPassword123;trusted_connection=false;Persist Security Info=False;Encrypt=False",
         "CommandTimeout": 30,
         "Procedures": {
            "UspInsertTask": "usp_InsertTask",
            "UspUpdateTask": "usp_UpdateTask",
            "UspGetById": "usp_GetById",
            "UspGetAllWithStatus": "usp_GetAllWithStatus"
         }
      },
      "MaxRetries": 5
   }
}

```

2. Set Up Your Stored Procedures
   Ensure that your database has the required stored procedures defined in RetryService.sql. This script should contain
   the necessary SQL commands for handling retry logic.

3. . Modify Program.cs
   In your Program.cs, set up the services and configure the application as follows:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = builder.Configuration;

// Add retry services
builder.Services.AddRetryServices(builder.Configuration, connectionString => new SqlConnection(connectionString));
```

4. To use the Retry Service:

- Make the method you want to retry static.
- Annotate the method with the [Register] attribute.
- Inject IDelegateService into your class.
- Use the DelegateRetry method to call your registered method.

```csharp
public class SomeClass
{
    [Register]
    public static string Func1(string param1, int param2)
    {
        // Your method logic here
    }
}

public class YourService
{
    private readonly IDelegateService _delegateService;

    public YourService(IDelegateService delegateService)
    {
        _delegateService = delegateService;
    }

    public async Task DoSomething()
    {
        var result = await _delegateService.DelegateRetry(SomeClass.Func1, "example", 42);
        // Use the result
    }
}
```
Important: Calling SomeClass.Func1() directly will not trigger the retry mechanism. You must use _delegateService.DelegateRetry() to ensure retry functionality.
Important: while developing you method e.g. Func1() , make sure to only throw exceptions when you can not handle it , because the library will keep trying for the max # of retrys 

## Supported Method Signatures
- Up to 10 parameters
- Parameters can be primitive types, complex objects, or collections
- Methods can have no parameters
- Return types can be any type or void

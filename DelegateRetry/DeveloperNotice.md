How It Works
The Retry Service uses reflection to find methods annotated with [Register]. When these methods are called through DelegateRetry, the service will:

Attempt to execute the method.
If it fails, retry up to the number of times specified in MaxRetries.
Wait between retries as specified in the configuration.
Best Practices
Use for operations likely to experience transient failures (e.g., network calls, database operations).
Avoid using for operations likely to fail due to non-transient issues.
Adjust MaxRetries, DelayInSeconds, and ErrorDelayInSeconds based on your specific use case.
Always use _delegateService.DelegateRetry() to call methods you want to be retried.

we cannot use it with instance methods , because we only save the inputs values and implementation, but we dont save the instance itself, so if there is some instance variable, it will not be accessed since its instance is gone.

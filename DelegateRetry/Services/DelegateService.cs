using System.Text.Json;
using DelegateRetry.Db;
using DelegateRetry.Models;

namespace DelegateRetry.Services;
public interface IDelegateService
{
    // Original method with one parameter
    public Task<Guid> DelegateRetry<T1, TResult>(Func<T1, TResult> function, T1 param);

    // Overload for two parameters
    public Task<Guid> DelegateRetry<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 param1, T2 param2);

    // Overload for three parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 param1, T2 param2, T3 param3);

    // Overload for four parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4);

    // Overload for five parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);

    // Overload for six parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

    // Overload for seven parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

    // Overload for eight parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

    // Overload for nine parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9);

    // Overload for ten parameters
    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10);

    public Task<Guid> DelegateRetry<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2);

    public Task<Guid> DelegateRetry<T1>(Action<T1> action, T1 param);

    public Task<Guid> DelegateRetry(Action function);

}

internal class DelegateService(TaskStorage storage) : IDelegateService
{

    public Task<Guid> DelegateRetry<T1, TResult>(Func<T1, TResult> function, T1 param)
    {
        return RecordFunctionAsyncInternal(function, param);
    }

    public Task<Guid> DelegateRetry<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 param1, T2 param2)
    {
        return RecordFunctionAsyncInternal(function, param1, param2 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 param1, T2 param2, T3 param3)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3, param4 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3, param4, param5 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3, param4, param5, param6 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3, param4, param5, param6, param7 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3, param4, param5, param6, param7, param8 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3, param4, param5, param6, param7, param8, param9 );
    }

    public Task<Guid> DelegateRetry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
    {
        return RecordFunctionAsyncInternal(function, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10 );
    }
    
    // void methods :
    
    // Overload for void methods with no parameters
    public async Task<Guid> DelegateRetry(Action function)
    {
        return await RecordFunctionAsyncInternal(function, Array.Empty<object>());
    }

    // Overload for void methods with one parameter
    public Task<Guid> DelegateRetry<T1>(Action<T1> action, T1 param)
    {
        return RecordFunctionAsyncInternal(action, param);
    }

    // Overload for void methods with two parameters
    public Task<Guid> DelegateRetry<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
    {
        return RecordFunctionAsyncInternal(action, param1, param2);
    }


    private async Task<Guid> RecordFunctionAsyncInternal(Delegate function, params object[] parameters)
    {
        var method = function.Method;
        var declaringType = method.DeclaringType;
        var functionName = $"{declaringType?.Name}.{method.Name}";

        Console.WriteLine($"Recording params values of {functionName} , params= {string.Join(", ", parameters)}");
        
        var parametersJson = "{}";
        parametersJson = JsonSerializer.Serialize(parameters);

        // Create a task record
        var taskRecord = CreateTaskRecord(functionName, parametersJson);

        // Save the task record in the database
        await storage.Save(taskRecord);

        return taskRecord.Id;
    }

    private static TaskRecord CreateTaskRecord(string functionId, string input)
    {
        // Create a new task record
        var taskRecord = new TaskRecord
        {
            Id = Guid.NewGuid(), // Generate a unique ID for the task
            FunctionName = functionId,
            SerializedInput = input,
            Status = Status.Pending, // Set initial status to Pending
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        return taskRecord;
    }
}

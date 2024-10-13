using System.Text.Json;
using DelegateRetry.Db;
using DelegateRetry.Models;
using JobUtils.Workers;
using Microsoft.Extensions.Logging;

namespace DelegateRetry.Core;

internal class RetryHandler(RetryOptions options, ILogger<RetryHandler> logger, TaskStorage storage)
    : IHandler
{
    
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[RetryHandler][ProcessAsync]...");
        var tasksToRetry = await storage.QueryTasks(Status.Pending, Status.Error);

        foreach (var task in tasksToRetry.Cast<TaskRecord?>())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Cancellation requested. Stopping retry process.");
                break;
            }

            var function = MethodRegistry.GetRegisteredMethod(task.FunctionName);
            if (function == null)
            {
                logger.LogError($"Function {task.FunctionName} not found!");
                await UpdateTask(task, null, Status.GiveUp, $"Function {task.FunctionName} not found");
                cancellationToken.ThrowIfCancellationRequested();
                continue;
            }

            try
            {
                var parameters =  JsonSerializer.Deserialize<object[] >(task.SerializedInput);
                var result = MethodRegistry.Execute(function, parameters);
                await UpdateTask(task, result, Status.Success);
            }
            catch (Exception ex)
            {
                var status = task.RetryCount >= options.MaxRetries - 1 ? Status.GiveUp : Status.Failure;
                logger.LogError($"Error processing task {task.Id}: {ex.Message} status: {status} (Attempt {task.RetryCount})");
                await UpdateTask(task, null, Status.Error, ex.Message);
            }
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
    private async Task UpdateTask(TaskRecord task, string serializedOutput, Status status, string errorMessage = null)
    {
        task.RetryCount++;
        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;
        task.SerializedOutput =errorMessage ?? serializedOutput;
        
        try
        {
            await storage.Update(task);
        }
        catch (Exception updateEx)
        {
            logger.LogError($"Failed to update task {task.Id}: {updateEx.Message}");
        }
    }
    
}
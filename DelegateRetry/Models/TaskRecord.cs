//
// using JobUtils.Abstract;
// using Microsoft.Extensions.Logging;
//
namespace DelegateRetry.Models;

public enum Status
{
    Pending='P',
    Success='S',
    Failure='F',
    Error='E',
    GiveUp='G'
}

internal interface ITaskRecord;
internal class TaskRecord:ITaskRecord
{
    public Guid Id { get; set; }
    
    public string FunctionName { get; set; }
    public Status Status { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string SerializedInput { get; set; }
    public string? SerializedOutput { get; set; }
}



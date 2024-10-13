using JobUtils;

namespace DelegateRetry.Models;

public class RetryOptions
{
    public required StorageOptions StorageOptions { get; init; }
    public required WorkerConfig WorkerOptions { get; init; }
    public int MaxRetries { get; init; }
    
}

public class StorageOptions
{
    public required string ConnectionString { get; init; } = string.Empty;
    public required int CommandTimeout { get; init; } = 2; // 30 seconds
    public required StorageProcedures Procedures { get; set; }

    public class StorageProcedures
    {
        public required string UspInsertTask { get; set; }
        public required string UspUpdateTask { get; set; }
        public required string UspGetById { get; set; }
        public required string UspGetAllWithStatus { get; set; }
    }
}
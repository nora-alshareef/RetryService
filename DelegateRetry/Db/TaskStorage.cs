using System.Data;
using System.Data.Common;
using DelegateRetry.Models;
using Microsoft.Extensions.Logging;

namespace DelegateRetry.Db;

internal class TaskStorage(
    RetryOptions options,
    Func<DbConnection> connectionFactory,
    ILogger<TaskStorage> logger)
{
    private readonly string _insertTask = options.StorageOptions.Procedures.UspInsertTask;
    private readonly string _updateTask = options.StorageOptions.Procedures.UspUpdateTask;
    private readonly string _getById = options.StorageOptions.Procedures.UspGetById;
    private readonly string _getAllWithStatus = options.StorageOptions.Procedures.UspGetAllWithStatus;

    internal async Task Save(ITaskRecord record)
    {
        // Serialize Input and Result before inserting

        if(record is TaskRecord taskRecord)

        await ExecuteNonQueryAsync(_insertTask, command =>
        {
            AddIdParameter(command, taskRecord.Id);
            AddParameter(command, "@Input", taskRecord.SerializedInput);
            AddParameter(command, "@Status", (char)taskRecord.Status);
            AddParameter(command, "@FunctionName", taskRecord.FunctionName);
            AddParameter(command, "@CreatedAt", taskRecord.CreatedAt);
            AddParameter(command, "@RetryCount", taskRecord.RetryCount);
        });
    }

    public async Task Update(ITaskRecord record)
    {

        if(record is TaskRecord taskRecord)
        await ExecuteNonQueryAsync(_updateTask, command =>
        {
            AddIdParameter(command, taskRecord.Id);
            AddParameter(command, "@Status", (char)taskRecord.Status);
            AddParameter(command, "@UpdatedAt", taskRecord.UpdatedAt);
            AddParameter(command, "@Output",taskRecord.SerializedOutput);
        });
    }

    public async Task<ITaskRecord> GetById(Guid id)
    {
        ITaskRecord record = null;

        await using var connection = connectionFactory();
        try
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = _getById;
            command.CommandType = CommandType.StoredProcedure;

            AddIdParameter(command, id);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                record = new TaskRecord
                {
                    Id = id,
                    SerializedInput = reader["Input"].ToString(),
                    Status = (Status)Convert.ToChar(reader["Status"]),
                    FunctionName = reader["FunctionName"].ToString(),
                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                    UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                    SerializedOutput = reader["Output"].ToString(),
                };
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving record by ID {Id}", id);
        }

        return record;
    }

    public async Task<List<ITaskRecord>> QueryTasks(params Status[] statuses)
    {
        var records = new List<ITaskRecord>();

        await using var connection = connectionFactory();
        try
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = _getAllWithStatus;
            command.CommandType = CommandType.StoredProcedure;

            var statusList = string.Join(",", statuses.Select(s => (char)s));
            AddParameter(command, "@Statuses", statusList);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var record = new TaskRecord
                {
                    Id = (Guid)reader["Id"],
                    SerializedInput = reader["Input"].ToString(),
                    Status = (Status)Convert.ToChar(reader["Status"]),
                    FunctionName = reader["FunctionName"].ToString(),
                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                    UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                    SerializedOutput = reader["Output"].ToString(),
                };
                records.Add(record);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving records with status {Status}", statuses);
        }

        return records;
    }

    private async Task ExecuteNonQueryAsync(string storedProcedure, Action<DbCommand> setParameters)
    {
        await using var connection = connectionFactory();
        try
        {
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = options.StorageOptions.CommandTimeout;

            setParameters(command);

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[JobUtils] Error executing stored procedure {StoredProcedure}", storedProcedure);
        }
    }

    private static void AddIdParameter<TId>(DbCommand command, TId taskId)
    {
        if (taskId != null)
        {
            if (typeof(TId) == typeof(Guid))
                AddParameter(command, "@Id", (Guid)(object)taskId);
            else if (typeof(TId) == typeof(long))
                AddParameter(command, "@Id", (long)(object)taskId);
            else if (typeof(TId) == typeof(int))
                AddParameter(command, "@Id", (int)(object)taskId);
            else if (typeof(TId) == typeof(string))
                AddParameter(command, "@Id", (string)(object)taskId);
            else
                throw new ArgumentException($"[JobUtils][DbRetryStorage] Unsupported taskId type: {typeof(TId).Name}");
        }
        else throw new ArgumentException($"[JobUtils][DbRetryStorage] taskId is null");
    }

    private static void AddParameter(DbCommand command, string parameterName, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value; // Handle null values appropriately
        command.Parameters.Add(parameter);
    }

}



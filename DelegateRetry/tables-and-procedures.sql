CREATE TABLE DelegatedRecords (
TaskId UNIQUEIDENTIFIER PRIMARY KEY,
Status CHAR(1) NOT NULL, -- Assuming Status is a single character (W, E, S)
FunctionName VARCHAR(50) NULL,
CreatedAt DATETIME NOT NULL,
UpdatedAt DATETIME NOT NULL,
Data NVARCHAR(MAX) NULL,
OutputData NVARCHAR(MAX) NULL,
RetryCount INT NOT NULL DEFAULT 0, -- Adding RetryCount with a default value of 0
rowVersion ROWVERSION -- Adding a rowVersion column for concurrency
);

--------------------------------------------

CREATE PROCEDURE usp_InsertTask
@TaskId UNIQUEIDENTIFIER,
@Status CHAR(1), -- Assuming Status is a single character (W, E, S)
@FunctionName VARCHAR(50) NULL,
@CreatedAt DATETIME,
@Data NVARCHAR(MAX) NULL,
@RetryCount INT = 0 -- Add RetryCount parameter with a default value
AS
BEGIN
INSERT INTO DelegatedRecords (TaskId, Status, FunctionName, CreatedAt, UpdatedAt, Data, OutputData, RetryCount)
VALUES (@TaskId, @Status, @FunctionName, @CreatedAt, GETDATE(), @Data, NULL, @RetryCount)
END

------------------------
CREATE PROCEDURE usp_UpdateTask
@TaskId UNIQUEIDENTIFIER,
@Status CHAR(1), -- Assuming Status is a single character (W, E, S)
@UpdatedAt DATETIME,
@OutputData NVARCHAR(MAX) NULL,
@RetryCount INT = NULL -- Optional RetryCount parameter
AS
BEGIN
UPDATE DelegatedRecords
SET
Status = @Status,
UpdatedAt = @UpdatedAt,
OutputData = @OutputData,
RetryCount = COALESCE(@RetryCount, RetryCount) -- Update RetryCount if provided
WHERE TaskId = @TaskId -- No need for rowVersion in the WHERE clause
END
-------------------

CREATE PROCEDURE usp_GetById
@TaskId UNIQUEIDENTIFIER
AS
BEGIN
SELECT * FROM DelegatedRecords WHERE TaskId = @TaskId
END
-------------------
CREATE PROCEDURE usp_GetAllWithStatus
@Statuses VARCHAR(50) -- Adjust the length as needed
AS
BEGIN
SET NOCOUNT ON
DECLARE @StatusTable TABLE (Status CHAR(1))
INSERT INTO @StatusTable (Status)
SELECT TRIM(value)
FROM STRING_SPLIT(@Statuses, ',')
SELECT *
FROM DelegatedRecords
WHERE Status IN (SELECT Status FROM @StatusTable)
END
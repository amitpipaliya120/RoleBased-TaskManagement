USE [Amit_Test]
GO
CREATE TABLE tblUsersMaster (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Email NVARCHAR(255),
    PasswordHash NVARCHAR(500),
    Role NVARCHAR(50), 
    TeamId INT,        
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO
CREATE TABLE tblTeamsMaster (
    TeamId INT IDENTITY(1,1) PRIMARY KEY,
    TeamName NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO
CREATE TABLE tblTasksMaster (
    TaskId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200),
    Description NVARCHAR(MAX),
    Status NVARCHAR(50), 
    Priority NVARCHAR(50),
    AssigneeId INT,      
    ManagerId INT,       
    Deadline DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO
CREATE TABLE tblCommentsMaster (
    CommentId INT IDENTITY(1,1) PRIMARY KEY,
    TaskId INT,          
    UserId INT,          
    CommentText NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_UsersMaster_CRUD]
    @UserId INT = NULL,
    @Name NVARCHAR(100) = NULL,
    @Email NVARCHAR(255) = NULL,
    @PasswordHash NVARCHAR(500) = NULL,
    @Role NVARCHAR(50) = NULL,
    @TeamId INT = NULL,
    @IsActive BIT = NULL,
    @Flag NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Flag = 'INSERT'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblUsersMaster WHERE Email = @Email)
            BEGIN
                SELECT 'Email already exists.' AS Message, 400 AS Code;
            END
            ELSE	
            BEGIN
                INSERT INTO tblUsersMaster (Name, Email, PasswordHash, Role, TeamId, IsActive)
                VALUES (@Name, @Email, @PasswordHash, @Role, @TeamId, COALESCE(@IsActive, 1));
                SELECT 'User inserted successfully.' AS Message, 201 AS Code;
            END
        END
        ELSE IF @Flag = 'UPDATE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblUsersMaster WHERE UserId = @UserId)
            BEGIN
                UPDATE tblUsersMaster
                SET
                    Name = ISNULL(@Name, Name),
                    Email = ISNULL(@Email, Email),
                    Role = ISNULL(@Role, Role),
                    TeamId = ISNULL(@TeamId, TeamId),
                    IsActive = ISNULL(@IsActive, IsActive)
                WHERE UserId = @UserId;
                SELECT 'User updated successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'User not found for update.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblUsersMaster WHERE UserId = @UserId)
            BEGIN
                DELETE FROM tblUsersMaster WHERE UserId = @UserId;
                SELECT 'User deleted successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'User not found for delete.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'GETBYID'
        BEGIN
            SELECT TOP 1 * FROM tblUsersMaster WHERE UserId = @UserId;
            SELECT 'User fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE IF @Flag = 'GETALL'
        BEGIN
            SELECT * FROM tblUsersMaster;
            SELECT 'User list fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE
        BEGIN
            SELECT 'Invalid flag specified.' AS Message, 400 AS Code;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_TasksMaster_CRUD]
    @TaskId INT = NULL,
    @Title NVARCHAR(200) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @Status NVARCHAR(50) = NULL,
    @Priority NVARCHAR(50) = NULL,
    @AssigneeId INT = NULL,
    @ManagerId INT = NULL,
    @Deadline DATETIME = NULL,
    @Flag NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Flag = 'INSERT'
        BEGIN
            INSERT INTO tblTasksMaster (Title, Description, Status, Priority, AssigneeId, ManagerId, Deadline)
            VALUES (@Title, @Description, COALESCE(@Status, 'To Do'), @Priority, @AssigneeId, @ManagerId, @Deadline);
            SELECT 'Task inserted successfully.' AS Message, 201 AS Code;
        END
        ELSE IF @Flag = 'UPDATE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTasksMaster WHERE TaskId = @TaskId)
            BEGIN
                UPDATE tblTasksMaster
                SET
                    Title = ISNULL(@Title, Title),
                    Description = ISNULL(@Description, Description),
                    Status = ISNULL(@Status, Status),
                    Priority = ISNULL(@Priority, Priority),
                    AssigneeId = ISNULL(@AssigneeId, AssigneeId),
                    Deadline = ISNULL(@Deadline, Deadline)
                WHERE TaskId = @TaskId;
                SELECT 'Task updated successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'Task not found for update.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'UPDATE_STATUS'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTasksMaster WHERE TaskId = @TaskId)
            BEGIN
                UPDATE tblTasksMaster
                SET Status = @Status
                WHERE TaskId = @TaskId;
                SELECT 'Task status updated successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'Task not found.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTasksMaster WHERE TaskId = @TaskId)
            BEGIN
                DELETE FROM tblTasksMaster WHERE TaskId = @TaskId;
                SELECT 'Task deleted successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'Task not found for delete.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'GETBYID'
        BEGIN
            SELECT TOP 1 * FROM tblTasksMaster WHERE TaskId = @TaskId;
            SELECT 'Task fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE IF @Flag = 'GETALL'
        BEGIN
            SELECT * FROM tblTasksMaster;
            SELECT 'Task list fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE IF @Flag = 'GET_BY_ASSIGNEE'
        BEGIN
            SELECT * FROM tblTasksMaster WHERE AssigneeId = @AssigneeId;
            SELECT 'Assigned Tasks fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE
        BEGIN
            SELECT 'Invalid flag specified.' AS Message, 400 AS Code;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_TeamsMaster_CRUD]
    @TeamId INT = NULL,
    @TeamName NVARCHAR(100) = NULL,
    @Flag NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Flag = 'INSERT'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTeamsMaster WHERE TeamName = @TeamName)
            BEGIN
                SELECT 'Team name already exists.' AS Message, 400 AS Code;
            END
            ELSE	
            BEGIN
                INSERT INTO tblTeamsMaster (TeamName)
                VALUES (@TeamName);
                SELECT 'Team inserted successfully.' AS Message, 201 AS Code;
            END
        END
        ELSE IF @Flag = 'UPDATE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTeamsMaster WHERE TeamId = @TeamId)
            BEGIN
                UPDATE tblTeamsMaster
                SET TeamName = ISNULL(@TeamName, TeamName)
                WHERE TeamId = @TeamId;
                SELECT 'Team updated successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'Team not found for update.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTeamsMaster WHERE TeamId = @TeamId)
            BEGIN
                DELETE FROM tblTeamsMaster WHERE TeamId = @TeamId;
                SELECT 'Team deleted successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'Team not found for delete.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'GETBYID'
        BEGIN
            SELECT TOP 1 * FROM tblTeamsMaster WHERE TeamId = @TeamId;
            SELECT 'Team fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE IF @Flag = 'GETALL'
        BEGIN
            SELECT * FROM tblTeamsMaster;
            SELECT 'Team list fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE
        BEGIN
            SELECT 'Invalid flag specified.' AS Message, 400 AS Code;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_CommentsMaster_CRUD]
    @CommentId INT = NULL,
    @TaskId INT = NULL,
    @UserId INT = NULL,
    @CommentText NVARCHAR(MAX) = NULL,
    @Flag NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Flag = 'INSERT'
        BEGIN
            INSERT INTO tblCommentsMaster (TaskId, UserId, CommentText)
            VALUES (@TaskId, @UserId, @CommentText);
            SELECT 'Comment added successfully.' AS Message, 201 AS Code;
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblCommentsMaster WHERE CommentId = @CommentId)
            BEGIN
                DELETE FROM tblCommentsMaster WHERE CommentId = @CommentId;
                SELECT 'Comment deleted successfully.' AS Message, 200 AS Code;
            END
            ELSE
            BEGIN
                SELECT 'Comment not found.' AS Message, 404 AS Code;
            END
        END
        ELSE IF @Flag = 'GET_BY_TASKID'
        BEGIN
            SELECT * FROM tblCommentsMaster WHERE TaskId = @TaskId ORDER BY CreatedDate DESC;
            SELECT 'Comments fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE
        BEGIN
            SELECT 'Invalid flag specified.' AS Message, 400 AS Code;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
GO

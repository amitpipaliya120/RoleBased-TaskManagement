IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Amit_Test')
BEGIN
    CREATE DATABASE Amit_Test;
END
GO
USE Amit_Test;
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblRolesMaster]') AND type = N'U')
BEGIN
    CREATE TABLE tblRolesMaster (
        RoleId INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL
    );
    INSERT INTO tblRolesMaster (RoleName) VALUES ('Admin'), ('Manager'), ('User');
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblTeamsMaster]') AND type = N'U')
BEGIN
    CREATE TABLE tblTeamsMaster (
        TeamId INT IDENTITY(1,1) PRIMARY KEY,
        TeamName NVARCHAR(100) NOT NULL
    );
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblUsersMaster]') AND type = N'U')
BEGIN
    CREATE TABLE tblUsersMaster (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        RoleId INT FOREIGN KEY REFERENCES tblRolesMaster(RoleId),
        TeamId INT NULL FOREIGN KEY REFERENCES tblTeamsMaster(TeamId),
        ManagerId INT NULL,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tblUsersMaster]') AND name = 'ManagerId')
        ALTER TABLE [dbo].[tblUsersMaster] ADD ManagerId INT NULL;
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblTasksMaster]') AND type = N'U')
BEGIN
    CREATE TABLE tblTasksMaster (
        TaskId INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX),
        Status NVARCHAR(50) DEFAULT 'To Do',
        Priority NVARCHAR(50),
        AssigneeId INT NULL FOREIGN KEY REFERENCES tblUsersMaster(UserId),
        ManagerId INT NULL FOREIGN KEY REFERENCES tblUsersMaster(UserId),
        Deadline DATETIME,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblCommentsMaster]') AND type = N'U')
BEGIN
    CREATE TABLE tblCommentsMaster (
        CommentId INT IDENTITY(1,1) PRIMARY KEY,
        TaskId INT FOREIGN KEY REFERENCES tblTasksMaster(TaskId),
        UserId INT FOREIGN KEY REFERENCES tblUsersMaster(UserId),
        CommentText NVARCHAR(MAX) NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_UsersMaster_CRUD]
    @UserId INT = NULL,
    @Name NVARCHAR(100) = NULL,
    @Email NVARCHAR(255) = NULL,
    @PasswordHash NVARCHAR(MAX) = NULL,
    @RoleId INT = NULL,
    @TeamId INT = NULL,
    @ManagerId INT = NULL,
    @IsActive BIT = NULL,
    @Flag NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Flag = 'INSERT'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblUsersMaster WHERE Email = @Email)
                SELECT 'Email already exists.' AS Message, 400 AS Code;
            ELSE
            BEGIN
                INSERT INTO tblUsersMaster (Name, Email, PasswordHash, RoleId, TeamId, ManagerId, IsActive)
                VALUES (@Name, @Email, @PasswordHash, @RoleId, @TeamId, @ManagerId, ISNULL(@IsActive, 1));
                SELECT 'User inserted successfully.' AS Message, 201 AS Code;
            END
        END
        ELSE IF @Flag = 'UPDATE'
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM tblUsersMaster WHERE UserId = @UserId)
                SELECT 'User not found.' AS Message, 404 AS Code;
            ELSE
            BEGIN
                UPDATE tblUsersMaster
                SET Name=ISNULL(@Name,Name), Email=ISNULL(@Email,Email), RoleId=ISNULL(@RoleId,RoleId),
                    TeamId=ISNULL(@TeamId,TeamId), ManagerId=ISNULL(@ManagerId,ManagerId), IsActive=ISNULL(@IsActive,IsActive)
                WHERE UserId = @UserId;
                SELECT 'User updated successfully.' AS Message, 200 AS Code;
            END
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM tblUsersMaster WHERE UserId = @UserId)
                SELECT 'User not found.' AS Message, 404 AS Code;
            ELSE
            BEGIN
                UPDATE tblUsersMaster SET IsActive = 0 WHERE UserId = @UserId;
                SELECT 'User deleted successfully.' AS Message, 200 AS Code;
            END
        END
        ELSE IF @Flag = 'GETALL'
        BEGIN
            SELECT u.*, r.RoleName, m.Name AS ManagerName
            FROM tblUsersMaster u
            INNER JOIN tblRolesMaster r ON u.RoleId = r.RoleId
            LEFT JOIN tblUsersMaster m ON u.ManagerId = m.UserId;
        END
        ELSE IF @Flag = 'GETBYID'
        BEGIN
            SELECT u.*, r.RoleName, m.Name AS ManagerName
            FROM tblUsersMaster u
            INNER JOIN tblRolesMaster r ON u.RoleId = r.RoleId
            LEFT JOIN tblUsersMaster m ON u.ManagerId = m.UserId
            WHERE u.UserId = @UserId;
        END
        ELSE IF @Flag = 'GET_BY_EMAIL'
        BEGIN
            SELECT u.*, r.RoleName, m.Name AS ManagerName
            FROM tblUsersMaster u
            INNER JOIN tblRolesMaster r ON u.RoleId = r.RoleId
            LEFT JOIN tblUsersMaster m ON u.ManagerId = m.UserId
            WHERE u.Email = @Email;
        END
        ELSE IF @Flag = 'GET_MANAGERS'
        BEGIN
            SELECT u.*, r.RoleName
            FROM tblUsersMaster u
            INNER JOIN tblRolesMaster r ON u.RoleId = r.RoleId
            WHERE u.RoleId = 2 AND u.IsActive = 1;
        END
        ELSE IF @Flag = 'GET_MY_USERS'
        BEGIN
            SELECT u.*, r.RoleName, m.Name AS ManagerName
            FROM tblUsersMaster u
            INNER JOIN tblRolesMaster r ON u.RoleId = r.RoleId
            LEFT JOIN tblUsersMaster m ON u.ManagerId = m.UserId
            WHERE u.ManagerId = @ManagerId AND u.IsActive = 1;
        END
        ELSE
            SELECT 'Invalid flag.' AS Message, 400 AS Code;
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
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
                SET Title=ISNULL(@Title,Title), Description=ISNULL(@Description,Description),
                    Status=ISNULL(@Status,Status), Priority=ISNULL(@Priority,Priority),
                    AssigneeId=ISNULL(@AssigneeId,AssigneeId), Deadline=ISNULL(@Deadline,Deadline)
                WHERE TaskId = @TaskId;
                SELECT 'Task updated successfully.' AS Message, 200 AS Code;
            END
            ELSE SELECT 'Task not found.' AS Message, 404 AS Code;
        END
        ELSE IF @Flag = 'UPDATE_STATUS'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTasksMaster WHERE TaskId = @TaskId)
            BEGIN
                UPDATE tblTasksMaster SET Status = @Status WHERE TaskId = @TaskId;
                SELECT 'Task status updated.' AS Message, 200 AS Code;
            END
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTasksMaster WHERE TaskId = @TaskId)
            BEGIN
                DELETE FROM tblTasksMaster WHERE TaskId = @TaskId;
                SELECT 'Task deleted successfully.' AS Message, 200 AS Code;
            END
            ELSE SELECT 'Task not found.' AS Message, 404 AS Code;
        END
        ELSE IF @Flag = 'GETALL'
        BEGIN
            SELECT t.*, u1.Name AS AssigneeName, u2.Name AS ManagerName
            FROM tblTasksMaster t
            LEFT JOIN tblUsersMaster u1 ON t.AssigneeId = u1.UserId
            LEFT JOIN tblUsersMaster u2 ON t.ManagerId = u2.UserId;
        END
        ELSE IF @Flag = 'GET_BY_MANAGER'
        BEGIN
            SELECT t.*, u1.Name AS AssigneeName, u2.Name AS ManagerName
            FROM tblTasksMaster t
            LEFT JOIN tblUsersMaster u1 ON t.AssigneeId = u1.UserId
            LEFT JOIN tblUsersMaster u2 ON t.ManagerId = u2.UserId
            WHERE t.ManagerId = @ManagerId
               OR t.AssigneeId IN (SELECT UserId FROM tblUsersMaster WHERE ManagerId = @ManagerId)
               OR t.AssigneeId = @ManagerId;
        END
        ELSE IF @Flag = 'GET_BY_ASSIGNEE'
        BEGIN
            SELECT t.*, u1.Name AS AssigneeName, u2.Name AS ManagerName
            FROM tblTasksMaster t
            LEFT JOIN tblUsersMaster u1 ON t.AssigneeId = u1.UserId
            LEFT JOIN tblUsersMaster u2 ON t.ManagerId = u2.UserId
            WHERE t.AssigneeId = @AssigneeId;
        END
        ELSE IF @Flag = 'GETBYID'
        BEGIN
            SELECT t.*, u1.Name AS AssigneeName, u2.Name AS ManagerName
            FROM tblTasksMaster t
            LEFT JOIN tblUsersMaster u1 ON t.AssigneeId = u1.UserId
            LEFT JOIN tblUsersMaster u2 ON t.ManagerId = u2.UserId
            WHERE t.TaskId = @TaskId;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
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
        END
        ELSE IF @Flag = 'GET_BY_TASKID'
        BEGIN
            SELECT c.*, u.Name AS UserName
            FROM tblCommentsMaster c
            LEFT JOIN tblUsersMaster u ON c.UserId = u.UserId
            WHERE c.TaskId = @TaskId
            ORDER BY c.CreatedDate DESC;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
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
                SELECT 'Team name already exists.' AS Message, 400 AS Code;
            ELSE
            BEGIN
                INSERT INTO tblTeamsMaster (TeamName) VALUES (@TeamName);
                SELECT 'Team inserted successfully.' AS Message, 201 AS Code;
            END
        END
        ELSE IF @Flag = 'UPDATE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTeamsMaster WHERE TeamId = @TeamId)
            BEGIN
                UPDATE tblTeamsMaster SET TeamName = ISNULL(@TeamName, TeamName) WHERE TeamId = @TeamId;
                SELECT 'Team updated successfully.' AS Message, 200 AS Code;
            END
            ELSE SELECT 'Team not found.' AS Message, 404 AS Code;
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF EXISTS (SELECT 1 FROM tblTeamsMaster WHERE TeamId = @TeamId)
            BEGIN
                DELETE FROM tblTeamsMaster WHERE TeamId = @TeamId;
                SELECT 'Team deleted successfully.' AS Message, 200 AS Code;
            END
            ELSE SELECT 'Team not found.' AS Message, 404 AS Code;
        END
        ELSE IF @Flag = 'GETALL'
        BEGIN
            SELECT * FROM tblTeamsMaster;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 500 AS Code;
    END CATCH
END
GO
PRINT 'Database setup complete! All tables and stored procedures are up to date.';
GO

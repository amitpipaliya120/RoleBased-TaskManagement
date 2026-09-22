USE [Amit_Test]
GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblRolesMaster' and xtype='U')
BEGIN
    CREATE TABLE tblRolesMaster (
        RoleId INT PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL
    );
    INSERT INTO tblRolesMaster (RoleId, RoleName) VALUES (1, 'Admin'), (2, 'Manager'), (3, 'User');
END
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tblUsersMaster') AND name = 'RoleId')
BEGIN
    ALTER TABLE tblUsersMaster ADD RoleId INT;
END
GO
UPDATE tblUsersMaster SET RoleId = 1 WHERE Role = 'Admin';
UPDATE tblUsersMaster SET RoleId = 2 WHERE Role = 'Manager';
UPDATE tblUsersMaster SET RoleId = 3 WHERE RoleId IS NULL;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tblUsersMaster') AND name = 'Role')
BEGIN
    ALTER TABLE tblUsersMaster DROP COLUMN Role;
END
GO
IF NOT EXISTS (SELECT 1 FROM tblUsersMaster WHERE Email = 'admin@company.com')
BEGIN
    INSERT INTO tblUsersMaster (Name, Email, PasswordHash, RoleId, IsActive)
    VALUES ('System Admin', 'admin@company.com', '$2a$11$e/rA9.X9wT0o3q6/5sJ3d.K1.7h.9.8.7.6.5.4.3.2.1.0', 1, 1);
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_UsersMaster_CRUD]
    @UserId INT = NULL,
    @Name NVARCHAR(100) = NULL,
    @Email NVARCHAR(255) = NULL,
    @PasswordHash NVARCHAR(MAX) = NULL,
    @RoleId INT = NULL,
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
                INSERT INTO tblUsersMaster (Name, Email, PasswordHash, RoleId, TeamId, IsActive)
                VALUES (@Name, @Email, @PasswordHash, ISNULL(@RoleId, 3), @TeamId, 1);
                SELECT 'User registered successfully.' AS Message, 201 AS Code;
            END
        END
        ELSE IF @Flag = 'UPDATE'
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM tblUsersMaster WHERE UserId = @UserId)
            BEGIN
                SELECT 'User not found.' AS Message, 404 AS Code;
            END
            ELSE
            BEGIN
                UPDATE tblUsersMaster
                SET Name = ISNULL(@Name, Name),
                    Email = ISNULL(@Email, Email),
                    RoleId = ISNULL(@RoleId, RoleId),
                    TeamId = ISNULL(@TeamId, TeamId),
                    IsActive = ISNULL(@IsActive, IsActive)
                WHERE UserId = @UserId;
                SELECT 'User updated successfully.' AS Message, 200 AS Code;
            END
        END
        ELSE IF @Flag = 'DELETE'
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM tblUsersMaster WHERE UserId = @UserId)
            BEGIN
                SELECT 'User not found.' AS Message, 404 AS Code;
            END
            ELSE
            BEGIN
                DELETE FROM tblUsersMaster WHERE UserId = @UserId;
                SELECT 'User deleted successfully.' AS Message, 200 AS Code;
            END
        END
        ELSE IF @Flag = 'GETBYID'
        BEGIN
            SELECT u.UserId, u.Name, u.Email, u.RoleId, r.RoleName, u.TeamId, u.IsActive, u.CreatedDate
            FROM tblUsersMaster u
            LEFT JOIN tblRolesMaster r ON u.RoleId = r.RoleId
            WHERE u.UserId = @UserId;
            SELECT 'User fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE IF @Flag = 'GETALL'
        BEGIN
            SELECT u.UserId, u.Name, u.Email, u.RoleId, r.RoleName, u.TeamId, u.IsActive, u.CreatedDate
            FROM tblUsersMaster u
            LEFT JOIN tblRolesMaster r ON u.RoleId = r.RoleId;
            SELECT 'User list fetched successfully.' AS Message, 200 AS Code;
        END
        ELSE IF @Flag = 'GET_BY_EMAIL'
        BEGIN
            SELECT u.UserId, u.Name, u.Email, u.PasswordHash, u.RoleId, r.RoleName, u.TeamId, u.IsActive
            FROM tblUsersMaster u
            LEFT JOIN tblRolesMaster r ON u.RoleId = r.RoleId
            WHERE u.Email = @Email;
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

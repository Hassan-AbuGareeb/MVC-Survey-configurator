IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Authentication_DB')
BEGIN
    CREATE DATABASE Authentication_DB
    ALTER DATABASE Authentication_DB SET COMPATIBILITY_LEVEL = 160
END
GO

USE Authentication_DB
GO

IF OBJECT_ID('dbo.RefreshTokens', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RefreshTokens](
        [Id] [int] NOT NULL PRIMARY KEY,
        [ExpireDate] [DateTime] NOT NULL
    )
END
GO
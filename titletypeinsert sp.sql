USE [IMDB]
GO

/****** Object:  StoredProcedure [dbo].[TitleTypeGetInsertID]    Script Date: 12/03/2025 13:33:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TitleTypeGetInsertID] @NewTiteType VARCHAR(100)
AS
IF NOT EXISTS ( SELECT [Id] FROM TitleTypes WHERE Name = @NewTiteType)
BEGIN
    INSERT INTO [TitleTypes] ([Name]) VALUES (@NewTiteType)
END
SELECT [Id] FROM TitleTypes WHERE Name = @NewTiteType

GO



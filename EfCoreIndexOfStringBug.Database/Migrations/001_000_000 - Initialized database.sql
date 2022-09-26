CREATE TABLE [dbo].[People](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[FullName] [nvarchar](128) NOT NULL
)
GO

CREATE FUNCTION dbo.NormalizeString
(
	@str nvarchar(max)
)
RETURNS nvarchar(max)
AS
BEGIN
	--Do hypothetical stuff
	RETURN @str
END
GO

insert into People (Id, FullName) values
	(newid(), 'Mike Stanley'),
	(newid(), 'John Smith'),
	(newid(), 'Anna Constance'),
	(newid(), 'Buck Williamns'),
	(newid(), 'Stan Mark')
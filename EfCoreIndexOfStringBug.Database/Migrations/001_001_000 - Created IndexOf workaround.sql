-- =============================================
-- Description:	Workaround bug EF Core 5 & 6: IndexOf does not work 
-- when applied to strings returned by a DbFunction
-- =============================================
CREATE FUNCTION [dbo].[IndexOf]
(
	@input nvarchar(max),
	@stringToMatch nvarchar(max)
)
RETURNS int
AS
BEGIN
	return charindex(@stringToMatch, @input)
END
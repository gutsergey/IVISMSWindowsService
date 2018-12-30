USE [IVIEventsMonitor]
GO

/****** Object:  StoredProcedure [dbo].[sms_GetAbandonedRecords]    Script Date: 10/10/2018 12:52:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[sms_GetAbandonedRecords]
(
@hostid nvarchar(50),
@OutputCode int out
)
AS
BEGIN
	DECLARE @storedprocedure NVARCHAR(100)
	DECLARE @result int = 900

	DECLARE @temptbl AS TABLE
	(

		 UCID NVARCHAR(50)
		,OCI_CallingDevice NVARCHAR(50)

	)

	SELECT @storedprocedure = OBJECT_NAME(@@PROCID)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	INSERT INTO @temptbl
		SELECT UCID, OCI_CallingDevice FROM dbo.EventsCallControlConnectionAbandoned WHERE [CI_CallOriginatorType] < @result
	UPDATE dbo.EventsCallControlConnectionAbandoned
		SET [CI_CallOriginatorType] = 900
		FROM dbo.EventsCallControlConnectionAbandoned  E
		INNER JOIN @temptbl T ON T.UCID=E.UCID
	SELECT @OutputCode = COUNT(*) FROM @temptbl
	SELECT * FROM @temptbl
	INSERT INTO dbo.SmsHistory
		SELECT UCID, 
		'' Sender, 
		OCI_CallingDevice PhoneNumber, 
		@result Result, 
		'' XmlResponse, 
		@hostid HostId, 
		'' ErrorMessage,
		@storedprocedure StoredProcedure,
		GETDATE() DateTime 
		FROM @temptbl

	RETURN @OutputCode
END

GO


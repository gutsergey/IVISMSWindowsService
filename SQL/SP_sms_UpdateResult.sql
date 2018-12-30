USE [IVIEventsMonitor]
GO

/****** Object:  StoredProcedure [dbo].[sms_UpdateResult]    Script Date: 10/10/2018 12:52:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sms_UpdateResult]
	@UCID varchar(50),

	@sender nvarchar(50),
	@phonenumber nvarchar(50), 
	@result int,
	@xmlresponse nvarchar(max), 
	@hostid nvarchar(50), 

	@OutputMsg nvarchar(2000) out,
	@OutputCode int out
AS 
BEGIN 
	DECLARE @rowcnt int
	DECLARE @storedprocedure NVARCHAR(100)

	SET NOCOUNT OFF;
	SET XACT_ABORT ON; 

	SET @OutputCode = 0
	SET @OutputMsg = ''
	SELECT @storedprocedure = OBJECT_NAME(@@PROCID)

	BEGIN TRY
		UPDATE [dbo].[EventsCallControlConnectionAbandoned]
			SET 
				[CI_CallOriginatorType] = @result
			WHERE [UCID]=@UCID

		SET @rowcnt = @@ROWCOUNT

		SET @OutputCode =  @rowcnt

	END TRY

	BEGIN CATCH
		SET @OutputMsg = 'ErrorNum:' + CAST(ERROR_NUMBER() as varchar(10)) + '; ErrorState:' + CAST(ERROR_STATE() as varchar(10)) + '; ' + ERROR_MESSAGE() + ' in line:' + CAST(ERROR_LINE() as varchar(10)) + ' UCID:' + @UCID
		-- Test whether the transaction is uncommittable.
		IF (XACT_STATE()) = -1
		BEGIN
			SET @OutputMsg =  @OutputMsg + ' ROLLBACK TRANSACTION';
			ROLLBACK TRANSACTION;
		END;

		-- Test whether the transaction is active and valid.
		IF (XACT_STATE()) = 1
		BEGIN
			SET @OutputMsg =  @OutputMsg + ' COMMIT TRANSACTION';
			COMMIT TRANSACTION;   
		END;
		SET @OutputCode = -99	-- error occurred

	END CATCH

	INSERT INTO dbo.SmsHistory
	(
		 [UCID]
		,[Sender]
		,[PhoneNumber]
		,[Result]
		,[XmlResponse]
		,[HostId]
		,[ErrorMessage]
		,[StoredProcedure]
		,[DateTime]
	) 
	VALUES
	(
		 @UCID
		,@Sender
		,@phonenumber
		,@result
		,@xmlresponse
		,@hostid
		,@OutputMsg
		,@storedprocedure
		,getdate()
	) 

	RETURN @OutputCode
END
GO


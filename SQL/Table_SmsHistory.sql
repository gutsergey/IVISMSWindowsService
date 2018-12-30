USE [IVIEventsMonitor]
GO

/****** Object:  Table [dbo].[SmsHistory]    Script Date: 10/10/2018 12:52:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SmsHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UCID] [nvarchar](50) NULL,
	[Sender] [nvarchar](50) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[Result] [int] NULL,
	[XmlResponse] [nvarchar](max) NULL,
	[HostId] [nvarchar](50) NULL,
	[ErrorMessage] [nvarchar](2000) NULL,
	[StoredProcedure] [nvarchar](50) NULL,
	[DateTime] [datetime] NULL,
 CONSTRAINT [PK_SmsHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[SmsHistory] ADD  CONSTRAINT [DF_SmsHistory_DateTime]  DEFAULT (getdate()) FOR [DateTime]
GO


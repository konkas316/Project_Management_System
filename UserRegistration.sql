USE [ProjectManagementSystemDB]
GO

/****** Object:  Table [dbo].[UserRegistration]    Script Date: 11/18/2021 5:41:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRegistration](
	[Uid] [int] IDENTITY(1,1) NOT NULL,
	[Uname] [nvarchar](50) NULL,
	[Upwd] [nvarchar](50) NULL,
	[Urepwd] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


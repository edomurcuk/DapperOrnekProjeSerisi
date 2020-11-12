USE [DenemeProjemiz]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 12.11.2020 19:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[NameSurname] [nvarchar](50) NOT NULL,
	[Phone] [nvarchar](15) NULL,
	[CreatedDateUtc] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsersLoginAttemption]    Script Date: 12.11.2020 19:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersLoginAttemption](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[IpAddress] [nvarchar](20) NOT NULL,
	[CreatedDateUtc] [datetime] NOT NULL,
	[IsSuccessLogin] [bit] NOT NULL,
 CONSTRAINT [PK_UsersLoginAttemption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_CreatedDateUtc]  DEFAULT (getutcdate()) FOR [CreatedDateUtc]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UsersLoginAttemption] ADD  CONSTRAINT [DF_UserLoginAttemption_CreatedDateUtc]  DEFAULT (getutcdate()) FOR [CreatedDateUtc]
GO

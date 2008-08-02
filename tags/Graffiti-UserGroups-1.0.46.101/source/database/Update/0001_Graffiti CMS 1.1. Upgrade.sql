/****** Object:  Table [dbo].[graffiti_RolePermissions]    Script Date: 04/25/2008 09:00:00 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_RolePermissions]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_RolePermissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](128)  NOT NULL,
	[HasRead] [bit] NOT NULL CONSTRAINT [DF_graffiti_RolePermissions_HasRead]  DEFAULT ((0)),
	[HasEdit] [bit] NOT NULL CONSTRAINT [DF_graffiti_RolePermissions_HasEdit]  DEFAULT ((0)),
	[HasPublish] [bit] NOT NULL CONSTRAINT [DF_graffiti_RolePermissions_HasPublish]  DEFAULT ((0)),
	[CreatedBy] [nvarchar](128)  NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128)  NULL,
 CONSTRAINT [PK_graffiti_RolePermissions] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_RolePermissions_RoleName] UNIQUE NONCLUSTERED
(
	[RoleName] ASC
)
) ON [PRIMARY]
END


/****** Object:  Table [dbo].[graffiti_RoleCategoryPermissions]    Script Date: 04/25/2008 09:00:00 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'[dbo].[graffiti_RoleCategoryPermissions]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[graffiti_RoleCategoryPermissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](128)  NOT NULL,
	[CategoryId] [int] NOT NULL,
	[HasRead] [bit] NOT NULL CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasRead]  DEFAULT ((0)),
	[HasEdit] [bit] NOT NULL CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasEdit]  DEFAULT ((0)),
	[HasPublish] [bit] NOT NULL CONSTRAINT [DF_graffiti_RoleCategoryPermissions_HasPublish]  DEFAULT ((0)),
	[CreatedBy] [nvarchar](128)  NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128)  NULL,
 CONSTRAINT [PK_graffiti_RoleCategoryPermissions] PRIMARY KEY CLUSTERED
(
	[Id] ASC
),
 CONSTRAINT [IX_graffiti_RoleCategoryPermissions_RoleName] UNIQUE NONCLUSTERED
(
	[RoleName] ASC,
	[CategoryId] ASC
)
) ON [PRIMARY]
END



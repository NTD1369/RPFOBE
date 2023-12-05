USE [RPFO_POS_FARMER_UAT_2022]
GO

/****** Object:  Table [dbo].[M_PlacePrint]    Script Date: 8/8/2023 4:24:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[M_PlacePrint](
	[PrintId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyCode] [nvarchar](300) NOT NULL,
	[StoreId] [nvarchar](50) NOT NULL,
	[PlaceId] [nvarchar](50) NULL,
	[PrintName] [nvarchar](100) NULL,
	[GroupItem] [nvarchar](1000) NULL,
	[Status] [nvarchar](50) NULL,	
	[CustomField1] [nvarchar](500) NULL,
	[CustomField2] [nvarchar](500) NULL,
	[CustomField3] [nvarchar](500) NULL,
	[CustomField4] [nvarchar](500) NULL,
	[CustomField5] [nvarchar](500) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[ModifiedOn] [datetime] NULL
 CONSTRAINT [PK_M_PlacePrint] PRIMARY KEY CLUSTERED 
(
	[PrintId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--------------------------------------------Add Default GeneralSetting--------------------------------------------
INSERT [dbo].[S_DefaultGeneralSetting] ([CompanyCode], [SettingId], [SettingName],[SettingValue], [SettingDescription], [ValueType], [TokenExpired], [DefaultValue], [CustomField1],[CustomField2], [CustomField3], [CustomField4], [CustomField5],[Status], [Priority])
VALUES (N'CP001', N'SettingPrintByDiviceName', N'Setting print by device name', N'SettingPrintByDiviceName', N'Setting print by device name',  N'CheckBox', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'A', NULL)

--------------------------------------------Add M_Function --------------------------------------------
INSERT [dbo].[M_Function] ([FunctionId], [CompanyCode], [Name], [Url], [ParentId], [Icon], [Status], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [LicenseType], [OrderNo], [CustomF1], [CustomF2], [CustomF3], [isShowMenu], [isParent], [MenuOrder], [rowguid])
VALUES (N'Adm_SettingMenu', N'CP001', N'Common Function', N'/admin/masterdata/setting-print', N'Adm_MasterData', N'setting.svg
', N'A', N'System', CAST(N'2021-03-29T22:40:21.520' AS DateTime), NULL, NULL, N'POS', 11, N'Table and Place', NULL, NULL, NULL, NULL, NULL, N'F1B77B1B-6B36-EE11-93AE-00155D542703')

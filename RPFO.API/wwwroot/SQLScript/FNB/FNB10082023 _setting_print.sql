
--------------------------------------------[USP_S_SettingPrint] -------------------------------------------------
USE [RPFO_POS_FARMER_UAT_2022]
GO
/****** Object:  StoredProcedure [dbo].[USP_S_SettingPrint]    Script Date: 8/11/2023 8:04:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- [USP_S_SettingPrint] '1','CP001', 'FM01'
ALTER PROCEDURE [dbo].[USP_S_SettingPrint]
	
	@CompanyCode nvarchar(300),
	@StoreId nvarchar(300)
AS
begin
select *,t2.IGName from M_PlacePrint t1  with (nolock)
left join  [dbo].[M_ItemGroup] t2 with (nolock) on t1.GroupItem = t2.IGId
where t1.CompanyCode = @CompanyCode and t1.StoreId = @StoreId
end 

--------------------------------------------[USP_I_SettingPrint] -------------------------------------------------
USE [RPFO_POS_FARMER_UAT_2022]
GO
/****** Object:  StoredProcedure [dbo].[USP_I_SettingPrint]    Script Date: 8/11/2023 9:26:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[USP_I_SettingPrint]
	-- Add the parameters for the stored procedure here
	@CompanyCode nvarchar(300),
	@StoreId nvarchar(300),
	@PlaceId nvarchar(300) = null,
	@PrintName nvarchar(300),
	@GroupItem nvarchar(300),
	@Status nvarchar(300),
	@CreatedOn DateTime,
	@CreatedBy nvarchar(300)
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[M_PlacePrint]
           (
		 
		   [CompanyCode]
           ,[StoreId]
           ,[PlaceId]
           ,[PrintName]
		   ,[GroupItem]
		   ,[Status]
           ,[CreatedOn]
		   ,[CreatedBy]

           )
     VALUES
           (
		  
		   @CompanyCode,
           @StoreId,
           @PlaceId,
           @PrintName,
		   @GroupItem,
           @Status,
		   @CreatedOn,
           @CreatedBy
           )
END
--------------------------------------------[USP_U_SettingPrint] -------------------------------------------------
USE [RPFO_POS_FARMER_UAT_2022]
GO
/****** Object:  StoredProcedure [dbo].[USP_U_SettingPrint]    Script Date: 8/11/2023 9:28:27 AM ******/


ALTER PROCEDURE [dbo].[USP_U_SettingPrint]
	@PrintId bigint,
	@CompanyCode	nvarchar(50)	,
	@StoreId	nvarchar(250)	,
	@PlaceId	nvarchar(250)	,
	@PrintName	nvarchar(250)	,
	@GroupItem	nvarchar(250)	,
	@Status	nvarchar(1)	,
	@CreatedOn	nvarchar(50)	,
	@ModifiedBy	nvarchar(50)
AS
UPDATE [dbo].[M_PlacePrint]
   SET [CompanyCode] = @CompanyCode
      ,[StoreId] = @StoreId    
      ,[PrintName] = @PrintName
      ,[GroupItem] = @GroupItem
      ,[Status] = @Status
 WHERE PrintId = @PrintId


--------------------------------------------[USP_D_SettingPrint] -------------------------------------------------
USE [RPFO_POS_FARMER_UAT_2022]
GO
/****** Object:  StoredProcedure [dbo].[USP_D_SettingPrint]    Script Date: 8/11/2023 9:28:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[USP_D_SettingPrint]
	@PrintId	bigint
	
	AS	DELETE 
	FROM [dbo].M_PlacePrint
    WHERE PrintId = @PrintId 


--------------------------------------------[USP_S_ItemGroup] -------------------------------------------------
-- [USP_S_ItemGroup] 
ALTER PROCEDURE [dbo].[USP_S_ItemGroup]
	@CompanyCode nvarchar(300),
	@StoreId nvarchar(300)
AS
begin
select IGId , IGName from M_ItemGroup t1 with (nolock)
 join M_Item t2 with (nolock) on t1.CompanyCode = t2.CompanyCode and t2.ItemGroupId = t1.IGId
 join  M_ItemStoreListing t3 with (nolock) on t3.ItemCode = t2.ItemCode and  t3.Status='A' and t3.CompanyCode = @CompanyCode and t3.StoreId = @StoreId
 group by IGId , IGName
end 

------------------------------USP_S_ViewItemByItemGroup------------------------------------------
ALTER PROCEDURE [dbo].[USP_S_ViewItemByItemGroup]
	@ItemGroup nvarchar(300)
AS
begin
select t1.*,t2.ItemName from M_ItemGroup t1
left join M_Item t2 on t2.ItemGroupId = t1.IGId
where t1.IGId = @ItemGroup


end 



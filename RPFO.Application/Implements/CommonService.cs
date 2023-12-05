
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ESCPOS.Commands;
using ESCPOS;
using ESCPOS.Utils;
using Microsoft.Extensions.Configuration;
 
using RPFO.Utilities.Extensions;
using System.IO;
using RPFO.Utilities.Helpers; 
using StackExchange.Redis; 
using System.Drawing.Printing; 
using DevExpress.Pdf;
using RPFO.Utilities.Constants;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace RPFO.Application.Implements
{
    public class CommonService : ICommonService
    {
        private readonly IGenericRepository<MCountry> _commonRepository;
        private readonly string LoopTime = "3";
        private readonly string WaiTime = "1";
        private readonly string Scale57 = "70";
        private readonly string Scale80 = "90";
        private readonly string PrintFolder = @"C:\\RPFO.API.Print";
        private readonly IGeneralSettingService _settingService;
        string redisConnection = "";
        private readonly IMapper _mapper;
        public CommonService(IGenericRepository<MCountry> commonRepository, IConfiguration config, IGeneralSettingService settingService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _commonRepository = commonRepository;
            _settingService = settingService;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            redisConnection =RPFO.Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("Redis"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            LoopTime = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrintLoopTime"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            WaiTime = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrintWaitTime"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            PrintFolder = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrintFolder"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if(LoopTime == null || string.IsNullOrEmpty(LoopTime))
            {
                LoopTime = "3";
            }
            if (WaiTime == null || string.IsNullOrEmpty(WaiTime))
            {
                WaiTime = "1";
            }

            Scale57 = config.GetConnectionString("PrintSize57Scale");
            Scale80 = config.GetConnectionString("PrintSize80Scale");
            if (Scale57 == null || string.IsNullOrEmpty(Scale57))
            {
                Scale57 = "70";
            }
            if (Scale80 == null || string.IsNullOrEmpty(Scale80))
            {
                Scale80 = "90";
            }
            if (PrintFolder == null || string.IsNullOrEmpty(PrintFolder))
            {
                PrintFolder = @"C:\\RPFO.API.Print";
            }
            initService();
            InitFirstService();
        }
        public class ObjectTableInfor
        {
            public string ColumnName { get; set;}
            public string ColumnId { get; set;}
            public string ColumnType { get; set;}
            public string ColumnLength { get; set;}
            public string NullableSign { get; set;}
       
        }
        public GenericResult initService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string queryCheckAndCreate = "IF (OBJECT_ID('USP_RPT_SyncDataStatusView_New') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                        " set @string = 'create PROCEDURE [dbo].[USP_RPT_SyncDataStatusView_New] @FromDate datetime, @ToDate datetime,@StoreId nvarchar(4000) = null AS " +
                        " begin SET NOCOUNT ON;" +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_Item'') IS NOT NULL DROP Table #TBL_RAW_Item " +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_Price'') IS NOT NULL DROP Table #TBL_RAW_Price " +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_Promo'') IS NOT NULL DROP Table #TBL_RAW_Promo " +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_SUM'') IS NOT NULL DROP Table #TBL_RAW_SUM " +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_SUM1'') IS NOT NULL DROP Table #TBL_RAW_SUM1 " +
                        " DECLARE @DynamicPivotQuery AS NVARCHAR(MAX) DECLARE @ColumnName AS NVARCHAR(MAX) " +
                        //" -------- Master data" +
                        " SELECT DISTINCT T.StoreId , COUNT(I.ItemCode) CountItem, CONVERT(nvarchar, I.ModifiedOn, 112) ModifiedOn, ''MasterData'' AS ''DataType'' INTO #TBL_RAW_Item " +
                        " FROM M_Item I LEFT JOIN T_SyncActionData T ON I.StatusGuid = T.StatusGuid AND T.SyncStatus = ''Y'' " +
                        " WHERE T.TableName = ''M_Item'' AND (CONVERT(nvarchar, I.ModifiedOn, 112) <= CONVERT(nvarchar, @ToDate, 112) AND CONVERT(nvarchar, I.ModifiedOn, 112) >= CONVERT(nvarchar, @FromDate, 112))" +
                        " Group by CONVERT(nvarchar, I.ModifiedOn, 112), T.StoreId" +
                        " Order by CONVERT(nvarchar, I.ModifiedOn, 112) DESC, T.StoreId" +
                        " IF ISNULL(@StoreId, '''') = '''' " +
                        " BEGIN" +
                        " SELECT @ColumnName = ISNULL(@ColumnName + '','', '''')  + QUOTENAME(StoreId) FROM (SELECT DISTINCT StoreId FROM #TBL_RAW_Item) AS TBLRAW ORDER BY StoreId" +
                        " end " +
                        " ELSE BEGIN" +
                        " SELECT @ColumnName = ISNULL(@ColumnName + '','', '''')  + QUOTENAME(TBLRAW.StoreId) FROM (SELECT DISTINCT StoreId FROM #TBL_RAW_Item) AS TBLRAW " +
                        " INNER JOIN (SELECT VALUE StoreId from string_split(@StoreId, '';'')) S ON TBLRAW.StoreId = S.StoreId ORDER BY TBLRAW.StoreId " +
                        " END " +
                        //" --Prepare the PIVOT query using the dynamic " +
                        " SET @DynamicPivotQuery =  N''SELECT DataType, ModifiedOn, '' + @ColumnName + ''  INTO dbo.TempTable FROM #TBL_RAW_Item PIVOT(SUM(CountItem)  FOR StoreId IN ('' + @ColumnName + '')) AS PVTTable''" +
                        //" --Execute the Dynamic Pivot Query " +
                        " EXEC sp_executesql @DynamicPivotQuery " +
                        " SELECT * INTO #TBL_RAW_SUM FROM dbo.TempTable; DROP TABLE dbo.TempTable; " +
                        //" -------- SellingPrice" +
                        " SELECT DISTINCT T.StoreId , COUNT(I.ItemCode) CountItem, CONVERT(nvarchar, I.ModifiedOn, 112) ModifiedOn, ''SellingPrice'' AS ''DataType'' INTO #TBL_RAW_Price " +
                        " FROM M_PriceList I  LEFT JOIN T_SyncActionData T ON I.StatusGuid = T.StatusGuid AND I.StoreId = T.StoreId AND T.SyncStatus = ''Y'' " +
                        " WHERE T.TableName = ''M_PriceList'' AND (CONVERT(nvarchar, I.ModifiedOn, 112) <= CONVERT(nvarchar, @ToDate, 112) AND CONVERT(nvarchar, I.ModifiedOn, 112) >= CONVERT(nvarchar, @FromDate, 112))" +
                        " Group by CONVERT(nvarchar, I.ModifiedOn, 112), T.StoreId  Order by CONVERT(nvarchar, I.ModifiedOn, 112) DESC, T.StoreId" +
                        //" --Prepare the PIVOT query using the dynamic " +
                        " SET @DynamicPivotQuery =  N''SELECT DataType, ModifiedOn, '' + @ColumnName + ''  INTO dbo.TempTable FROM #TBL_RAW_Price PIVOT(SUM(CountItem)  FOR StoreId IN ('' + @ColumnName + '')) AS PVTTable''" +
                        //" --Execute the Dynamic Pivot Query" +
                        " EXEC sp_executesql @DynamicPivotQuery" +
                        " SELECT * INTO #TBL_RAW_SUM1 FROM dbo.TempTable; DROP TABLE dbo.TempTable;" +
                        //" -------- Promotions" +
                        " SELECT DISTINCT T.StoreId , COUNT(I.PromoId) CountItem, CONVERT(nvarchar, I.ModifiedOn, 112) ModifiedOn, ''Promotions'' AS ''DataType'' INTO #TBL_RAW_Promo" +
                        " FROM S_PromoHeader I  LEFT JOIN T_SyncActionData T ON I.StatusGuid = T.StatusGuid AND T.SyncStatus = ''Y''" +
                        " WHERE T.TableName = ''S_PromoHeader'' AND (CONVERT(nvarchar, I.ModifiedOn, 112) <= CONVERT(nvarchar, @ToDate, 112) AND CONVERT(nvarchar, I.ModifiedOn, 112) >= CONVERT(nvarchar, @FromDate, 112))" +
                        " Group by CONVERT(nvarchar, I.ModifiedOn, 112), T.StoreId Order by CONVERT(nvarchar, I.ModifiedOn, 112) DESC, T.StoreId " +
                        //" --Prepare the PIVOT query using the dynamic " +
                        " SET @DynamicPivotQuery =  N''SELECT DataType, ModifiedOn, '' + @ColumnName + ''  INTO dbo.TempTable FROM #TBL_RAW_Promo PIVOT(SUM(CountItem)  FOR StoreId IN ('' + @ColumnName + '')) AS PVTTable''" +
                        //" --Execute the Dynamic Pivot Query" +
                        " EXEC sp_executesql @DynamicPivotQuery " +
                        " select ''DataType'' controlId, ''Data Type'' controlName,1 orderNum,''string'' custom2, ''1'' groupNum , null groupItem  union all" +
                        " select ''ModifiedOn'' controlId, ''Modified On'' controlName,2 orderNum,''string'' custom2, ''0'' groupNum, null groupItem  union all " +
                        " SELECT SUBSTRING(value, 2 ,4) as controlId, SUBSTRING(value, 2 ,4) as controlName,( ROW_NUMBER() OVER(ORDER BY SUBSTRING(value, 2 ,4) ASC) + 2) AS  orderNum, ''number'' custom2, null groupNum , null groupItem  FROM STRING_SPLIT(@ColumnName, '',''); " +
                        " SELECT * FROM #TBL_RAW_SUM UNION ALL  SELECT * FROM #TBL_RAW_SUM1 UNION ALL  SELECT * FROM dbo.TempTable ORDER BY ModifiedOn DESC " +
                        " DROP TABLE dbo.TempTable;" +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_Item'') IS NOT NULL  DROP Table #TBL_RAW_Item " +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_Price'') IS NOT NULL  DROP Table #TBL_RAW_Price" +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_Promo'') IS NOT NULL  DROP Table #TBL_RAW_Promo " +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_SUM'') IS NOT NULL  DROP Table #TBL_RAW_SUM" +
                        " IF OBJECT_ID(''tempdb..#TBL_RAW_SUM1'') IS NOT NULL  DROP Table #TBL_RAW_SUM1 " +
                        " end '; " +
                        " EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreate);



                    db.Close();
                    result.Success = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    return result;

                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }


        }

        //public GenericResult GetGeneralSetting(string CompanyCode, string StoreId)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);
        //        //List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
        //        //if (settingData.Success)
        //        //{

        //        //    SettingList = settingData.Data as List<GeneralSettingStore>;

        //        //}
        //        result.Success = true;
        //        result.Data = SettingList;
        //    }
        //    catch(Exception Ex)
        //    {

        //    }
        //    return result;
        //}


        public DataTable CreaDataTable(string TableName)
        {
            
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string getTable = $"select * from dbo.[fn_GetTableInfor]('{TableName}' , '1')";
                    var items = db.Query<ObjectTableInfor>(getTable, null, commandType: CommandType.Text, commandTimeout: 3600);

                    var GILines = new DataTable(TableName + "TableType");
                    foreach (var item in items)
                    {
                        Type type = typeof(string);
                        if (item.ColumnType.ToLower() == "string")  type = typeof(string); 
                        if (item.ColumnType.ToLower() == "long")  type = typeof(long); 
                        if (item.ColumnType.ToLower() == "byte[]")  type = typeof(byte[]); 
                        if (item.ColumnType.ToLower() == "bool")  type = typeof(bool); 
                        if (item.ColumnType.ToLower() == "datetime")  type = typeof(DateTime); 
                        if (item.ColumnType.ToLower() == "datetimeoffset")  type = typeof(DateTimeOffset); 
                        if (item.ColumnType.ToLower() == "decimal")  type = typeof(decimal); 
                        if (item.ColumnType.ToLower() == "double")  type = typeof(double); 
                        if (item.ColumnType.ToLower() == "int")  type = typeof(int); 
                        if (item.ColumnType.ToLower() == "float")  type = typeof(float); 
                        if (item.ColumnType.ToLower() == "short")  type = typeof(short); 
                        if (item.ColumnType.ToLower() == "timespan")  type = typeof(TimeSpan); 
                        if (item.ColumnType.ToLower() == "byte")  type = typeof(byte); 
                        if (item.ColumnType.ToLower() == "guid")  type = typeof(Guid);  
                        
                        GILines.Columns.Add(item.ColumnName, type);
                    }
                    return GILines;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return null;

        }
        public GenericResult InitFirstService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string queryCheckfn_GetTableInfor = $" IF NOT EXISTS (SELECT *  FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[fn_GetTableInfor]') AND type IN(N'FN', N'IF', N'TF', N'FS', N'FT')) " +
                        $" begin declare @string nvarchar(MAX) = ''; " +
                        $" set @string = 'create FUNCTION [dbo].[fn_GetTableInfor]    ( @TableName nvarchar(250), @ConvertToC bit ) RETURNS TABLE AS Return    select  replace(col.name, '' '', ''_'') ColumnName,  column_id ColumnId, case @ConvertToC when 1 " +
                        $" then case typ.name  when ''bigint'' then ''long'' when ''binary'' then ''byte[]'' when ''bit'' then ''bool''" +
                        $" when ''char'' then ''string'' when ''date'' then ''DateTime'' when ''datetime'' then ''DateTime'' when ''datetime2'' then ''DateTime'' when ''datetimeoffset'' then ''DateTimeOffset''" +
                        $" when ''decimal'' then ''decimal'' when ''float'' then ''double'' when ''image'' then ''byte[]'' when ''int'' then ''int'' when ''money'' then ''decimal'' when ''nchar'' then ''string''" +
                        $" when ''ntext'' then ''string'' when ''numeric'' then ''decimal'' when ''nvarchar'' then ''string'' when ''real'' then ''float'' when ''smalldatetime'' then ''DateTime'' when ''smallint'' then ''short'' when ''smallmoney'' then ''decimal'' " +
                        $" when ''text'' then ''string'' when ''time'' then ''TimeSpan'' when ''timestamp'' then ''long'' when ''tinyint'' then ''byte'' when ''uniqueidentifier'' then ''Guid'' when ''varbinary'' then ''byte[]''" +
                        $" when ''varchar'' then ''string'' else ''UNKNOWN_'' + typ.name end else  typ.name   end ColumnType," +
                        $" case typ.name   when ''nvarchar''   then ''('' +   case when  COL_LENGTH(OBJECT_NAME(object_id), col.name) = -1  then ''MAX'' else CONVERT(nvarchar(50) ,  COL_LENGTH(OBJECT_NAME(object_id), col.name)  ) end+ '')''" +
                        $" when ''varchar'' then  ''('' + CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')''  when ''nchar'' then  ''('' + CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')''" +
                        $" when ''char'' then  ''('' +CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')'' when ''text'' then   ''('' + CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name))+ '')''" +
                        $" when ''text'' then ''('' +CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')'' when ''decimal'' then ''(19, 6)'' " +
                        $" end ColumnLength, case  when col.is_nullable = 1 " +
                        //and typ.name in (''bigint'', ''bit'', ''date'', ''datetime'', ''datetime2'', ''datetimeoffset'', ''decimal'', ''float'', ''int'', ''money'', ''numeric'', ''real'', ''smalldatetime'', ''smallint'', ''smallmoney'', ''time'', ''tinyint'', ''uniqueidentifier'') 
                        $" then ''?''  else ''''  end NullableSign from sys.columns col join sys.types typ on  col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id   where object_id = object_id(@TableName)" +
                        " '; " +
                        " EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckfn_GetTableInfor);
 

                    string queryCheckAndCreateStoreCreateTableType = "IF (OBJECT_ID('USP_CheckAndCreateTableType') IS NULL)  begin declare @string nvarchar(MAX) = ''; " +
                        " set @string = 'create PROCEDURE [dbo].[USP_CheckAndCreateTableType] @TableName nvarchar(250), @IsDropAndCreate bit AS  " +
                        "begin   if ((select count(*) from dbo.[fn_GetTableInfor] ( @TableName , 0)) <> 0)   " +
                        "begin declare @Result varchar(max)  = ''''; declare @NumOfTable int; " +
                        "declare @NumOfTableType int; declare @NamTableType varchar(450)  = ''''; set @NamTableType =  QUOTENAME(@TableName + ''TableType''); set 	@TableName = QUOTENAME(@TableName)  " +
                        "IF EXISTS (SELECT * FROM   [sys].[table_types]  WHERE  user_type_id = Type_id(@NamTableType))  BEGIN if (@IsDropAndCreate = 1) begin set @NumOfTable = (select count(column_name) as Number " +
                        "from information_schema.columns with (nolock)  where table_name= @TableName) set @NumOfTableType = ( Select COUNT( c.name ) as Number From sys.table_types t  with (nolock)  Inner join sys.columns c with (nolock) on c.object_id = t.type_table_object_id WHERE t.is_user_defined = 1 AND t.is_table_type = 1 and t.name = @NamTableType)  if(@NamTableType <> @NumOfTable) begin   declare @Drop  varchar(max);  set @Drop = N''DROP TYPE '' + @NamTableType + '' ;''; set @Result = @Drop + '' CREATE TYPE dbo.'' + @NamTableType + '' AS TABLE('' select @Result = @Result + '''' + QUOTENAME(ColumnName) +   '' '' +  QUOTENAME( ColumnType ) +   ''''+ isnull(ColumnLength ,'''')  +  '' '' + case  when   NullableSign = ''?'' then ''null'' else ''not null'' end + '','' from ( select * from dbo.[fn_GetTableInfor] ( @TableName , 0) ) t order by ColumnId set @Result = (SELECT LEFT(@Result, NULLIF(LEN(@Result)-1,-1)))   + '')''				select  @Result end else begin select  ''-1'' end  end  else begin select ''-1'' end END  else begin set @Result  = N''CREATE TYPE dbo.'' + @NamTableType + '' AS TABLE( ''  select @Result = @Result + '''' + QUOTENAME(ColumnName) +   '' '' +  QUOTENAME( ColumnType ) +   ''''+ isnull(ColumnLength ,'''')  +  '' '' + case  when   NullableSign = ''?'' then ''null'' else ''not null'' end + '','' from ( select * from dbo.[fn_GetTableInfor] ( @TableName , 0) ) t order by ColumnId set @Result =  (SELECT LEFT(@Result, NULLIF(LEN(@Result)-1,-1)))   + '')''  set @Result = N'''' + @Result +''''; select  @Result  end end else begin select  ''-1'' end end'; EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreateStoreCreateTableType);

 


                    result.Success = true;

                    //tran.Commit();

                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return result;



        }
        public GenericResult FirstInitDB(ObjectInitNewData model ,string customStr)
        {
            GenericResult result = new GenericResult();
            string StepMessage = "";
            using (IDbConnection db = _commonRepository.GetConnectionCustom(customStr))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //Flag = "Connect Database";
                    //result.Success = true;
                    //result.Message = model.DatabaseName + " has been created and connect successfully completed.";

                    //Flag = "Init Store Data";
                    string Folder = Path.Combine(
                              Directory.GetCurrentDirectory(),
                              "wwwroot", "initClientQuery");
                    string folderPath = Folder;
                    int step = 1;
                    string declareStr = $"use {model.DatabaseName}; ";
                    declareStr += Environment.NewLine;
                    declareStr += $" Go ";
                    declareStr += Environment.NewLine;
                    declareStr += $" declare @StoreId nvarchar(250), @StoreName nvarchar(500), @StoreAddress nvarchar(1000); ";
                    declareStr += Environment.NewLine;
                    declareStr += $" set @StoreId = '{model.StoreId}'; ";
                    declareStr += Environment.NewLine;
                    declareStr += $" set @StoreName = '{model.StoreName}'; ";
                    declareStr += Environment.NewLine;
                    declareStr += $" set @StoreAddress = N'{model.StoreAddress}'; ";
                    declareStr += Environment.NewLine;
                    declareStr += $" Go ";
                    declareStr += Environment.NewLine;
                    foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt"))
                    { 
                        string contents = File.ReadAllText(file);
                        if (!string.IsNullOrEmpty(contents))
                        {

                            string strX = declareStr + contents;
                            //string sqlText;
                            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FOO;Integrated Security=True;";


                            SqlConnection sqlConnection = new SqlConnection(customStr);
                            ServerConnection svrConnection = new ServerConnection(sqlConnection);
                            Server server = new Server(svrConnection);
                            server.ConnectionContext.ExecuteNonQuery(strX);
                            //strX = strX.Replace("Go", @"\r\n");
                            //strX = strX.Replace("GO", @"\r\n");
                            //strX = strX.Replace("go", @"\r\n");
                            //strX = strX.Replace("gO", @"\r\n");
                            //db.Execute(strX, commandTimeout: 3600);
                            StepMessage += step + ",";
                        } 
                        step++;
                    }
                    result.Message += " - " + StepMessage.ToString();
                    result.Success = true;
                    return result;
                }
                catch (System.Exception ex)
                {
                    result.Success = false;
                    result.Message = StepMessage + " ~ " + ex.Message;
                    return result;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                    {
                        db.Close();
                    }
                }
            }

        }    
        public GenericResult InitDb(ObjectInitNewData model)
        {
            GenericResult result = new GenericResult();
            if(string.IsNullOrEmpty(model.ServerName))
            {
                result.Success =false;
                result.Message = "Please Input Database Server Name";
                return result;
            }
            if (string.IsNullOrEmpty(model.DatabaseName))
            {
                result.Success = false;
                result.Message = "Please Input Database Name";
                return result;
            }
            if (string.IsNullOrEmpty(model.DBUser))
            {
                result.Success = false;
                result.Message = "Please Input Database User";
                return result;
            }
            if (string.IsNullOrEmpty(model.DBPassword))
            {
                result.Success = false;
                result.Message = "Please Input Database Password";
                return result;
            }
            if (string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = "Please Input Store Id";
                return result;
            }
            if (string.IsNullOrEmpty(model.StoreName))
            {
                result.Success = false;
                result.Message = "Please Input Store Name";
                return result;
            }
            if (string.IsNullOrEmpty(model.StoreAddress))
            {
                result.Success = false;
                result.Message = "Please Input Store Address";
                return result;
            }
            string customStr = $"Server={model.ServerName};Database={model.DatabaseName};user id={model.DBUser};password={model.DBPassword};MultipleActiveResultSets=true";
            SqlConnection myConn = new SqlConnection($"Server={model.ServerName};user id={model.DBUser};password={model.DBPassword};MultipleActiveResultSets=true;database=master");

            //string str = $"CREATE DATABASE {model.DatabaseName} ON PRIMARY " +
            // "(NAME = MyDatabase_Data, " +
            // "FILENAME = 'C:\\MyDatabaseData.mdf', " +
            // "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%)" +
            // "LOG ON (NAME = MyDatabase_Log, " +
            // "FILENAME = 'C:\\MyDatabaseLog.ldf', " +
            // "SIZE = 1MB, " +
            // "MAXSIZE = 5MB, " +
            // "FILEGROWTH = 10%)";
            //If(db_id(N'{model.DatabaseName}') IS NULL)
            if(model.IsServer!=null && model.IsServer == true)
            {
                string StepMessage = "";
                using (IDbConnection db = _commonRepository.GetConnectionCustom(customStr))
                {
                    try
                    {
                        
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Flag = "Connect Database";
                        //result.Success = true;
                        //result.Message = model.DatabaseName + " has been created and connect successfully completed.";

                        //Flag = "Init Store Data";
                        string Folder = Path.Combine(
                                  Directory.GetCurrentDirectory(),
                                  "wwwroot", "initServerQuery");
                        string folderPath = Folder;
                        int step = 1;
                        string declareStr = $"use {model.DatabaseName}; ";
                        declareStr += Environment.NewLine;
                        
                        declareStr += $" declare @StoreId nvarchar(250), @StoreName nvarchar(500), @StoreAddress nvarchar(1000); ";
                        declareStr += Environment.NewLine;
                        declareStr += $" set @StoreId = '{model.StoreId}'; ";
                        declareStr += Environment.NewLine;
                        declareStr += $" set @StoreName = '{model.StoreName}'; ";
                        declareStr += Environment.NewLine;
                        declareStr += $" set @StoreAddress = N'{model.StoreAddress}'; ";
                        declareStr += Environment.NewLine;
                       
                        foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt"))
                        {
                            string contents = File.ReadAllText(file);
                            if (!string.IsNullOrEmpty(contents))
                            {

                                string strX = declareStr + contents;
                                //string sqlText;
                                //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FOO;Integrated Security=True;";


                                SqlConnection sqlConnection = new SqlConnection(customStr);
                                ServerConnection svrConnection = new ServerConnection(sqlConnection);
                                Server server = new Server(svrConnection);
                                server.ConnectionContext.ExecuteNonQuery(strX);
                                //strX = strX.Replace("Go", @"\r\n");
                                //strX = strX.Replace("GO", @"\r\n");
                                //strX = strX.Replace("go", @"\r\n");
                                //strX = strX.Replace("gO", @"\r\n");
                                //db.Execute(strX, commandTimeout: 3600);
                                StepMessage += step + ",";
                            }
                            step++;
                        }
                        result.Message = StepMessage + " successfully completed";
                        result.Success = true;
                      
                    }
                    catch (System.Exception ex)
                    {
                        result.Success = false;
                        result.Message = StepMessage + " ~ " + ex.Message;
                         
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                        {
                            db.Close();
                        }
                    }
                }
                return result;
            }
            else
            {
                string stringCreateDb = $" CREATE DATABASE [{model.DatabaseName}]";
                SqlCommand myCommand = new SqlCommand(stringCreateDb, myConn);
                string Flag = "";
                try
                {
                    myConn.Open();
                    myCommand.ExecuteNonQuery();
                    result.Success = true;
                    result.Message = model.DatabaseName + " has been created.";
                    Flag = "Create database";
                    using (IDbConnection db = _commonRepository.GetConnectionCustom(customStr))
                    {
                        try
                        {
                            if (db.State == ConnectionState.Closed)
                                db.Open();
                            Flag = "Connect Database";
                            result.Success = true;
                            result.Message = model.DatabaseName + " has been created and connect successfully completed.";

                            Flag = "Init Store Data";
                            var resultInit = FirstInitDB(model, customStr);
                            resultInit.Message = result.Message + resultInit.Message;
                            result = resultInit;
                        }
                        catch (System.Exception ex)
                        {
                            result.Success = false;
                            result.Message = model.DatabaseName + " has been created. Connect Message:" + ex.Message;
                            
                        }
                        finally
                        {
                            if (db.State == ConnectionState.Open)
                            {
                                db.Close();
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    result.Success = false;
                    result.Message = "Create database " + ex.Message;
                   
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
                return result;
            }
            


        }
        public GenericResult GetLicenseInfor(string CompanyCode, string License)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("KeyId", string.IsNullOrEmpty(License) ? "" : License);

                    var item = db.Query<SLicense>("USP_GetLicenseInfor", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600).FirstOrDefault();
                    if(item!=null)
                    {
                        LicenseInfor data = new LicenseInfor();
                        string jsonStr = Encryptor.DecryptString(item.Token, AppConstants.TEXT_PHRASE);

                        data= JsonConvert.DeserializeObject<LicenseInfor>(jsonStr);

                        result.Success = true;
                        result.Data = data;
                    }    
                    else
                    {
                        result.Success = false;
                        result.Message = "Data not found";
                    }    
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            return result;
        }
        public GenericResult InitService(string ServiceName, List<string> TableNameList)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                     
                    string encryptName = Utilities.Helpers.Encryptor.EncryptString(ServiceName, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    // Create TableType
                  

                    foreach (string table in TableNameList)
                    {
                        string query = $"USP_CheckAndCreateTableType N'{table}', N'0'";
                        string queryData = _commonRepository.GetScalar(query, null, commandType: CommandType.Text);
                        if (!string.IsNullOrEmpty(queryData) && queryData != "-1")
                        {
                            _commonRepository.Execute(queryData, null, commandType: CommandType.Text);
                        }
                    }

                    // Create Store if not existed
                    string storeQuery = "";
                    string queryGetdata = $" IF EXISTS ( SELECT *   FROM INFORMATION_SCHEMA.TABLES   WHERE TABLE_SCHEMA = N'dbo'   AND  TABLE_NAME = N'S_AC_StoreProcedure' ) begin SELECT * FROM  [dbo].[S_AC_StoreProcedure] with (nolock) where FunctionService = N'{encryptName.Replace("'", "''")}' end";
                    var storeList = db.Query<SACStoreProcedure>(queryGetdata, null, commandType: CommandType.Text, commandTimeout: 3600);
                    if (storeList != null && storeList.Count() > 0)
                    {
                        foreach (SACStoreProcedure storeProcedure in storeList)
                        {

                            string checkStoreExisted = $"IF EXISTS ( SELECT *  FROM sysobjects  with (nolock) WHERE id = object_id(N'[dbo].[{storeProcedure.NameOfStore}]') and OBJECTPROPERTY(id, N'IsProcedure') = 1 ) BEGIN select '1' END else begin select '-1' end";
                            string queryCheck = _commonRepository.GetScalar(checkStoreExisted, null, commandType: CommandType.Text);
                            if (!string.IsNullOrEmpty(queryCheck) && queryCheck != "1")
                            {  
                                string content1 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content1, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content2 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content2, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content3 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content3, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content4 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content4, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content5 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content5, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content6 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content6, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content7 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content7, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content8 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content8, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content9 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content9, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                string content10 = Utilities.Helpers.Encryptor.DecryptString(storeProcedure.Content10, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);

                                if (!string.IsNullOrEmpty(content1)) storeQuery += content1;
                                if (!string.IsNullOrEmpty(content2)) storeQuery += content2;
                                if (!string.IsNullOrEmpty(content3)) storeQuery += content3;
                                if (!string.IsNullOrEmpty(content4)) storeQuery += content4;
                                if (!string.IsNullOrEmpty(content5)) storeQuery += content5;
                                if (!string.IsNullOrEmpty(content6)) storeQuery += content6;
                                if (!string.IsNullOrEmpty(content7)) storeQuery += content7;
                                if (!string.IsNullOrEmpty(content8)) storeQuery += content8;
                                if (!string.IsNullOrEmpty(content9)) storeQuery += content9;
                                if (!string.IsNullOrEmpty(content10)) storeQuery += content10;

                                _commonRepository.Execute(storeQuery, null, commandType: CommandType.Text);
                            }


                        }
                    }
                    result.Success = true;

                    //tran.Commit();

                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return result;


          
        }

        public async Task<GenericResult> GetCountries(string Area)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from  M_Country with (nolock) where 1=1 ";
                    if (!string.IsNullOrEmpty(Area))
                    {
                        query += $" and UserName ='{Area}'";
                    }
                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

        public async Task<GenericResult> GetQuery(QueryModel model)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = "";
                    if (!string.IsNullOrEmpty(model.QueryExcute) )
                    { 
                        query = model.QueryExcute; 
                    }   
                    else
                    {
                        string EncQuery = Utilities.Helpers.Encryptor.DecryptString(model.Query, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                        if(model.ParramObjects!=null && model.ParramObjects.Count > 0)
                        {
                            foreach (var parram in model.ParramObjects)
                            {
                                EncQuery = EncQuery.Replace("{{" +parram.Id+ "}}",  parram.Value);
                            }
                        }    
                        query = EncQuery;
                      
                    }
                    var dataX = await db.QueryAsync(query, null);

                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }


        public async Task<GenericResult> GetMaxValueCurrency(string Currency)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $" select MaxValue from M_Currency with (nolock) where CurrencyCode ='{Currency}' ";

                    var dataX = await db.ExecuteScalarAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

        public async Task<GenericResult> GetArea()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from  M_Area with (nolock) where 1=1 ";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public async Task<GenericResult> GetProvinceList()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from  M_Province with (nolock) where 1=1 ";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
       
       
        public async Task<GenericResult> GetCurrencyList()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from  M_Currency with (nolock) where 1=1 ";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public GenericResult ClearRedisCache(string Key, string Prefix)
        {
            GenericResult result = new GenericResult();
            try
            {
                if(!string.IsNullOrEmpty(redisConnection))
                {
                    var options = ConfigurationOptions.Parse(redisConnection, true);
                    options.AllowAdmin = true;
                    var redis = ConnectionMultiplexer.Connect(options);

                    var endpoints = redis.GetEndPoints();
                    var server = redis.GetServer(endpoints[0]);
                    result.Success = true;
                    if (string.IsNullOrEmpty(Key) && string.IsNullOrEmpty(Prefix))
                    {
                        server.FlushAllDatabases();
                        result.Message = "Clear all cache completed";
                    }   
                    else
                    {
                        string mess = "";
                        if(!string.IsNullOrEmpty(Prefix))
                        {
                              
                            var _cache = redis.GetDatabase();
                            var keys = server.Keys();
                            bool delKey = false; 
                            foreach (var key in keys)
                            {
                                if (Prefix.ToLower() == "item")
                                { 
                                    if (key.ToString().Length >= Prefix.Length && key.ToString().Contains("/api/item"))
                                    {
                                        delKey = true;
                                        _cache.KeyDelete(key);
                                    }
                                }
                                else
                                {
                                    if (key.ToString().Length >= Prefix.Length && key.ToString().Substring(0, Prefix.Length).ToLower() == Prefix.ToLower())
                                    {
                                        delKey = true;
                                        _cache.KeyDelete(key);
                                    }
                                }
                               
                            }
                            if (delKey)
                            {
                                result.Success = true;
                                mess = "Clear cache with prefix: " + Prefix + " completed.";
                            }
                            else
                            {
                                result.Success = false;
                                mess = "Can't get cache prefix: " + Prefix;
                            }
                        }
                        if (!string.IsNullOrEmpty(Key))
                        {
                            if (!string.IsNullOrEmpty(mess))
                            {
                                mess +=  " <br />";
                            }
                            var _cache = redis.GetDatabase();
                            var keys = server.Keys();
                            bool delKey = false;
                            foreach (var key in keys)
                            { 
                                if(key.ToString().ToLower() == Key.ToLower())
                                {
                                    delKey = true;
                                    _cache.KeyDelete(key);
                                }    
                            }
                            if(delKey)
                            {
                                result.Success = true;
                                mess +=  " Clear cache with key: " + Key + " completed.";
                            }    
                            else
                            {
                                result.Success = false;
                                mess +=  " Can't get cache key: " + Key;
                            }    
                        }
                        result.Message = mess;
                    }    
                   
                   
                }   
                else
                {
                    result.Success = false;
                    result.Message = "Can't get cache connection";
                }    
               
               
            }
            catch (Exception ex)
            { 
                result.Success = false;
                result.Message = ex.Message;
                //throw ex;
            }
          
            return result;
             
        }

        public async Task<GenericResult> GetRegion()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from  M_Region with (nolock) where 1=1 ";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public class FieldResult 
        {
            public string Field { get; set; }
            public List<string> Values { get; set; } = new List<string>();
        }
        public async Task<GenericResult> GetItemCustomList(string Field)
        {
            GenericResult result = new GenericResult();
            
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"[dbo].[USP_U_S_ItemCustomList] '{Field}'";
                    var dataX = await db.QueryAsync(query, null); 
                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public async Task<GenericResult> GetPOSType()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"[USP_S_POSOptionType]";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public async Task<GenericResult> GetPOSOption(string Type)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"USP_S_POSOption '{Type}'";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

      

        public async Task<GenericResult> GetPOSVersion(string CompanyCode, string Version)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"USP_S_S_Version '{CompanyCode}', '{Version}'";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public  GenericResult  OpenDrawerCashTry(string Name)
        {
            GenericResult result = new GenericResult(); 
            try
            {
                byte[] openCash = OpenDrawer;
                openCash.Print(Name);

                result.Success = true;
                result.Message = "";
            }
            catch (Exception ex)
            { 
                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }

        public GenericResult PageCutTry(bool? IsFull, string Name)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (IsFull == true)
                {
                    byte[] openCash = FullPaperCut;
                    openCash.Print(Name);
                }
                else
                {
                    byte[] openCash = PaperCut; 
                    openCash.Print(Name);
                }


                result.Success = true;
                result.Message = "";
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }
        public void WriteLog(GenericResult result, string BillNo, string Name)
        {
            result.Data = Name;
            if (string.IsNullOrEmpty(BillNo))
            {
                BillNo = "Undefined";
            }
            string NameX = BillNo +"_"+ result.Success.ToString();
            try
            {
               
                string Folder = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot", "OpenDrawerLogs");
                if (!Directory.Exists(Folder))
                    Directory.CreateDirectory(Folder);

                var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/OpenDrawerLogs", "");
               
                LogUtils.WriteLogData(path, "", NameX, result.ToJson());
                //RPFO.Utilities.Helpers.LogUtils.WriteLogData(path, "",, result.ToJson());
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
                string Folder = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot", "OpenDrawerLogs");
                if (!Directory.Exists(Folder))
                    Directory.CreateDirectory(Folder);

                var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/OpenDrawerLogs", "");

                LogUtils.WriteLogData(path, "", NameX, result.ToJson());
            }
         
         

        }
        public async Task<GenericResult> OpenDrawerCash(string Name, string BillNo)
        {
            GenericResult result = new GenericResult(); 
            try
            {
                int maxTry = int.Parse(LoopTime); 
                for(int i= 1; i<= maxTry; i++)
                {
                    //await Task.Delay(int.Parse(WaiTime) * 1000);
                    result = OpenDrawerCashTry(Name); 
                    if (result.Success==true)
                    {
                        WriteLog(result, BillNo, Name);
                        return result;
                    }    
                    else
                    { 
                        if ( i>= maxTry)
                        {
                            return result;
                        }
                        await Task.Delay(int.Parse(WaiTime) * 1000);
                    }     
                }    
                
                result.Success = true;
                result.Message = "";
            }
            catch (Exception ex)
            {
                 
                result.Success = false;
                result.Message = ex.Message;
                 
            }
            WriteLog(result, BillNo, Name);
            return result;
        }
        public async Task<GenericResult> PageCut(bool? IsFull, string Name)
        {
            GenericResult result = new GenericResult();
            try
            {
                //if(IsFull == true)
                //{
                //    byte[] openCash = FullPaperCut ;
                //    openCash.Print(Name);
                //}
                //else
                //{
                //    byte[] openCash = PaperCut;
                //    openCash.Print(Name);
                //}
                int maxTry = int.Parse(LoopTime);
                for (int i = 1; i <= maxTry; i++)
                {
                    result = PageCutTry(IsFull, Name);
                    if (result.Success == true)
                    { 
                        return result;
                    }
                    else
                    {
                        if (i >= maxTry)
                        {
                            return result;
                        }
                        await Task.Delay(int.Parse(WaiTime) * 1000);
                    }
                }

                result.Success = true;
                result.Message = "";
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }

        //public async Task<GenericResult> GetOpenDailyId(string CompanyCode, string StoreId)
        //{
        //    GenericResult result = new GenericResult();
        //    using (IDbConnection db = _commonRepository.GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();
        //            if (!Date.HasValue)
        //            {
        //                Date = DateTime.Now;
        //            }
        //            //string query = $"USP_S_M_ItemCollection";
        //            string query = $"select dbo.[fnc_GetDailyID]( '{StoreId}','{CompanyCode}', '{Date}')";

        //            var dataX = db.ExecuteScalar(query, null);

        //            db.Close();
        //            result.Success = true;
        //            result.Data = dataX;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //            result.Success = false;
        //            result.Message = ex.Message;
        //            //throw ex;
        //        }
        //        finally
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //        }
        //        return result;
        //    }
        //}

        public async Task<GenericResult> GetDailyId(string CompanyCode, string StoreId, DateTime? Date)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    if(!Date.HasValue)
                    {
                        Date = DateTime.Now;
                    }
                    string dateStr = Date.Value.ToString("yyyy/MM/dd HH:mm:ss");
                    //string query = $"USP_S_M_ItemCollection";
                    string query = $"select dbo.[fnc_GetDailyID]( N'{StoreId}', N'{CompanyCode}', N'{dateStr}')";

                    var dataX = db.ExecuteScalar(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

        public async Task<GenericResult> GetItemCollection()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"USP_S_M_ItemCollection";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public async Task<GenericResult> GetConfigType()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from  S_ConfigType with (nolock) where 1=1 ";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        //PdfDocumentProcessor documentProcessor
        private GenericResult createViewPrint(string PrintName, string PrintSize, PdfDocumentProcessor documentProcessor)
        {
            string Flag = "Init Print";
            GenericResult result = new GenericResult();
            try
            {


                PrinterSettings printerSettings = new PrinterSettings();
                if (!string.IsNullOrEmpty(PrintName))
                {
                    string[] spl = PrintName.Split(@"\");
                    string name = spl[spl.Length - 1];
                    printerSettings.PrinterName = name;
                    Flag = "Init Print Name" + printerSettings.PrinterName;
                }

                PdfPrinterSettings pdfPrinterSettings = new PdfPrinterSettings(printerSettings);

                pdfPrinterSettings.PageOrientation = PdfPrintPageOrientation.Portrait;

                pdfPrinterSettings.ScaleMode = PdfPrintScaleMode.CustomScale;
                if (PrintSize == "57")
                {
                    pdfPrinterSettings.Scale = float.Parse(Scale57);
                }
                else
                {
                    pdfPrinterSettings.Scale = float.Parse(Scale80);
                }

                Flag = "Print " + printerSettings.PrinterName;

                documentProcessor.Print(pdfPrinterSettings);

                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = Flag + " " + ex.Message;
            }
            return result;
        }

        //private GenericResult createViewPrint(string PrintName, RPFO.Application.PrintLayout.PrintPDF receipt, string PrintSize)
        //{
        //    string Flag = "Init Print";
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        //PrinterSettings printerSettings = new PrinterSettings();
        //        //if (!string.IsNullOrEmpty(PrintName))
        //        //{
        //        //    //string[] spl = PrintName.Split(@"\");
        //        //    //string name = spl[spl.Length - 1];
        //        //    printerSettings.PrinterName = PrintName;
        //        //    Flag = "Init Print Name" + printerSettings.PrinterName;
        //        //}

        //        //PdfPrinterSettings pdfPrinterSettings = new PdfPrinterSettings(printerSettings);

        //        //pdfPrinterSettings.PageOrientation = PdfPrintPageOrientation.Portrait;

        //        //pdfPrinterSettings.ScaleMode = PdfPrintScaleMode.CustomScale;

        //        ////Flag = "Print " + printerSettings.PrinterName;
        //        //ReportPrintTool printTool = new ReportPrintTool(report);
        //        //printTool.Print("Printer Name");

        //        //receipt.PrintingSystem.Document.AutoFitToPagesWidth = 1;
        //        receipt.Print();
        //        result.Success = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = Flag + " " + ex.Message;
        //    }
        //    return result;
        //}

        public async Task<GenericResult> PrintByPDF(string CompanyCode, string StoreId, string pdfFileName, string PrintName, string PrintSize, string printStatus)
        {
            string Folder = Path.Combine(PrintFolder, "");
            string Flag = "";
            string FilePath = Folder + @"\" + pdfFileName + ".pdf";
            GenericResult result = new GenericResult();
            await Task.Delay(int.Parse(WaiTime) * 1000);
            try
            {
                PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor();

                Flag = "Load File";
                documentProcessor.LoadDocument(Folder + @"\" + pdfFileName + ".pdf");

                Flag = "Go to Print";

                int maxTry = 3;
                GenericResult printResult = new GenericResult();
                for (int i = 1; i <= maxTry; i++)
                {
                    printResult = createViewPrint(PrintName, PrintSize, documentProcessor); //documentProcessor
                    if (printResult.Success)
                    {
                        break;
                    }

                }

                documentProcessor.CloseDocument();

                await Task.Delay(int.Parse(WaiTime) * 1000);
                File.Delete(Path.Combine(Folder, pdfFileName + ".pdf"));

                Flag = "After Print";
                if (printResult != null && printResult.Success)
                {

                    GenericResult cutResult = await PageCut(true, PrintName);
                    if (cutResult.Success)
                    {
                        if (!string.IsNullOrEmpty(CompanyCode) && !string.IsNullOrEmpty(StoreId))
                        {
                            var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);
                            List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                            if (settingData.Success)
                            {

                                SettingList = settingData.Data as List<GeneralSettingStore>;

                            }
                            string openDrawer = "false";
                            if (!string.IsNullOrEmpty(printStatus))
                            {
                                switch (printStatus.ToLower())
                                {
                                    case "receipt re-print":

                                        var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnReprint").FirstOrDefault();
                                        if (setting != null)
                                        {
                                            openDrawer = setting.SettingValue;
                                        }
                                        break;
                                    case "hold":

                                        var settingHold = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnHold").FirstOrDefault();
                                        if (settingHold != null)
                                        {
                                            openDrawer = settingHold.SettingValue;
                                        }
                                        break;
                                    case "receipt":

                                        var settingReceipt = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnReceipt").FirstOrDefault();
                                        if (settingReceipt != null)
                                        {
                                            openDrawer = settingReceipt.SettingValue;
                                        }
                                        break;
                                    default:
                                        openDrawer = "false";
                                        break;
                                }
                                if (openDrawer == "true")
                                {

                                    OpenDrawerCash(PrintName, "");
                                }
                            }
                        }
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Cut failed: " + cutResult.Message;
                    }
                }
                else
                {
                    printResult.Message = FilePath + " " + printResult.Message;
                    result = printResult;
                }



                // hết phần process

                //PdfViewer pdfViewer = new PdfViewer();
                //pdfViewer.LoadDocument(@"..\..\Demo.pdf");

                //int maxTry = 3;
                //GenericResult printResult = new GenericResult();

                //if (PrintSize == "57")
                //{


                //}
                //else
                //{

                //}

                //RPFO.Application.PrintLayout.PrintPDF receipt = new RPFO.Application.PrintLayout.PrintPDF();
                //receipt.

                //receipt.PaperKind = PaperKind.Custom;
                //if (!string.IsNullOrEmpty(PrintName))
                //{
                //    receipt.PrinterName = PrintName;
                //} 
                //if (PrintSize == "57")
                //{

                //    receipt.PageWidth = 550;

                //}
                //else
                //{
                //    receipt.PageWidth = 700;
                //}
                //receipt.PaperKind = PaperKind.Custom;
                //receipt.PageWidth = 80;
                //receipt.PaperKind = PaperKind.A3;
                //receipt.PrintingSystem.Document.ScaleFactor = 0.6f;
                //receipt.SetModel(FilePath, PrintSize);
                //receipt.CreateDocument();

                //for (int i = 1; i <= maxTry; i++)
                //{
                //    printResult = createViewPrint(PrintName, receipt, PrintSize); //documentProcessor
                //    if (printResult.Success)
                //    {
                //        break;
                //    }

                //} 
                //    await Task.Delay(int.Parse(WaiTime) * 1000);
                //    File.Delete(Path.Combine(Folder, pdfFileName + ".pdf"));

                //    Flag = "After Print";
                //    if (printResult != null && printResult.Success)
                //    {

                //        GenericResult cutResult = await PageCut(true, PrintName);
                //        if (cutResult.Success)
                //        {
                //            if (!string.IsNullOrEmpty(CompanyCode) && !string.IsNullOrEmpty(StoreId))
                //            {
                //                var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);
                //                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                //                if (settingData.Success)
                //                {

                //                    SettingList = settingData.Data as List<GeneralSettingStore>;

                //                }
                //                string openDrawer = "false";
                //                if (!string.IsNullOrEmpty(printStatus))
                //                {
                //                    switch (printStatus.ToLower())
                //                    {
                //                        case "receipt re-print":

                //                            var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnReprint").FirstOrDefault();
                //                            if (setting != null)
                //                            {
                //                                openDrawer = setting.SettingValue;
                //                            }
                //                            break;
                //                        case "hold":

                //                            var settingHold = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnHold").FirstOrDefault();
                //                            if (settingHold != null)
                //                            {
                //                                openDrawer = settingHold.SettingValue;
                //                            }
                //                            break;
                //                        case "receipt":

                //                            var settingReceipt = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnReceipt").FirstOrDefault();
                //                            if (settingReceipt != null)
                //                            {
                //                                openDrawer = settingReceipt.SettingValue;
                //                            }
                //                            break;
                //                        default:
                //                            openDrawer = "false";
                //                            break;
                //                    }
                //                    if (openDrawer == "true")
                //                    {

                //                        OpenDrawerCash(PrintName, "");
                //                    }
                //                }
                //            }
                //            result.Success = true;
                //        }
                //        else
                //        {
                //            result.Success = false;
                //            result.Message = "Cut failed: " + cutResult.Message;
                //        }


                //    }
                //    else
                //    {
                //        printResult.Message = FilePath + " " + printResult.Message;
                //        result = printResult;

                //    }


                //if (printResult.Success)
                //{
                //    //documentProcessor.CloseDocument();
                //    pdfViewer.CloseDocument();

                //}
                //else
                //{
                //    result.Success = false;
                //    result.Message = "Print failed: " + printResult.Message;
                //}




            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = FilePath+ " " + Flag + " " + ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetRoundingMethod()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _commonRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from fn_GetRoundingMethod()";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        } 
    }

}

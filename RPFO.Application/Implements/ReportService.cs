
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModel.RPT;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<MBomheader> _bomHeaderRepository;
        //    private readonly IGenericRepository<MBomline> _bomLineRepository;

        private readonly IMapper _mapper;
        private string CustomConnection = "";
        public ReportService(IMapper mapper, IGenericRepository<MBomheader> bomHeaderRepository, IConfiguration config
         // IGenericRepository<MBomline> bomLineRepository,/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _bomHeaderRepository = bomHeaderRepository;
            //_bomLineRepository = bomLineRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            try
            {
                CustomConnection = Encryptor.DecryptString(config.GetConnectionString("ReportConnection"), AppConstants.TEXT_PHRASE);
            }
            catch(Exception Ex)
            {

            }
            initService();
        }
        public GenericResult initService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection())
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
        public async Task<GenericResult> Get_RPT_InventoryAudit(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();
            
            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_InventoryAuditModel>("USP_RPT_InventoryAudit", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
            //throw new NotImplementedException();
        }

        public async Task<GenericResult> Get_RPT_InventoryOnHand(string CompanyCode, string StoreId, string Userlogin)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    var items = await db.QueryAsync<RPT_InventoryOnHandModel>("USP_RPT_InventoryOnHand", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_SalesStoreSummary(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_SalesStoreSummaryModel>("USP_RPT_SalesStoreSummary", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_SalesTransactionDetail(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_SalesTransactionDetailModel>("USP_RPT_SalesTransactionDetail", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_Dash_SaleDetailTransactionByTop(string CompanyCode, string StoreId, string FromDate, string ToDate, string ViewType , string ViewBy ,int? Top)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("FromDate", FromDate);
                    parameters.Add("ToDate", ToDate);
                    parameters.Add("Type", ViewType);
                    parameters.Add("ViewBy", ViewBy);
                    parameters.Add("Top", Top);
                    var items = await db.QueryAsync("USP_Dash_SaleDetailTransactionByTop", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_SalesTransactionDetail_Return(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_SalesTransactionDetailModel>("USP_RPT_SalesTransactionDetail_Return", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_SalesTransactionDetail_Ex(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_SalesTransactionDetailModel>("USP_RPT_SalesTransactionDetail_Ex", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_SalesTransactionSummary(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_SalesTransactionSummaryModel>("USP_RPT_SalesTransactionSummary", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_SalesTransactionSummaryByDepartment(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate, string DailyId)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    parameters.Add("DailyId", DailyId);
                    var items = await db.QueryAsync("USP_RPT_SalesTransactionDetail_SummaryByDeparment", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_SalesTopProduct(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate, int? Top)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    parameters.Add("Top", Top);
                    var items = await db.QueryAsync<RPT_SalesTopProductModel>("USP_RPT_SalesTopProduct", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_SalesByHour(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);

                    var items = await db.QueryAsync<RPT_SalesByHourModel>("USP_RPT_SalesByHour", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public class RPTResult
        {
            public object Header { get; set; }
            public object Lines { get; set; }

        }
        public GenericResult Get_RPT_SyncDataStatusView(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    //parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    //parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FromDate", FDate);
                    parameters.Add("ToDate", TDate);

                    var reader = db.QueryMultiple("USP_RPT_SyncDataStatusView_New", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);

                    //var reader = await db.QueryMultipleAsync("USP_RPT_POSPromo", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var Header = reader.Read().ToList();
                    var List = reader.Read().ToList();
                    RPTResult data = new RPTResult();
                    data.Header = Header;
                    data.Lines = List;

                    result.Success = true;
                    result.Data = data;
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

        public async Task<GenericResult> Get_RPT_SalesByYear(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);

                    var items = await db.QueryAsync<RPT_SalesByYearModel>("USP_RPT_SalesByYear", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_SalesBySalesPerson(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);

                    var items = await db.QueryAsync<RPT_SalesBySalesPersonModel>("USP_RPT_SalesBySalesPerson", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_SalesTransactionPayment(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_SalesTransactionPaymentModel>("USP_RPT_SalesTransactionPayment", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_Dashboard(string CompanyCode, string StoreId, string Userlogin, string Date)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("Date", Date);
                    var items = await db.QueryAsync<RPT_DashboardModel>("USP_RPT_Dashboard", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_LoadChartOrderPeriodByYear(string companyCode, string storeId, string userlogin, string year)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("UserId", userlogin);
                    parameters.Add("Year", year);
                    var items = await db.QueryAsync("USP_LoadChartOrderPeriodByYear", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_LoadChartOrderPeriodByMonth(string companyCode, string storeId, string userlogin, string year, string month)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("UserId", userlogin);
                    parameters.Add("Year", year);
                    parameters.Add("Month", month);
                    var items = await db.QueryAsync("USP_LoadChartOrderPeriodByMonth", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_LoadChartOrderPeriodByWeek(string companyCode, string storeId, string userlogin)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("UserId", userlogin);
                    var items = await db.QueryAsync("USP_LoadChartOrderPeriodByWeek", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_Rpt_GiftVoucher(string fromDate, string toDate, string OutletID)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@FDate", fromDate);
                    parameters.Add("@TDate", toDate);
                    parameters.Add("OutletID", OutletID);
                    var items = await db.QueryAsync<Rpt_GiftVoucherModel>("USP_Rpt_GiftVoucher", parameters, commandType: CommandType.StoredProcedure, commandTimeout:3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_POSPromo(string fromDate, string toDate, string OutletID)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("FDate", fromDate);
                    parameters.Add("TDate", toDate);
                    parameters.Add("OutletID", OutletID);
                    var items = await db.QueryAsync<RPT_POSPromoModel>("USP_RPT_POSPromo", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_InventoryPosting(string CompanyCode, string StoreId, string Userlogin, string fromDate, string toDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", fromDate);
                    parameters.Add("TDate", toDate);
                    var items = await db.QueryAsync<RPT_InventoryPostingModel>("USP_RPT_InventoryPosting", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_InvoiceTransactionDetail(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_InvoiceTransactionDetailModel>("USP_RPT_InvoiceTransactionDetail", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> Get_RPT_InvoiceTransactionSummary(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_InvoiceTransactionSummaryModel>("USP_RPT_InvoiceTransactionSummary", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_InvoiceTransactionPayment(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_InvoiceTransactionPaymentModel>("USP_RPT_InvoiceTransactionPayment", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_InventorySerial(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_InventorySerialModel>("USP_RPT_InventorySerial", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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

        public class SPromoOTGroupRpt  : SPromoOTGroup
        {
            public string DataTable { get; set; }
            public string AdditionValue { get; set; }
        }
        public class SPromoHeaderRpt : SPromoHeader
        {
            public string ApplyLineValue { get; set; }
            public string StoreCode { get; set; }
            public string SchemaId { get; set; }
            public string Priority { get; set; }
            public string CustomerCode { get; set; }
            //public decimal? MaxQtyByStore { get; set; }
            //public string StoreCode { get; set; }

        }
        public class PromotionViewModelRpt : PromotionViewModel
        {
            public string ApplyLineValue { get; set; }
            public string StoreCode { get; set; }
            public string SchemaId { get; set; }
            public string Priority { get; set; }
            public string CustomerCode { get; set; }
        }
        public class RPT_SOToDevitionResult 
        {
           public object Header { get; set; }
           public object Data { get; set; }
        }
        public async Task<GenericResult> Get_RPT_SOToDivision(string CompanyCode, string Date, string CusId, string TransId, bool? InComplete)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("Date", Date);
                    parameters.Add("CusId", string.IsNullOrEmpty(CusId) ? "" : CusId );
                    parameters.Add("TransId", string.IsNullOrEmpty(TransId) ? "" : TransId);
                    parameters.Add("InComplete", InComplete);
                    //var items = await db.QueryAsync("USP_S_GetSOToDivision", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var reader = await db.QueryMultipleAsync("USP_S_GetSOToDivision", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var dataLine  = reader.Read().ToList();
                    var header = reader.Read().ToList();
                    var data = new RPT_SOToDevitionResult();
                    data.Header = header;
                    data.Data = dataLine;

                    result.Success = true;
                    result.Data = data;
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

        public async Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var reader = await db.QueryMultipleAsync("USP_RPT_POSPromo", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var ProductListHeader = reader.Read<SPromoHeaderRpt>().ToList();
                    var ProductListBuy = reader.Read<SPromoBuy>().ToList();
                    var ProductListGet = reader.Read<SPromoGet>().ToList();
                    var ProductListOTGroupBuy = reader.Read<SPromoOTGroupRpt>().ToList();
                    var ProductListOTGroupGet = reader.Read<SPromoOTGroupRpt>().ToList();
                    foreach (var BuyLine in ProductListBuy)
                    {
                        if(BuyLine.LineType == "OneTimeGroup")
                        {
                            var list = ProductListOTGroupBuy.Where(x => x.PromoId == BuyLine.PromoId && x.DataTable == "BuyOneTimeGroup").ToList();
                            BuyLine.Lines = _mapper.Map<List<SPromoOTGroup>>(list);
                        }     
                    }
                    foreach (var GetLine in ProductListGet)
                    {
                        if (GetLine.LineType == "OneTimeGroup")
                        {
                            var list = ProductListOTGroupGet.Where(x => x.PromoId == GetLine.PromoId && x.DataTable == "GetOneTimeGroup").ToList();
                            GetLine.Lines = _mapper.Map<List<SPromoOTGroup>>(list);
                        }
                    }
                    List<PromotionViewModelRpt> rptList = new List<PromotionViewModelRpt>();
                    foreach (var header in ProductListHeader)
                    {
                        PromotionViewModelRpt promotion = new PromotionViewModelRpt();
                        promotion = _mapper.Map<PromotionViewModelRpt>(header);
                        promotion.PromoBuys = ProductListBuy.Where(x => x.PromoId == header.PromoId).ToList();
                        promotion.PromoGets = ProductListGet.Where(x => x.PromoId == header.PromoId).ToList();
                        rptList.Add(promotion);
                    }    

                    result.Success = true;
                    result.Data = rptList;
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
        public async Task<GenericResult> Get_RPT_VoucherCheckIn(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate, string Keyword)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    parameters.Add("Keyword", Keyword);
                    var items = await db.QueryAsync("USP_RPT_VoucherCheckIn", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_SalesbyItem(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync<RPT_SalesByItemModel>("USP_RPT_SalesByItem", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_ActionOnOrder(string CompanyCode, string StoreId, string TransId, string User, string Userlogin, string FDate, string TDate, string Type)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();


                    string queryCheckAndCreate = "IF (OBJECT_ID('USP_RPT_ActionOnOrder') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                        " set @string = 'create PROCEDURE [dbo].[USP_RPT_ActionOnOrder] @CompanyCode nvarchar(50), @StoreId	nvarchar(150), @Userlogin nvarchar(50), @User nvarchar(50), @TransId nvarchar(50), @FDate date, @TDate date, @Type nvarchar(50) AS " +
                        "begin  select * FROM [dbo].[S_Log] with (nolock) where  (ISNULL(@CompanyCode, '''') = '''' OR CompanyCode = @CompanyCode )  and   (ISNULL(@StoreId, '''') = '''' OR StoreId = @StoreId ) and   (ISNULL(@User, '''') = '''' OR CreatedBy = @User )  and   (ISNULL(@TransId, '''') = '''' OR TransId = @TransId )    and   (ISNULL(@Type, '''') = '''' OR Type = @Type ) and   (ISNULL(@FDate, '''') = '''' OR CONVERT(date, CreatedOn) >=  CONVERT(date, @FDate) ) and   (ISNULL(@TDate, '''') = '''' OR CONVERT(date, CreatedOn) <=  CONVERT(date, @TDate) )   ORDER BY lineNum + 0 ASC  " +
                         
                        " end '; " +
                        " EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreate);


                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("TransId", TransId);
                    parameters.Add("User", User);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    parameters.Add("Type", Type);
                    var items = await db.QueryAsync("USP_RPT_ActionOnOrder", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_SalesEPAYDetail(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    var items = await db.QueryAsync("USP_RPT_SalesEPAYDetail", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> RPT_SYNC_ITEM_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    //parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("FItem", string.IsNullOrEmpty(FItem)  ? "" : FItem);
                    parameters.Add("TItem", string.IsNullOrEmpty(TItem) ? "" : TItem);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);

                    var reader = await db.QueryMultipleAsync("USP_RPT_SYNC_ITEM_CMP", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var items = reader.Read().ToList();
                    var Error = reader.Read<string>().ToList();
                    string message = "";
                    if (Error!=null)
                    { 
                        string mes = Convert.ToString(Error.FirstOrDefault());
                        if (!string.IsNullOrEmpty(mes))
                        {
                            message = mes;
                        }    
                    }    
                    if(!string.IsNullOrEmpty(message))
                    {
                        result.Message = message;
                    }    
                    //else
                    //{ 
                    //    result.Success = false;
                       
                    //}
                    result.Success = true;
                    result.Data = items;
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
       public async Task<GenericResult> RPT_SYNC_LISTING_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    //parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("FItem", string.IsNullOrEmpty(FItem)  ? "" : FItem);
                    parameters.Add("TItem", string.IsNullOrEmpty(TItem) ? "" : TItem);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);

                    var reader = await db.QueryMultipleAsync("USP_RPT_SYNC_LISTING_CMP", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var items = reader.Read().ToList();
                    var Error = reader.Read<string>().ToList();
                    string message = "";
                    if (Error!=null)
                    { 
                        string mes = Convert.ToString(Error.FirstOrDefault());
                        if (!string.IsNullOrEmpty(mes))
                        {
                            message = mes;
                        }    
                    }    
                    if(!string.IsNullOrEmpty(message))
                    {
                        result.Message = message;
                    }    
                    //else
                    //{ 
                    //    //result.Success = false;
                       
                    //}
                    result.Success = true;
                    result.Data = items;
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

     
        public async Task<GenericResult> RPT_SYNC_PROMO_CMP(string CompanyCode, string FId, string TId, DateTime? FDate, DateTime? TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    //parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("FPromoID", string.IsNullOrEmpty(FId) ? "" : FId);
                    parameters.Add("TPromoID", string.IsNullOrEmpty(TId) ? "" : TId);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);

                    var reader = await db.QueryMultipleAsync("USP_RPT_SYNC_PROMO_CMP", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var items = reader.Read().ToList();
                    var Error = reader.Read<string>().ToList();
                    string message = "";
                    if (Error != null)
                    {
                        string mes = Convert.ToString(Error.FirstOrDefault());
                        if (!string.IsNullOrEmpty(mes))
                        {
                            message = mes;
                        }
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        result.Message = message;
                    }
                    //else
                    //{
                    //    result.Success = false;
                       
                    //}
                    result.Success = true;
                    result.Data = items;
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

        public async Task<GenericResult> RPT_SYNC_PRICE_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    //parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("FItem", string.IsNullOrEmpty(FItem) ? "" : FItem);
                    parameters.Add("TItem", string.IsNullOrEmpty(TItem) ? "" : TItem);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);

                    var reader = await db.QueryMultipleAsync("USP_RPT_SYNC_PRICE_CMP", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var items = reader.Read().ToList();
                    var Error = reader.Read<string>().ToList();
                    string message = "";
                    if (Error != null)
                    {
                        string mes = Convert.ToString(Error.FirstOrDefault());
                        if (!string.IsNullOrEmpty(mes))
                        {
                            message = mes;
                        }
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        //result.Success = true;
                        //result.Data = items;
                        result.Message = message;
                    }
                     

                    //var items = await db.QueryAsync("USP_RPT_SYNC_PRICE_CMP", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public class ObjectResult
        {
            public object Header { get; set; }
            public object Data { get; set; }
        }
        public async Task<GenericResult> Get_RPT_SyncDataStatusByIdoc(string CompanyCode, string IdocNum, string DataType)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("IdocNum", IdocNum);
                    parameters.Add("DataType", DataType);
                    var items = await db.QueryAsync("USP_RPT_SyncDataStatusByIdoc", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
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
        public async Task<GenericResult> Get_RPT_CollectionDailyByCounter(string CompanyCode, string StoreId, string Userlogin,string Date)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _bomHeaderRepository.GetConnection(GConnection.ReportConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    parameters.Add("Userlogin", string.IsNullOrEmpty(Userlogin) ? "" : Userlogin);
                    parameters.Add("Date", Date); 

                    var reader = await db.QueryMultipleAsync("USP_RPT_CollectionDailyByCounter", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var items = reader.Read().ToList();
                    var headers = reader.Read().ToList();
                    var data = new ObjectResult();
                    data.Header = headers;
                    data.Data = items;

                    result.Success = true;
                    result.Data = data;
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
    }

}


using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ClientDisallowanceService: IClientDisallowanceService
    {
        private readonly IGenericRepository<SClientDisallowance> _disallowanceRepository;

        private readonly IMapper _mapper;
        public ClientDisallowanceService(IGenericRepository<SClientDisallowance> disallowanceRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _disallowanceRepository = disallowanceRepository;
            _mapper = mapper; 

        }
        //public GenericResult InitFirstService()
        //{
        //    GenericResult result = new GenericResult();

        //    using (IDbConnection db = _commonRepository.GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();

        //            string queryCheckfn_GetTableInfor = $" IF NOT EXISTS (SELECT *  FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[fn_GetTableInfor]') AND type IN(N'FN', N'IF', N'TF', N'FS', N'FT')) " +
        //                $" begin declare @string nvarchar(MAX) = ''; " +
        //                $" set @string = 'create FUNCTION [dbo].[fn_GetTableInfor]    ( @TableName nvarchar(250), @ConvertToC bit ) RETURNS TABLE AS Return    select  replace(col.name, '' '', ''_'') ColumnName,  column_id ColumnId, case @ConvertToC when 1 " +
        //                $" then case typ.name  when ''bigint'' then ''long'' when ''binary'' then ''byte[]'' when ''bit'' then ''bool''" +
        //                $" when ''char'' then ''string'' when ''date'' then ''DateTime'' when ''datetime'' then ''DateTime'' when ''datetime2'' then ''DateTime'' when ''datetimeoffset'' then ''DateTimeOffset''" +
        //                $" when ''decimal'' then ''decimal'' when ''float'' then ''double'' when ''image'' then ''byte[]'' when ''int'' then ''int'' when ''money'' then ''decimal'' when ''nchar'' then ''string''" +
        //                $" when ''ntext'' then ''string'' when ''numeric'' then ''decimal'' when ''nvarchar'' then ''string'' when ''real'' then ''float'' when ''smalldatetime'' then ''DateTime'' when ''smallint'' then ''short'' when ''smallmoney'' then ''decimal'' " +
        //                $" when ''text'' then ''string'' when ''time'' then ''TimeSpan'' when ''timestamp'' then ''long'' when ''tinyint'' then ''byte'' when ''uniqueidentifier'' then ''Guid'' when ''varbinary'' then ''byte[]''" +
        //                $" when ''varchar'' then ''string'' else ''UNKNOWN_'' + typ.name end else  typ.name   end ColumnType," +
        //                $" case typ.name   when ''nvarchar''   then ''('' +   case when  COL_LENGTH(OBJECT_NAME(object_id), col.name) = -1  then ''MAX'' else CONVERT(nvarchar(50) ,  COL_LENGTH(OBJECT_NAME(object_id), col.name)  ) end+ '')''" +
        //                $" when ''varchar'' then  ''('' + CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')''  when ''nchar'' then  ''('' + CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')''" +
        //                $" when ''char'' then  ''('' +CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')'' when ''text'' then   ''('' + CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name))+ '')''" +
        //                $" when ''text'' then ''('' +CONVERT(nvarchar(50) , COL_LENGTH(OBJECT_NAME(object_id), col.name)) + '')'' when ''decimal'' then ''(19, 6)'' " +
        //                $" end ColumnLength, case  when col.is_nullable = 1 " +
        //                //and typ.name in (''bigint'', ''bit'', ''date'', ''datetime'', ''datetime2'', ''datetimeoffset'', ''decimal'', ''float'', ''int'', ''money'', ''numeric'', ''real'', ''smalldatetime'', ''smallint'', ''smallmoney'', ''time'', ''tinyint'', ''uniqueidentifier'') 
        //                $" then ''?''  else ''''  end NullableSign from sys.columns col join sys.types typ on  col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id   where object_id = object_id(@TableName)" +
        //                " '; " +
        //                " EXECUTE sp_executesql @string;  end";
        //            db.Execute(queryCheckfn_GetTableInfor);


        //            string queryCheckAndCreateStoreCreateTableType = "IF (OBJECT_ID('USP_CheckAndCreateTableType') IS NULL)  begin declare @string nvarchar(MAX) = ''; " +
        //                " set @string = 'create PROCEDURE [dbo].[USP_CheckAndCreateTableType] @TableName nvarchar(250), @IsDropAndCreate bit AS  " +
        //                "begin   if ((select count(*) from dbo.[fn_GetTableInfor] ( @TableName , 0)) <> 0)   " +
        //                "begin declare @Result varchar(max)  = ''''; declare @NumOfTable int; " +
        //                "declare @NumOfTableType int; declare @NamTableType varchar(450)  = ''''; set @NamTableType =  QUOTENAME(@TableName + ''TableType''); set 	@TableName = QUOTENAME(@TableName)  " +
        //                "IF EXISTS (SELECT * FROM   [sys].[table_types]  WHERE  user_type_id = Type_id(@NamTableType))  BEGIN if (@IsDropAndCreate = 1) begin set @NumOfTable = (select count(column_name) as Number " +
        //                "from information_schema.columns with (nolock)  where table_name= @TableName) set @NumOfTableType = ( Select COUNT( c.name ) as Number From sys.table_types t  with (nolock)  Inner join sys.columns c with (nolock) on c.object_id = t.type_table_object_id WHERE t.is_user_defined = 1 AND t.is_table_type = 1 and t.name = @NamTableType)  if(@NamTableType <> @NumOfTable) begin   declare @Drop  varchar(max);  set @Drop = N''DROP TYPE '' + @NamTableType + '' ;''; set @Result = @Drop + '' CREATE TYPE dbo.'' + @NamTableType + '' AS TABLE('' select @Result = @Result + '''' + QUOTENAME(ColumnName) +   '' '' +  QUOTENAME( ColumnType ) +   ''''+ isnull(ColumnLength ,'''')  +  '' '' + case  when   NullableSign = ''?'' then ''null'' else ''not null'' end + '','' from ( select * from dbo.[fn_GetTableInfor] ( @TableName , 0) ) t order by ColumnId set @Result = (SELECT LEFT(@Result, NULLIF(LEN(@Result)-1,-1)))   + '')''				select  @Result end else begin select  ''-1'' end  end  else begin select ''-1'' end END  else begin set @Result  = N''CREATE TYPE dbo.'' + @NamTableType + '' AS TABLE( ''  select @Result = @Result + '''' + QUOTENAME(ColumnName) +   '' '' +  QUOTENAME( ColumnType ) +   ''''+ isnull(ColumnLength ,'''')  +  '' '' + case  when   NullableSign = ''?'' then ''null'' else ''not null'' end + '','' from ( select * from dbo.[fn_GetTableInfor] ( @TableName , 0) ) t order by ColumnId set @Result =  (SELECT LEFT(@Result, NULLIF(LEN(@Result)-1,-1)))   + '')''  set @Result = N'''' + @Result +''''; select  @Result  end end else begin select  ''-1'' end end'; EXECUTE sp_executesql @string;  end";
        //            db.Execute(queryCheckAndCreateStoreCreateTableType);



        //            result.Success = true;

        //            //tran.Commit();

        //        }
        //        catch (Exception ex)
        //        {
        //            result.Success = false;
        //            result.Message = ex.Message;
        //            //result.Data = failedlist;
        //        }
        //        finally
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //        }
        //    }
        //    //result.Data = failedlist;
        //    return result;



        //}
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<UOMResultViewModel> resultlist = new List<UOMResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Uom)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
        //            //if (itemResult.Success == false)
        //            //{
        //                UOMResultViewModel itemRs = new UOMResultViewModel();
        //                itemRs = _mapper.Map<UOMResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
        //            //}
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        //result.Data = failedlist;
        //    } 
        //    return result;
        //}


        public async Task<GenericResult> Create(SClientDisallowance model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 
              


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("CounterId", model.CounterId);
                parameters.Add("Permission", model.Permission);
                parameters.Add("FunctionId", model.FunctionId);
                parameters.Add("ControlId", model.ControlId);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Status", model.Status);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.ModifiedBy);

                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _disallowanceRepository.Insert("USP_I_S_ClientDisallowance", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
         
        public async Task<GenericResult> Delete(SClientDisallowance model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("CounterId", model.CounterId);
                parameters.Add("Id", model.Id); 
                var data = _disallowanceRepository.Execute($"USP_D_S_ClientDisallowance", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
               
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
     
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Id, string CounterId, string Keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("CounterId", CounterId);
                parameters.Add("Id", Id);
                parameters.Add("Keyword", Keyword);

                var data = await _disallowanceRepository.GetAllAsync($"USP_S_S_ClientDisallowance", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("CounterId", "");
                parameters.Add("Id", Id);
                parameters.Add("Keyword", "");
                var data = await _disallowanceRepository.GetAsync($"USP_S_S_ClientDisallowance", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

     

        public async Task<GenericResult> Update(SClientDisallowance model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("CounterId", model.CounterId);
                parameters.Add("Permission", model.Permission);
                parameters.Add("FunctionId", model.FunctionId);
                parameters.Add("ControlId", model.ControlId); 
                parameters.Add("Remark", model.Remark);
                parameters.Add("Status", model.Status);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("ModifiedBy", model.ModifiedBy );
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _disallowanceRepository.Insert("USP_U_S_ClientDisallowance", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

     

        
    }

}

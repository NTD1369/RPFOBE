
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ShippingDivisionService : IShippingDivisionService
    {
        private readonly IGenericRepository<TShippingDivisionHeader> _devisionRepository;
        private readonly IGenericRepository<TShippingDivisionLine> _devisionLineRepository;
        string ServiceName = "T_ShippingDivision";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;
        public ShippingDivisionService(IGenericRepository<TShippingDivisionHeader> devisionRepository,
            IGenericRepository<TShippingDivisionLine> devisionLineRepository, 
            ICommonService commonService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _devisionRepository = devisionRepository;
            _devisionLineRepository = devisionLineRepository;
            _mapper = mapper;
           
            _commonService = commonService;
            _mapper = mapper;
            TableNameList.Add(ServiceName + "Line");
            //TableNameList.Add(ServiceName + "LineSerial");
            _commonService.InitService(ServiceName, TableNameList);

            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            //InitService();
        }
        public GenericResult InitService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _devisionRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string queryCheckAndCreate = "IF (OBJECT_ID('USP_S_GetControlByFunction') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                       " set @string = 'create PROCEDURE [dbo].[USP_S_GetControlByFunction] @CompanyCode nvarchar(50), @Function	nvarchar(150) AS " +
                       "begin  select * from M_Control with(nolock) where CompanyCode = @CompanyCode  and Status = ''A''  and FunctionId = @Function order By OrderNum end '; " +
                       "EXECUTE sp_executesql @string;  end";
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
            }


        }
        public async Task<GenericResult> Create(TShippingDivisionHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                string POLineTbl = ServiceName + "Line";
                var POLines = _commonService.CreaDataTable(POLineTbl);


                var parameters = new DynamicParameters();
 
                //parameters.Add("Id", model.Id, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("StoreName", model.StoreName); 
                parameters.Add("ShiftId", model.ShiftId);
                parameters.Add("ContractNo", model.ContractNo);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("CusId", model.CusId);
                parameters.Add("CusGrpId", model.CusGrpId);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("IsCanceled", model.IsCanceled);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("DocDate", model.DocDate);

                int stt = 0;
                
                foreach (var line in model.Lines)
                {
                    stt++;

                    line.Id = "";
                    line.LineId = stt.ToString();
                    line.CreatedBy = model.CreatedBy;
                    line.CompanyCode = model.CompanyCode;
                    //line.CreatedOn = DateTime.Now;
                    //line.ModifiedBy = null;
                    //line.ModifiedOn = null;
                    //else
                    //{
                    //    line.CreatedOn = model.CreatedOn;
                    //    line.ModifiedBy = model.ModifiedBy;
                    //    line.ModifiedOn = DateTime.Now;

                    //}

                }
                POLines = ExtensionsNew.ConvertListToDataTable(model.Lines, POLines);

                string tblLineType = POLineTbl + "TableType";
                //string tblGISerialTbl = POSerialTbl + "TableType";

                parameters.Add("@Lines", POLines.AsTableValuedParameter(POLineTbl + "TableType"));

                var affectedRows = _devisionRepository.Insert("USP_I_T_ShippingDivisionHeader", parameters, commandType: CommandType.StoredProcedure);
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

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetDivisionToShip(string CompanyCode, string Id, string Date)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", string.IsNullOrEmpty(Id) ? "" : Id);
                parameters.Add("Date", string.IsNullOrEmpty(Date) ? "" : Date);

                //var affectedRows = _devisionRepository.Insert("USP_I_T_DevisionHeader", parameters, commandType: CommandType.StoredProcedure);
                var data = await _devisionLineRepository.GetAllAsync($"USP_GetDivisionToShip", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string FromDate, string ToDate )
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", "");
                parameters.Add("FromDate", string.IsNullOrEmpty(FromDate) ? "" : FromDate);
                parameters.Add("ToDate", string.IsNullOrEmpty(ToDate) ? "" : ToDate);
              
                //var affectedRows = _devisionRepository.Insert("USP_I_T_DevisionHeader", parameters, commandType: CommandType.StoredProcedure);
                var data = await _devisionRepository.GetAllAsync($"USP_S_T_ShippingDivisionHeader", parameters, commandType: CommandType.StoredProcedure);
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
       
        public async Task<GenericResult> GetByCode(string CompanyCode, string Id )
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", Id);
                parameters.Add("FromDate", "");
                parameters.Add("ToDate", ""); 
              
                var data = await _devisionRepository.GetAsync($"USP_S_T_ShippingDivisionHeader", parameters, commandType: CommandType.StoredProcedure);
                if(data!=null)
                {
                    var lineParameters = new DynamicParameters();
                    lineParameters.Add("CompanyCode", CompanyCode);
                    lineParameters.Add("Id", Id);
                    var lines = await _devisionLineRepository.GetAllAsync($"USP_S_T_ShippingDivisionLine", lineParameters, commandType: CommandType.StoredProcedure);
                    data.Lines = lines;
                }    
             
                result.Success = true;
                result.Data =data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
 
        public async Task<GenericResult> Update(TShippingDivisionHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", model.Id, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("StoreName", model.StoreName);
                parameters.Add("ContractNo", model.ContractNo); 
                parameters.Add("ShiftId", model.ShiftId); 
                parameters.Add("Status", model.Status);
                parameters.Add("CusId", model.CusId);
                parameters.Add("CusGrpId", model.CusGrpId);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("IsCanceled", model.IsCanceled);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                var affectedRows = _devisionRepository.Update("USP_U_T_ShippingDivisionHeader", parameters, commandType: CommandType.StoredProcedure);
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

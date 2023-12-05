
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
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
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<TPaymentHeader> _paymentRepository;
        private readonly IGenericRepository<TPaymentLine> _paymentLineRepository;
        string ServiceName = "T_Payment";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;
        public PaymentService(IGenericRepository<TPaymentHeader> paymentRepository, IGenericRepository<TPaymentLine> paymentLineRepository, ICommonService commonService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _paymentRepository = paymentRepository;
            _paymentLineRepository = paymentLineRepository;
            _commonService = commonService;
            _mapper = mapper;
            TableNameList.Add(ServiceName + "Line");
            //TableNameList.Add(ServiceName + "LineSerial");
            _commonService.InitService(ServiceName, TableNameList);

        }
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

         
        public async Task<GenericResult> Create(TPaymentHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                string POLineTbl = ServiceName + "Line";
                var POLines = _commonService.CreaDataTable(POLineTbl);


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("TransId", model.TransId); 
                parameters.Add("Title", !string.IsNullOrEmpty(model.Title) ? model.Title : ""); 
                parameters.Add("Remark", !string.IsNullOrEmpty(model.Remark) ? model.Remark : "" ); 
                parameters.Add("CusId", !string.IsNullOrEmpty(model.CusId) ? model.CusId : "" ); 
                parameters.Add("Type", !string.IsNullOrEmpty(model.Type) ? model.Type : ""  ); 
                parameters.Add("RefTransId", !string.IsNullOrEmpty(model.RefTransId) ? model.RefTransId : ""  ); 
                parameters.Add("Reason", !string.IsNullOrEmpty(model.Reason) ? model.Reason : ""); 
                parameters.Add("DocDate", model.DocDate); 
                parameters.Add("DocDueDate", model.DocDueDate); 
                parameters.Add("DocDate", model.DocDate); 
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                //if(!string.IsNullOrEmpty(model.CustomerF1)) 
                    parameters.Add("CustomF1", model.CustomF1); 
                //if (!string.IsNullOrEmpty(model.CustomerF2)) 
                    parameters.Add("CustomF2", model.CustomF2);
                //if (!string.IsNullOrEmpty(model.CustomerF3)) 
                    parameters.Add("CustomF3", model.CustomF3);
                //if (!string.IsNullOrEmpty(model.CustomerF4))
                    parameters.Add("CustomF4", model.CustomF4);
                //if (!string.IsNullOrEmpty(model.CustomerF5)) 
                    parameters.Add("CustomF5", model.CustomF5);

                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                int stt = 0;
                foreach (var line in model.Lines)
                {
                    stt++;

                    line.TransId = "";
                    line.LineId = stt.ToString();
                    line.CreatedBy = model.CreatedBy;
                    line.CompanyCode = model.CompanyCode;
                    line.PaymentMode = String.IsNullOrEmpty(line.PaymentMode) ? "" : line.PaymentMode;
                    line.PaymentType = String.IsNullOrEmpty(line.PaymentType) ? "" : line.PaymentType;
                    line.CardType = String.IsNullOrEmpty(line.CardType) ? "" : line.CardType;
                    line.CardHolderName = String.IsNullOrEmpty(line.CardHolderName) ? "" : line.CardHolderName;
                    line.CardHolderName = String.IsNullOrEmpty(line.CardHolderName) ? "" : line.CardHolderName;
                    line.CardHolderName = String.IsNullOrEmpty(line.CardHolderName) ? "" : line.CardHolderName;
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
                parameters.Add("@Lines", POLines.AsTableValuedParameter(POLineTbl + "TableType"));

                var key = _paymentRepository.GetScalar("USP_I_T_Payment", parameters, commandType: CommandType.StoredProcedure).ToString();
                result.Success = true;
                result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Delete(TPaymentHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode); 
                parameters.Add("TransId", model.TransId);

                var data = await _paymentRepository.GetAllAsync($"USP_D_T_Payment", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string CusId, string FromDate, string ToDate,string Status, string top)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CusId", CusId);
                parameters.Add("FromDate", FromDate);
                parameters.Add("ToDate", ToDate);
                parameters.Add("Top", top);
                parameters.Add("Id", "");
                parameters.Add("Status", Status);
                
                var data = await _paymentRepository.GetAllAsync($"USP_S_T_Payment", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CusId", "");
                parameters.Add("FromDate", "");
                parameters.Add("ToDate", "");
                parameters.Add("Top", "");
                parameters.Add("Id", Code);
                parameters.Add("Status", "");

                var data = await _paymentRepository.GetAsync($"USP_S_T_Payment", parameters, commandType: CommandType.StoredProcedure);

                if(data!=null)
                {
                    var Xparameters = new DynamicParameters();

                    Xparameters.Add("CompanyCode", CompanyCode);
                    Xparameters.Add("Id", Code);
                    var lines = await _paymentLineRepository.GetAllAsync($"USP_S_T_PaymentLine", Xparameters, commandType: CommandType.StoredProcedure);
                    if(lines!=null && lines.Count > 0)
                    {
                        data.Lines = lines;

                    }    
                }    
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

      
 

        public async Task<GenericResult> Update(TPaymentHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var delRs =   Delete(model).Result;
                if(delRs.Success)
                {
                    result = Create(model).Result;
                }   
                else
                {
                    result = delRs;
                }    
               
                //var parameters = new DynamicParameters();

                //parameters.Add("CompanyCode", model.CompanyCode);
                //parameters.Add("TransId", model.TransId);
                //parameters.Add("Title", !string.IsNullOrEmpty(model.Title) ? model.Title : "");
                //parameters.Add("Remark", !string.IsNullOrEmpty(model.Remark) ? model.Remark : "");
                //parameters.Add("CusId", !string.IsNullOrEmpty(model.CusId) ? model.CusId : "");
                //parameters.Add("Type", !string.IsNullOrEmpty(model.Type) ? model.Type : "");
                //parameters.Add("RefTransId", !string.IsNullOrEmpty(model.RefTransId) ? model.RefTransId : "");
                //parameters.Add("Reason", !string.IsNullOrEmpty(model.Reason) ? model.Reason : "");
                //parameters.Add("DocDate", model.DocDate);
                //parameters.Add("DocDueDate", model.DocDueDate);
                //parameters.Add("DocDate", model.DocDate);
                //parameters.Add("ModifiedBy", model.ModifiedBy);
                //parameters.Add("Status", model.Status);
                ////if(!string.IsNullOrEmpty(model.CustomerF1)) 
                //parameters.Add("CustomF1", model.CustomF1);
                ////if (!string.IsNullOrEmpty(model.CustomerF2)) 
                //parameters.Add("CustomF2", model.CustomF2);
                ////if (!string.IsNullOrEmpty(model.CustomerF3)) 
                //parameters.Add("CustomF3", model.CustomF3);
                ////if (!string.IsNullOrEmpty(model.CustomerF4))
                //parameters.Add("CustomF4", model.CustomF4);
                ////if (!string.IsNullOrEmpty(model.CustomerF5)) 
                //parameters.Add("CustomF5", model.CustomF5);
                //var affectedRows = _paymentRepository.Update("USP_U_T_Payment", parameters, commandType: CommandType.StoredProcedure);
                //result.Success = true;
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

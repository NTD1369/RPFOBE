
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
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
    public class SalesPlanService : ISalesPlanService
    {
        private readonly IGenericRepository<MSalesPlanHeader> _salesPlanRepository;
        private readonly IGenericRepository<MSalesPlanLine> _salesPlanLineRepository;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;
        private string PrefixSP = "SP";
        string ServiceName = "M_SalesPlan";
        List<string> TableNameList = new List<string>();
        public SalesPlanService(IGenericRepository<MSalesPlanHeader> salesPlanRepository, IGenericRepository<MSalesPlanLine> salesLineRepository, IConfiguration config, ICommonService commonService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _salesPlanRepository = salesPlanRepository;
            _salesPlanLineRepository = salesLineRepository;
            _mapper = mapper;
            _commonService = commonService;

            PrefixSP = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixSP"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(PrefixSP))
            {
                PrefixSP = "SP";
            }

            TableNameList.Add(ServiceName + "Line"); 
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

        public async Task<bool> checkExist(string CompanyCode, string HldCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("HldCode", HldCode);
            parameters.Add("StrDate", "");
            parameters.Add("EndDate", "");
            parameters.Add("Rmrks", "");
            parameters.Add("Status", "");
            parameters.Add("Keyword", "");
            var affectedRows = await _salesPlanRepository.GetAsync("USP_S_S_SalesPlan", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MSalesPlanHeader model, Boolean? isUpdate)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                string PlanLineTbl = "M_SalesPlanLine";
                var PlanLines = _commonService.CreaDataTable(PlanLineTbl);
  
                if (PlanLines == null )
                {
                    result.Success = false;
                    result.Message = "Table Type Object can't init";
                    return result;
                }
                string key = _salesPlanRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixSP}','{model.CompanyCode}','')", null, commandType: CommandType.Text);

                if (isUpdate == true)
                {
                    key = model.Id;
                }
               
                model.Id = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                //parameters.Add("SetupType", model.SetupType);
                //parameters.Add("SetupValue", model.SetupValue); 
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("FromDate", model.FromDate);
                parameters.Add("ToDate", model.ToDate);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status)? "C" : model.Status);
                parameters.Add("Remark", model.Remark);
              
             
                if(isUpdate == true)
                {
                    parameters.Add("PrefixSP", "");
                    parameters.Add("CreatedBy", model.CreatedBy);
                    parameters.Add("CreatedOn", model.CreatedOn.Value.ToString("yyyy/MM/dd HH:mm:ss"));
                    parameters.Add("ModifiedBy", model.ModifiedBy);
                    parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                }   
                else
                {
                    parameters.Add("CreatedBy", model.CreatedBy);                     
                    parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    parameters.Add("PrefixSP", PrefixSP);
                    parameters.Add("ModifiedBy", "");
                    parameters.Add("ModifiedOn", "");
                }    
                int lineNum = 0;
                foreach (var line in model.Lines)
                {
                    lineNum++;
                    line.CompanyCode = model.CompanyCode;
                    line.SalesPlanId = model.Id;
                    line.LineNum = lineNum;
                    line.StoreId = "TempVl";
                    line.Name = model.Name;
                    line.Description = model.Description;
                    line.FilterBy ="";
                    line.CustomF1 = string.IsNullOrEmpty(line.CustomF1)?"" : line.CustomF1;
                    line.CustomF2 = string.IsNullOrEmpty(line.CustomF2) ? "" : line.CustomF2;
                    line.CustomF3 = string.IsNullOrEmpty(line.CustomF3) ? "" : line.CustomF3;
                    line.CustomF4 = string.IsNullOrEmpty(line.CustomF4) ? "" : line.CustomF4;
                    line.Remark = string.IsNullOrEmpty(line.Remark) ? "" : line.Remark;
                    line.CustomF5 = string.IsNullOrEmpty(line.CustomF5) ? "" : line.CustomF5;
                    line.Status = string.IsNullOrEmpty(line.Status) ? "C" : line.Status;
                    line.CreatedBy = model.CreatedBy;

                    if (isUpdate == true)
                    {

                        line.CreatedOn = model.CreatedOn.Value;
                        line.ModifiedBy = model.ModifiedBy;
                        line.ModifiedOn = DateTime.Now;
                         
                    }
                    else
                    {
                        line.CreatedOn = DateTime.Now;
                        line.ModifiedBy = "";
                        line.ModifiedOn = null;
                       
                    }

                    //line.StoreId = "TempVl";
                    //line.StoreId = "TempVl";

                }
                PlanLines = ExtensionsNew.ConvertListToDataTable(model.Lines, PlanLines);

                parameters.Add("@Lines", PlanLines.AsTableValuedParameter(PlanLineTbl + "TableType"));

                //key = db.ExecuteScalar("USP_I_T_Inventory", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();

                //result.Success = true;
                //result.Message = key;
                //tran.Commit();

                //var exist = await checkExist(model.CompanyCode, model.HldCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.HldCode + " existed.";
                //    return result;
                //}
                key = _salesPlanRepository.GetScalar("USP_I_SalesPlan", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MSalesPlanHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                 
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id);
               
                var affectedRows = _salesPlanRepository.Execute("USP_D_SalesPlan", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetItems(string CompanyCode, string Id, string Name, string Keyword, DateTime? FromDate, DateTime? ToDate)
        {
            //string StoreId, string Type,
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", "");
                parameters.Add("Type", "");
                parameters.Add("Id", Id);
                //parameters.Add("Name", Name);
                parameters.Add("Keyword", Keyword);
                parameters.Add("FromDate", FromDate);
                parameters.Add("ToDate", ToDate);
                var data = await _salesPlanRepository.GetAllAsync($"USP_S_SalesPlan", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetItemById(string CompanyCode, string Id)
        {
            //string StoreId, string Type,
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", "");
                parameters.Add("Type", "");
                parameters.Add("Id", Id);
                //parameters.Add("Name", Name);
                parameters.Add("Keyword", "");
                parameters.Add("FromDate", null);
                parameters.Add("ToDate", null);
                var data = await _salesPlanRepository.GetAsync($"USP_S_SalesPlan", parameters, commandType: CommandType.StoredProcedure);
                if(data!=null)
                {
                    var lineParameters = new DynamicParameters();
                     
                    lineParameters.Add("CompanyCode", CompanyCode);
                    lineParameters.Add("Id", Id);
                    var dataLine = await _salesPlanLineRepository.GetAllAsync($"USP_S_SalesPlanLine", lineParameters, commandType: CommandType.StoredProcedure);
                    data.Lines = dataLine;
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


        public async Task<GenericResult> Update(MSalesPlanHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                var resultDelete = await Delete(model);
                if(resultDelete.Success)
                {
                    result = await Create(model, true);
                }   
                else
                {
                    result = resultDelete;
                }    
                 
                //parameters.Add("Id", model.Id);
                //parameters.Add("CompanyCode", model.CompanyCode);
                //parameters.Add("Name", model.Name);
                //parameters.Add("Description", model.Description);
                //parameters.Add("StoreId", model.StoreId);
                //parameters.Add("Type", model.Type);
                //parameters.Add("Value", model.Value);
                //parameters.Add("Target", model.Target);
                //parameters.Add("Percent", model.Percent);
                //parameters.Add("Priority", model.Priority);
                //parameters.Add("CustomF1", model.CustomF1);
                //parameters.Add("CustomF2", model.CustomF2);
                //parameters.Add("CustomF3", model.CustomF3);
                //parameters.Add("CustomF4", model.CustomF4);
                //parameters.Add("CustomF5", model.CustomF5);
                //parameters.Add("FromDate", model.FromDate);
                //parameters.Add("ToDate", model.ToDate);
                //parameters.Add("Status", model.Status);
                //parameters.Add("ModifiedBy", model.ModifiedBy);
                //var affectedRows = _salesPlanRepository.Update("USP_U_S_SalesPlan", parameters, commandType: CommandType.StoredProcedure);
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

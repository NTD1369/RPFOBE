
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
    public class VoucherTransactionService : IVoucherTransactionService
    {
        private readonly IGenericRepository<TVoucherTransaction> _voucherTransactionRepository;

        private readonly IMapper _mapper;
        public VoucherTransactionService(IGenericRepository<TVoucherTransaction> voucherTransactionRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _voucherTransactionRepository = voucherTransactionRepository;
            _mapper = mapper; 

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

        //public async Task<bool> checkExist(string CompanyCode, string UomCode)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("UomCode", UomCode);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _uomRepository.GetAsync("USP_S_M_UOM", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(TVoucherTransaction model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
  
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode); 
                parameters.Add("VoucherNo", model.VoucherNo);
                parameters.Add("VoucherValue", model.VoucherValue);
                parameters.Add("VoucherType", model.VoucherType);
                parameters.Add("IssueDate", model.IssueDate);
                parameters.Add("IssueTransId", model.IssueTransId);
                parameters.Add("RedeemDate", model.RedeemDate);
                parameters.Add("RedeemTransId", model.RedeemTransId);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _voucherTransactionRepository.Insert("USP_I_T_VoucherTransaction", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("ItemCode", "");
                parameters.Add("VoucherNo", "");
                parameters.Add("VoucherType", "");

                var data = await _voucherTransactionRepository.GetAllAsync($"USP_S_T_Voucher", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true; 
                result.Data = data.ToList();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Code = -1;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string ItemCode, string VoucherNo, string VoucherType)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                parameters.Add("VoucherNo", string.IsNullOrEmpty(VoucherNo) ? "" : VoucherNo); 
                parameters.Add("VoucherType", string.IsNullOrEmpty(VoucherType) ? "" : VoucherType);

                var data = await _voucherTransactionRepository.GetAsync($"USP_S_T_Voucher", parameters, commandType: CommandType.StoredProcedure);
                if(data==null)
                {
                    result.Success = false;
                    result.Code = -2;
                    result.Message = "Voucher number not existed";
                }    
                else
                { 
                    if(!string.IsNullOrEmpty(data.RedeemTransId))
                    {
                        result.Success = false;
                        result.Code = -3;
                        result.Message = "Voucher redeemed";
                    }    
                    else
                    {
                        result.Success = true;
                        result.Code = 0;
                        result.Data = data;
                    }    
                 
                }    
                
               
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Code = -1;
                result.Message = ex.Message;
            }
            return result;
        }
  
        public async Task<GenericResult> Update(TVoucherTransaction model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("VoucherNo", model.VoucherNo);
                parameters.Add("VoucherValue", model.VoucherValue);
                parameters.Add("VoucherType", model.VoucherType);
                parameters.Add("IssueDate", model.IssueDate);
                parameters.Add("IssueTransId", model.IssueTransId);
                parameters.Add("RedeemDate", model.RedeemDate);
                parameters.Add("RedeemTransId", model.RedeemTransId);
                var affectedRows = _voucherTransactionRepository.Update("USP_U_T_VoucherTransaction", parameters, commandType: CommandType.StoredProcedure);
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

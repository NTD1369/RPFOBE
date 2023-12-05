
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
    public class ShippingService : IShippingService
    {
        private readonly IGenericRepository<MShipping> _shipRepository;

        private readonly IMapper _mapper;
        public ShippingService(IGenericRepository<MShipping> shipRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _shipRepository = shipRepository;
            _mapper = mapper; 

        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<TaxResultViewModel> resultlist = new List<TaxResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Tax)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
                     
        //                TaxResultViewModel itemRs = new TaxResultViewModel();
        //                itemRs = _mapper.Map<TaxResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
                    
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
                 
        //    } 
        //    return result;
        //}

        //public async Task<bool> checkExist(string CompanyCode, string TaxCode)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("TaxCode", TaxCode);
   
        //    var affectedRows = await _shipRepository.GetAsync("USP_S_M_Shipping", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(MShipping model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ShippingCode", model.ShippingCode);
                parameters.Add("ShippingName", model.ShippingName);
                parameters.Add("Description", model.Description);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Status", model.Status);
                parameters.Add("DocDate", model.DocDate);
                parameters.Add("DocDueDate", model.DocDueDate);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CustomF6", model.CustomF6);
                parameters.Add("CustomF7", model.CustomF7);
                parameters.Add("CustomF8", model.CustomF8);
                parameters.Add("CustomF9", model.CustomF9);
                parameters.Add("CustomF10", model.CustomF10);
                parameters.Add("Amount1", model.Amount1);
                parameters.Add("Amount2", model.Amount2);
                parameters.Add("Amount3", model.Amount3);
                parameters.Add("Amount4", model.Amount4);
                parameters.Add("Amount5", model.Amount5);
                parameters.Add("LicensePlate", model.LicensePlate);
                parameters.Add("Driver", model.Driver);

                var affectedRows = _shipRepository.Update("USP_I_M_Shipping", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MShipping model)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode, string Key)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("ShippingCode", "");
                parameters.Add("Key", Key); 
                var data = await _shipRepository.GetAllAsync($"USP_S_M_Shipping", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _shipRepository.GetAllAsync($"select * from M_Tax with (nolock) where  CompanyCode = '{CompanyCode}'", null, commandType: CommandType.Text);
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
                parameters.Add("ShippingCode", Code);
                parameters.Add("Key", "");
                var data = await _shipRepository.GetAsync($"USP_S_M_Shipping", parameters, commandType: CommandType.StoredProcedure);
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

      
        
        public async Task<GenericResult> Update(MShipping model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", model.CompanyCode); 
                parameters.Add("ShippingCode", model.ShippingCode);
                parameters.Add("ShippingName", model.ShippingName);
                parameters.Add("Description", model.Description);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Status", model.Status);
                parameters.Add("DocDate", model.DocDate);
                parameters.Add("DocDueDate", model.DocDueDate);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CustomF6", model.CustomF6);
                parameters.Add("CustomF7", model.CustomF7);
                parameters.Add("CustomF8", model.CustomF8);
                parameters.Add("CustomF9", model.CustomF9);
                parameters.Add("CustomF10", model.CustomF10);
                parameters.Add("Amount1", model.Amount1);
                parameters.Add("Amount2", model.Amount2);
                parameters.Add("Amount3", model.Amount3);
                parameters.Add("Amount4", model.Amount4);
                parameters.Add("Amount5", model.Amount5);
                parameters.Add("LicensePlate", model.LicensePlate);
                parameters.Add("Driver", model.Driver);
                var affectedRows = _shipRepository.Update("USP_U_M_Shipping", parameters, commandType: CommandType.StoredProcedure);
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


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
    public class InvoiceInforService : IInvoiceInforService
    {
        private readonly IGenericRepository<MInvoiceInfor> _invoiceRepository;

        private readonly IMapper _mapper;
        public InvoiceInforService(IGenericRepository<MInvoiceInfor> invoiceRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _invoiceRepository = invoiceRepository;
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

        public async Task<bool> checkExist(MInvoiceInfor model)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", model.CompanyCode);
            parameters.Add("CustomerId", model.CustomerId);
            parameters.Add("Id", model.Id);
            parameters.Add("Phone", model.Phone);
            parameters.Add("Fax", model.Fax);
            parameters.Add("Email", model.Email);
            parameters.Add("TaxCode", model.TaxCode);
            parameters.Add("Name", model.Name);
            parameters.Add("Address", model.Address);
            parameters.Add("Remarks", "");
            parameters.Add("Status", "");
            var affectedRows = await _invoiceRepository.GetAsync("USP_S_M_InvoiceInfor", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MInvoiceInfor model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CustomerId", model.CustomerId);
                parameters.Add("CustomerName", model.CustomerName);
                parameters.Add("Phone", model.Phone);
                parameters.Add("Fax", model.Fax);
                parameters.Add("Email", model.Email);
                parameters.Add("TaxCode", model.TaxCode);
                parameters.Add("Name", model.Name);
                parameters.Add("Address", model.Address);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("Status", model.Status);
                var exist = await checkExist(model);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = "Model existed.";
                    return result;
                }
                var affectedRows = _invoiceRepository.Insert("USP_I_M_InvoiceInfor", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string CustomerId, string Phone, string Email, string TaxCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CustomerId", CustomerId);
                parameters.Add("Id", null);
                parameters.Add("Phone", Phone);
                parameters.Add("Fax", "");
                parameters.Add("Email", Email);
                parameters.Add("TaxCode", TaxCode);
                parameters.Add("Name", "");
                parameters.Add("Address", "");
                parameters.Add("Remarks", "");
                parameters.Add("Status", "");
                var data = await _invoiceRepository.GetAllAsync("USP_S_M_InvoiceInfor", parameters, commandType: CommandType.StoredProcedure);

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

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CustomerId", "");
                parameters.Add("Id", Code);
                parameters.Add("Phone", "");
                parameters.Add("Fax", "");
                parameters.Add("Email", "");
                parameters.Add("TaxCode", "");
                parameters.Add("Name", "");
                parameters.Add("Address", "");
                parameters.Add("Remarks", "");
                parameters.Add("Status", "");
                var data = await _invoiceRepository.GetAsync("USP_S_M_InvoiceInfor", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _invoiceRepository.GetAsync($"select * from M_Uom with (nolock) where UOMCode='{Code}'", null, commandType: CommandType.Text);
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




        public async Task<GenericResult> Update(MInvoiceInfor model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CustomerId", model.CustomerId);
                parameters.Add("CustomerName", model.CustomerName);
                parameters.Add("Id", model.Id);
                parameters.Add("Phone", model.Phone);
                parameters.Add("Fax", model.Fax);
                parameters.Add("Email", model.Email);
                parameters.Add("TaxCode", model.TaxCode);
                parameters.Add("Name", model.Name);
                parameters.Add("Address", model.Address);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("Status", model.Status);
                var affectedRows = _invoiceRepository.Update("USP_U_M_InvoiceInfor", parameters, commandType: CommandType.StoredProcedure);
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

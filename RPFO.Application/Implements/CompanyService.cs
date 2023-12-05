
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
    public class CompanyService : ICompanyService
    {
        private readonly IGenericRepository<MCompany> _companyRepository;

        private readonly IMapper _mapper;
        public CompanyService(IGenericRepository<MCompany> companyRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _companyRepository = companyRepository;
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

         
        public async Task<GenericResult> Create(MCompany model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 
                 
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CompanyName", model.CompanyName); 
                parameters.Add("ForeignName", model.ForeignName); 
                parameters.Add("ShortName", model.ShortName); 
                parameters.Add("Logo", model.Logo); 
                parameters.Add("Address", model.Address); 
                parameters.Add("Email", model.Email); 
                parameters.Add("Phone", model.Phone); 
                parameters.Add("TaxCode", model.TaxCode); 
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                if(!string.IsNullOrEmpty(model.CustomerF1)) parameters.Add("CustomerF1", model.CustomerF1); 
                if (!string.IsNullOrEmpty(model.CustomerF2)) parameters.Add("CustomerF2", model.CustomerF2);
                if (!string.IsNullOrEmpty(model.CustomerF3)) parameters.Add("CustomerF3", model.CustomerF3);
                if (!string.IsNullOrEmpty(model.CustomerF4)) parameters.Add("CustomerF4", model.CustomerF4);
                if (!string.IsNullOrEmpty(model.CustomerF5)) parameters.Add("CustomerF5", model.CustomerF5);
                if (!string.IsNullOrEmpty(model.CustomerF6)) parameters.Add("CustomerF6", model.CustomerF6);
                if (!string.IsNullOrEmpty(model.CustomerF7)) parameters.Add("CustomerF7", model.CustomerF7);
                if (!string.IsNullOrEmpty(model.CustomerF8)) parameters.Add("CustomerF8", model.CustomerF8);
                if (!string.IsNullOrEmpty(model.CustomerF9)) parameters.Add("CustomerF9", model.CustomerF9);
                if (!string.IsNullOrEmpty(model.CustomerF10)) parameters.Add("CustomerF10", model.CustomerF10);
                
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _companyRepository.Insert("USP_I_M_Company", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll()
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _companyRepository.GetAllAsync($"select * from M_Company with (nolock)", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByCode(string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _companyRepository.GetAsync($"select * from M_Company with (nolock) where CompanyCode='{Code}'", null, commandType: CommandType.Text);
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

        public Task<List<MCompany>> GetByItem(string Item)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> LogoUpdate(string CompanyCode,   string Url)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                var affectedRows = _companyRepository.Update($"update M_Company set Logo = N'{Url}' where CompanyCode = '{CompanyCode}'   ", parameters, commandType: CommandType.Text);
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


        public async Task<GenericResult> Update(MCompany model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CompanyName", model.CompanyName);
                parameters.Add("ForeignName", model.ForeignName);
                parameters.Add("ShortName", model.ShortName);
                parameters.Add("Logo", model.Logo);
                parameters.Add("Address", model.Address);
                parameters.Add("Email", model.Email);
                parameters.Add("Phone", model.Phone);
                parameters.Add("TaxCode", model.TaxCode);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _companyRepository.Update("USP_U_M_Company", parameters, commandType: CommandType.StoredProcedure);
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

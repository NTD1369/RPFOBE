
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
    public class BarcodeSetupService : IBarcodeSetupService
    {
        private readonly IGenericRepository<SBarcodeSetup> _barcodeRepository;

        private readonly IMapper _mapper;
        public BarcodeSetupService(IGenericRepository<SBarcodeSetup> barcodeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _barcodeRepository = barcodeRepository;
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

         
        public async Task<GenericResult> Create(SBarcodeSetup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                string newId = Guid.NewGuid().ToString();
                parameters.Add("Id", newId);
                parameters.Add("CompanyCode", model.CompanyCode); 
                parameters.Add("Name", model.Name); 
                parameters.Add("Prefix", model.Prefix);   
                parameters.Add("PrefixPosition", model.PrefixPosition);
                parameters.Add("PLUPosition", model.PLUPosition);
                parameters.Add("BarCodePosition", model.BarCodePosition);
                parameters.Add("QtyPosition", model.QtyPosition);
                parameters.Add("AmountPosition", model.AmountPosition);
                parameters.Add("CheckPosition", model.CheckPosition);
                parameters.Add("CheckCode", model.CheckCode);
                parameters.Add("CharSeparator", model.CharSeparator);
                parameters.Add("AmountCalculation", model.AmountCalculation);
                parameters.Add("AmountValue", model.AmountValue);
                parameters.Add("WeightCalculation", model.WeightCalculation);
                parameters.Add("WeightValue", model.WeightValue);
                parameters.Add("Status", model.Status);

                if (model.IsOrgPrice.HasValue)
                    parameters.Add("IsOrgPrice", model.IsOrgPrice);
                if (model.PrefixCheckLength.HasValue)
                    parameters.Add("PrefixCheckLength", model.PrefixCheckLength);
                if (!string.IsNullOrEmpty(model.CustomF1))
                    parameters.Add("CustomF1", model.CustomF1);
                if (!string.IsNullOrEmpty(model.CustomF2))
                    parameters.Add("CustomF2", model.CustomF2);
                if (!string.IsNullOrEmpty(model.CustomF3))
                    parameters.Add("CustomF3", model.CustomF3);
                if (!string.IsNullOrEmpty(model.CustomF4))
                    parameters.Add("CustomF4", model.CustomF4);
                if (!string.IsNullOrEmpty(model.CustomF5))
                    parameters.Add("CustomF5", model.CustomF5);

                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _barcodeRepository.Insert("USP_I_S_BarcodeSetup", parameters, commandType: CommandType.StoredProcedure);
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
         
        public async Task<GenericResult> Delete(SBarcodeSetup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
              

                var affectedRows = _barcodeRepository.Execute("USP_D_S_BarcodeSetup", parameters, commandType: CommandType.StoredProcedure);
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
        
        public async Task<GenericResult> GetAll(string CompanyCode, string Keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _barcodeRepository.GetAllAsync($"USP_S_S_BarcodeSetup '{CompanyCode}','' , N'{Keyword}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetById(string CompanyCode, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _barcodeRepository.GetAsync($"USP_S_S_BarcodeSetup '{CompanyCode}', '{Id}' , ''", null, commandType: CommandType.Text);
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

     

        public async Task<GenericResult> Update(SBarcodeSetup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Name", model.Name);
                parameters.Add("Prefix", model.Prefix);
                parameters.Add("PrefixPosition", model.PrefixPosition);
                parameters.Add("PLUPosition", model.PLUPosition);
                parameters.Add("BarCodePosition", model.BarCodePosition);
                parameters.Add("QtyPosition", model.QtyPosition);
                parameters.Add("AmountPosition", model.AmountPosition);
                parameters.Add("CheckPosition", model.CheckPosition);
                parameters.Add("CheckCode", model.CheckCode);
                parameters.Add("CharSeparator", model.CharSeparator);
                parameters.Add("AmountCalculation", model.AmountCalculation);
                parameters.Add("AmountValue", model.AmountValue);
                parameters.Add("WeightCalculation", model.WeightCalculation);
                parameters.Add("WeightValue", model.WeightValue);
                parameters.Add("Status", model.Status);
                if (model.IsOrgPrice.HasValue)
                    parameters.Add("IsOrgPrice", model.IsOrgPrice);
                if (model.PrefixCheckLength.HasValue)
                    parameters.Add("PrefixCheckLength", model.PrefixCheckLength);
                if (!string.IsNullOrEmpty(model.CustomF1))
                    parameters.Add("CustomF1", model.CustomF1);
                if (!string.IsNullOrEmpty(model.CustomF2))
                    parameters.Add("CustomF2", model.CustomF2);
                if (!string.IsNullOrEmpty(model.CustomF3))
                    parameters.Add("CustomF3", model.CustomF3);
                if (!string.IsNullOrEmpty(model.CustomF4))
                    parameters.Add("CustomF4", model.CustomF4);
                if (!string.IsNullOrEmpty(model.CustomF5))
                    parameters.Add("CustomF5", model.CustomF5);
                var affectedRows = _barcodeRepository.Update("USP_U_S_BarcodeSetup", parameters, commandType: CommandType.StoredProcedure);
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

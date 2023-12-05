
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
    public class HolidayService : IHolidayService
    {
        private readonly IGenericRepository<MHoliday> _holidayRepository;

        private readonly IMapper _mapper;
        public HolidayService(IGenericRepository<MHoliday> holidayRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _holidayRepository = holidayRepository;
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
            var affectedRows = await _holidayRepository.GetAsync("USP_S_M_Holiday", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MHoliday model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("HldCode", model.HldCode);
                parameters.Add("StrDate", model.StrDate); 
                parameters.Add("EndDate", model.EndDate);
                parameters.Add("Rmrks", model.Rmrks);
                parameters.Add("Status", model.Status);
                var exist = await checkExist(model.CompanyCode, model.HldCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.HldCode + " existed.";
                    return result;
                }
                var affectedRows = _holidayRepository.Insert("USP_I_M_Holiday", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _holidayRepository.GetAllAsync($"select * from M_Holiday with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
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
                var data = await _holidayRepository.GetAsync($"select * from M_Holiday with (nolock) where CompanyCode='{CompanyCode}' and HldCode='{Code}'", null, commandType: CommandType.Text);
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
 
       
        
        public async Task<GenericResult> Update(MHoliday model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("HldCode", model.HldCode);
                parameters.Add("StrDate", model.StrDate);
                parameters.Add("EndDate", model.EndDate);
                parameters.Add("Rmrks", model.Rmrks);
                parameters.Add("Status", model.Status);
                var affectedRows = _holidayRepository.Update("USP_U_M_Holiday", parameters, commandType: CommandType.StoredProcedure);
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

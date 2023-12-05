
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
    public class TimeFrameService : ITimeFrameService
    {
        private readonly IGenericRepository<MTimeFrame> _timeframeRepository;

        private readonly IMapper _mapper;
        public TimeFrameService(IGenericRepository<MTimeFrame> timeframeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _timeframeRepository = timeframeRepository;
            _mapper = mapper; 

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<TaxResultViewModel> resultlist = new List<TaxResultViewModel>();
            try
            {
                //foreach (var item in model.Tax)
                //{
                //    item.CreatedBy = model.CreatedBy;
                //    item.CompanyCode = model.CompanyCode;
                //    var itemResult = await Create(item);
                //    //if (itemResult.Success == false)
                //    //{
                //        TaxResultViewModel itemRs = new TaxResultViewModel();
                //        itemRs = _mapper.Map<TaxResultViewModel>(item);
                //        itemRs.Success = itemResult.Success;
                //        itemRs.Message = itemResult.Message;
                //    resultlist.Add(itemRs);
                //    //}
                //}
                result.Success = true;
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Data = failedlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string TimeFrameId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("TimeFrameId", TimeFrameId);
            parameters.Add("Status", "");
            var affectedRows = await _timeframeRepository.GetAsync("USP_S_M_TimeFrame", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MTimeFrame model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                if (!string.IsNullOrEmpty(model.StartTimeStr))
                {
                    model.StartTime = TimeSpan.Parse(model.StartTimeStr);
                }
                if (!string.IsNullOrEmpty(model.EndTimeStr))
                {
                    model.EndTime = TimeSpan.Parse(model.EndTimeStr);
                }
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("TimeFrameId", model.TimeFrameId);
                parameters.Add("StartTime", model.StartTime);
                parameters.Add("EndTime", model.EndTime);
                //parameters.Add("Duration", model.Duration);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);

                var exist = await checkExist(model.CompanyCode, model.TimeFrameId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.TimeFrameId + " existed.";
                    return result;
                }

                var affectedRows = _timeframeRepository.Insert("USP_I_M_TimeFrame", parameters, commandType: CommandType.StoredProcedure);
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

                var data = await _timeframeRepository.GetAllAsync($"select * from M_TimeFrame with (nolock) where CompanyCode = N'{CompanyCode}'", null, commandType: CommandType.Text);
                var resultMap = _mapper.Map<List<MTimeFrame>>(data);
                result.Success = true;
                result.Data = resultMap;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string companyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _timeframeRepository.GetAsync($"select * from M_TimeFrame with (nolock) where companyCode= '{companyCode}' and TaxCode='{Code}'", null, commandType: CommandType.Text);
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

     

        public async Task<PagedList<MTimeFrame>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _timeframeRepository.GetAllAsync($"select * from M_TimeFrame with (nolock)", null, commandType: CommandType.Text);
             
                return await PagedList<MTimeFrame>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<GenericResult> Update(MTimeFrame model)
        {
            GenericResult result = new GenericResult();
            try
            {
                if(!string.IsNullOrEmpty(model.StartTimeStr) )
                {
                    model.StartTime = TimeSpan.Parse(model.StartTimeStr);
                }
                if (!string.IsNullOrEmpty(model.EndTimeStr))
                {
                    model.EndTime = TimeSpan.Parse(model.EndTimeStr);
                }
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("TimeFrameId", model.TimeFrameId);
                parameters.Add("StartTime", model.StartTime);
                parameters.Add("EndTime", model.EndTime);
                //parameters.Add("Duration", model.Duration);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _timeframeRepository.Update("USP_U_M_TimeFrame", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetTimeFrame(string company, string code)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _timeframeRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", company);
                    parameters.Add("TimeFrameId", string.IsNullOrEmpty(code) ? "":code);
                    var data = await db.QueryAsync<TimeFrameViewModel>($"USP_S_TimeFrame", parameters, commandType: CommandType.StoredProcedure);
                    //return data.ToList();
                    result.Success = true;
                    result.Data = data.ToList();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
               
            }
            return result;

        }
    }

}

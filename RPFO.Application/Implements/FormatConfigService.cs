
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class FormatConfigService : IFormatConfigService
    {
        private readonly IGenericRepository<SFormatConfig> _formatRepository;

        private readonly IMapper _mapper;
        public FormatConfigService(IGenericRepository<SFormatConfig> formatRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _formatRepository = formatRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(SFormatConfig model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("FormatId", model.FormatId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("FormatName", model.FormatName);
                parameters.Add("SetupType", model.SetupType);
                parameters.Add("SetupCode", model.SetupCode);
                parameters.Add("DateFormat", model.DateFormat);
                parameters.Add("DecimalFormat", model.DecimalFormat);
                parameters.Add("ThousandFormat", model.ThousandFormat);
                parameters.Add("DecimalPlacesFormat", model.DecimalPlacesFormat);
                parameters.Add("QtyDecimalPlacesFormat", model.QtyDecimalPlacesFormat);
                parameters.Add("PerDecimalPlacesFormat", model.PerDecimalPlacesFormat);
                parameters.Add("RateDecimalPlacesFormat", model.RateDecimalPlacesFormat);
                parameters.Add("Status", model.Status);
                //parameters.Add("QuantityRoundingMethod", model.QuantityRoundingMethod);
                //parameters.Add("QuantityFormat", model.QuantityFormat);
                //parameters.Add("PointRoundingMethod", model.PointRoundingMethod);
                //parameters.Add("PointFormat", model.PointFormat);
                //parameters.Add("AmountRoundingMethod", model.AmountRoundingMethod);
                //parameters.Add("AmountFormat", model.AmountFormat);
                //parameters.Add("InventoryPercent", model.InventoryPercent);

                var affectedRows = _formatRepository.Insert("USP_I_S_FormatConfig", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _formatRepository.GetAllAsync($"select * from S_FormatConfig with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _formatRepository.GetAsync($"select t1.*  from S_FormatConfig t1 with(nolock)  left join M_Store  t2 with(nolock) on t1.FormatId = t2.FormatConfigId where t2.StoreId = '{StoreId}' and t1.CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
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
                var data = await _formatRepository.GetAsync($"select * from S_FormatConfig with (nolock)  where CompanyCode='{CompanyCode}' and ID ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<SFormatConfig>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _formatRepository.GetAllAsync($"select * from S_FormatConfig with (nolock) where FormatName like N'%{userParams.keyword}%'    or FormatId like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.FormatName);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.FormatId);
                }
                return await PagedList<SFormatConfig>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(SFormatConfig model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("FormatId", model.FormatId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("FormatName", model.FormatName);
                parameters.Add("SetupType", model.SetupType);
                parameters.Add("SetupCode", model.SetupCode);
                parameters.Add("DateFormat", model.DateFormat);
                parameters.Add("DecimalFormat", model.DecimalFormat);
                parameters.Add("ThousandFormat", model.ThousandFormat);
                parameters.Add("DecimalPlacesFormat", model.DecimalPlacesFormat);
                parameters.Add("QtyDecimalPlacesFormat", model.QtyDecimalPlacesFormat);
                parameters.Add("PerDecimalPlacesFormat", model.PerDecimalPlacesFormat);
                parameters.Add("RateDecimalPlacesFormat", model.RateDecimalPlacesFormat);
                parameters.Add("Status", model.Status);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                //parameters.Add("QuantityRoundingMethod", model.QuantityRoundingMethod);
                //parameters.Add("QuantityFormat", model.QuantityFormat);
                //parameters.Add("PointRoundingMethod", model.PointRoundingMethod);
                //parameters.Add("PointFormat", model.PointFormat);
                //parameters.Add("AmountRoundingMethod", model.AmountRoundingMethod);
                //parameters.Add("InventoryPercent", model.InventoryPercent);

                var affectedRows = _formatRepository.Insert("USP_U_S_FormatConfig", parameters, commandType: CommandType.StoredProcedure);
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

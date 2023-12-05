
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
    public class QuickAccessMenuService : IQuickAccessMenuService
    {
        private readonly IGenericRepository<SQuickAccessMenu> _menuRepository;

        private readonly IMapper _mapper;
        public QuickAccessMenuService(IGenericRepository<SQuickAccessMenu> menuRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(SQuickAccessMenu model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("Id", model.Id, DbType.String);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Url", model.Url);
                parameters.Add("Icon", model.Icon);
                parameters.Add("Image", model.Image);
                parameters.Add("OrderNum", model.OrderNum);
                parameters.Add("Status", model.Status);
                var affectedRows = _menuRepository.Insert("USP_I_S_QuickAccessMenu", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<SQuickAccessMenu>> GetAll()
        {
            try
            {
                var data = await _menuRepository.GetAllAsync($"select * from S_QuickAccessMenu with (nolock) ", null, commandType: CommandType.Text);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SQuickAccessMenu> GetByCode(string Code)
        {
            try
            {
                var data = await _menuRepository.GetAsync($"select * from S_QuickAccessMenu with (nolock)  where Id ='{Code}'", null, commandType: CommandType.Text);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<SQuickAccessMenu>> GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<SQuickAccessMenu>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _menuRepository.GetAllAsync($"select * from S_QuickAccessMenu with (nolock) where StoreGroupId like N'%{userParams.keyword}%' or CompanyCode like N'%{userParams.keyword}%'  or StoreGroupName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.StoreGroupName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.StoreGroupId);
                //}
                return await PagedList<SQuickAccessMenu>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GenericResult> ReOrder(string SourceId, string DesId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("SourceId", SourceId, DbType.String);
                parameters.Add("DesId", DesId); 
                var affectedRows = _menuRepository.Insert("USP_U_S_QuickAccessMenu_ReOrder", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<GenericResult> Update(SQuickAccessMenu model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("Id", model.Id, DbType.String);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Url", model.Url);
                parameters.Add("Icon", model.Icon);
                parameters.Add("Image", model.Image);
                parameters.Add("OrderNum", model.OrderNum);
                parameters.Add("Status", model.Status);
                var affectedRows = _menuRepository.Insert("USP_U_S_QuickAccessMenu", parameters, commandType: CommandType.StoredProcedure);
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

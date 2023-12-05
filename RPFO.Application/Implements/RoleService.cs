
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
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
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<MRole> _roleRepository;
        private readonly IMapper _mapper;
        public RoleService(IGenericRepository<MRole> roleRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(MRole model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("RoleId", Guid.NewGuid());
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("RoleName", model.RoleName);
                parameters.Add("Description", model.Description);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("DefaultScreen", model.DefaultScreen);
                var Id = _roleRepository.Insert("USP_I_M_Role", parameters, commandType: CommandType.StoredProcedure);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = model;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll()
        {
          
            //return data;
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _roleRepository.GetAllAsync("select * from M_Role with (nolock)", null, commandType: CommandType.Text);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> GetByCode(string Code)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _roleRepository.GetAsync($"select * from M_Role with (nolock) where RoleId = '{Code}'", null, commandType: CommandType.Text);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
           
            //return data;
        }

     
        public async Task<PagedList<MRole>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = "select * from M_Role with (nolock) where 1=1";

                if (!string.IsNullOrEmpty(userParams.keyword))
                {
                    query += $" and RoleId like N'%{userParams.keyword}%' or RoleName like N'%{userParams.keyword}%' ";
                }

                var data = await _roleRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.RoleName);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.RoleId);
                }
                return await PagedList<MRole>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
                throw new NotImplementedException();
            }
        }

        public async Task<GenericResult> Update(MRole model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("RoleId", model.RoleId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("RoleName", model.RoleName);
                parameters.Add("Description", model.Description);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("DefaultScreen", model.DefaultScreen);
               
                var Id = _roleRepository.Update("USP_U_M_Role", parameters, commandType: CommandType.StoredProcedure);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = model;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }
    } 

}

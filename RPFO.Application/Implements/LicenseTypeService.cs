
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
    public class LicenseTypeService : ILicenseTypeService
    {
        private readonly IGenericRepository<SLicenseType> _licenseTypeRepository;
        private readonly IMapper _mapper;
        public LicenseTypeService(IGenericRepository<SLicenseType> licenseTypeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _licenseTypeRepository = licenseTypeRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(SLicenseType model)
        {
            GenericResult rs = new GenericResult();
            try
            {

                var Id = await _licenseTypeRepository.GetConnection().InsertAsync<Guid, SLicenseType>(model);
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

        public async Task<List<SLicenseType>> GetAll()
        {
            var data = await _licenseTypeRepository.GetAllAsync("select * from M_Role with (nolock)", null, commandType: CommandType.Text);
            return data;
        }

        public Task<SLicenseType> GetByCode(string Code)
        {
            throw new NotImplementedException();
        }

        public Task<List<SLicenseType>> GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<SLicenseType>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = "select * from S_LicenseType with (nolock) where 1=1 ";

                if (!string.IsNullOrEmpty(userParams.keyword))
                {
                    query += $" and LicenseType like N'%{userParams.keyword}%' or Description like N'%{userParams.keyword}%' ";
                }

                var data = await _licenseTypeRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.LicenseType);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.LicenseType);
                }
                return await PagedList<SLicenseType>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
                throw new NotImplementedException();
            }
        }

        public async Task<GenericResult> Update(SLicenseType model)
        {
            GenericResult rs = new GenericResult();
            try
            {

                var Id = await _licenseTypeRepository.GetConnection().UpdateAsync<SLicenseType>(model);
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

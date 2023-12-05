
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
    public class LicenseService : ILicenseService
    {
        private readonly IGenericRepository<SLicense> _licenseRepository;
        private readonly IMapper _mapper;
        public LicenseService(IGenericRepository<SLicense> licenseRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _licenseRepository = licenseRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(SLicense model)
        {
            GenericResult rs = new GenericResult();
            try
            {
      
  
                 var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("LicenseId", model.LicenseId);
                parameters.Add("LicenseType", model.LicenseType);
                parameters.Add("LicenseDesc", model.LicenseDesc);
                parameters.Add("LicenseCode", model.LicenseCode);
                parameters.Add("ValidFrom", model.ValidFrom);
                parameters.Add("ValidTo", model.ValidTo);
                parameters.Add("Status", model.Status);
                parameters.Add("LicenseAmt", model.LicenseAmt);
                parameters.Add("LicenseRemain", model.LicenseRemain);
                parameters.Add("Token", model.Token);
                parameters.Add("CustomF1", model.CustomF1); 
                parameters.Add("CustomF2", model.CustomF2); 
                parameters.Add("CustomF3", model.CustomF3); 
                parameters.Add("CustomF4", model.CustomF4); 
                parameters.Add("CustomF5", model.CustomF5); 
                parameters.Add("CreatedBy", model.CreatedBy); 
                parameters.Add("NotifyShowOn", model.NotifyShowOn); 

                var affectedRows = _licenseRepository.GetScalar("USP_I_S_License", parameters, commandType: CommandType.StoredProcedure);

                //var items = await db.QueryAsync("USP_RPT_EOD_SummaryByDeparment", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                //var Id = await _licenseRepository.GetConnection().InsertAsync<string, SLicense>(model);
                //model.UserId = Id;
                rs.Success = true;
                rs.Message = affectedRows;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }

      
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult rs = new GenericResult();
            try
            { 
                var data = await _licenseRepository.GetAllAsync("select * from S_License with (nolock) where CompanyCode ='" + CompanyCode + "'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByLicense(string CompanyCode, string License)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _licenseRepository.GetAsync($"select * from S_License with (nolock) where CompanyCode = N'{CompanyCode}' and LicenseId =  N'{License}'", null, commandType: CommandType.Text);
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

       

        public async Task<GenericResult> Update(SLicense model)
        {
            GenericResult rs = new GenericResult();
            try
            {

                var Id = await _licenseRepository.GetConnection().UpdateAsync<SLicense>(model);
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
        }
    } 

}

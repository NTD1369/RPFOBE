
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LicensePlateController : ControllerBase
    {
        private readonly IConfiguration _config;
        ILicensePlateService _licensePlateService;
        private readonly ILogger<LicensePlateController> _logger;

        public LicensePlateController(IConfiguration config, ILicensePlateService licensePlateService, ILogger<LicensePlateController> logger)
        {
            _config = config;
            _licensePlateService = licensePlateService;
            _logger = logger;
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _licensePlateService.Import(models);
        }
        [HttpGet]
        [Route("CheckLicensePlate")]
        public async Task<GenericResult> CheckLicensePlate(string CompanyCode, string LicensePlate,decimal quantity )
        {
            return await _licensePlateService.CheckLicensePlate(CompanyCode,LicensePlate,quantity);
        }
        [HttpGet]
        //[AllowAnonymous]
        [Route("GetVoucherInfo")]
        public async Task<GenericResult> GetVoucherInfo(string CompanyCode,string StoreId, string key, string Type)
        {
            return await _licensePlateService.GetVoucherInfo(CompanyCode,StoreId, key,Type);
        }

        [HttpGet]
        //[AllowAnonymous]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string key)
        {
            return await _licensePlateService.GetAll(CompanyCode, key);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateRedeem")]
        public async Task<GenericResult> CreateRedeem( TSalesRedeemMenber model)
        {
            return await _licensePlateService.Redeem(model);
        }
        [HttpGet]
        //[AllowAnonymous]
        [Route("GetSerialInfo")]
        public async Task<GenericResult> GetSerialInfo(string CompanyCode, string StoreId, string key)
        {
            return await _licensePlateService.GetSerialInfo(CompanyCode, StoreId, key);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companycode, string id)
         {
            return await _licensePlateService.GetById(companycode, id);
        }
        [HttpGet]
        [Route("Search")]
        public async Task<GenericResult> Search(string companycode, string key)
        {
            return await _licensePlateService.Search(companycode, key);
        }
    }
}

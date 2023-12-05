using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
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
    public class GeneralSettingController : ControllerBase    
    {

        IGeneralSettingService _settingService;
        private readonly ILogger<GeneralSettingController> _logger;

        public GeneralSettingController(ILogger<GeneralSettingController> logger, IGeneralSettingService settingService)
        {
            _logger = logger;
            _settingService = settingService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _settingService.GetAll(CompanyCode);
        }
        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId)
        {
            return await _settingService.GetByStore(CompanyCode, StoreId);
        }
        [HttpGet]
        [Route("GetGeneralSettingByStore")]
        public async Task<GenericResult> GetGeneralSettingByStore(string CompanyCode, string StoreId)
        {
            return await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);
        }
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code)
        {
            return await _settingService.GetByCode(CompanyCode, StoreId,  Code);
        }
        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    return await _settingService.Import(model);
        //}
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SGeneralSetting model)
        {
            return await _settingService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SGeneralSetting model)
        {
            return await _settingService.Update(model);
        }
        [HttpPut]
        [Route("UpdateList")]
        public async Task<GenericResult> UpdateList(List<SGeneralSetting> models)
        {
            return await _settingService.UpdateList(models);
        }
    }
}

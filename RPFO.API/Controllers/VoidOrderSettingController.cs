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
    public class VoidOrderSettingController : ControllerBase    
    {

        IVoidOrderSettingService _voidOrderSettingService;
        private readonly ILogger<VoidOrderSettingController> _logger;

        public VoidOrderSettingController(ILogger<VoidOrderSettingController> logger, IVoidOrderSettingService voidOrderSettingService)
        {
            _logger = logger;
            _voidOrderSettingService = voidOrderSettingService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll()
        {
            return await _voidOrderSettingService.GetAll();
        }

        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string Type, string Code)
        {
            return await _voidOrderSettingService.GetByCode(Type, Code);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SVoidOrderSetting model)
        {
            return await _voidOrderSettingService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SVoidOrderSetting model)
        {
            return await _voidOrderSettingService.Update(model);
        }
         
    }
}

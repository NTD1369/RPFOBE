using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase    
    {

        IVersionService _versionService;
        private readonly ILogger<VersionController> _logger;

        public VersionController(ILogger<VersionController> logger, IVersionService versionService)
        {
            _logger = logger;
            _versionService = versionService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _versionService.GetAll(CompanyCode);
        }
      
        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string Version)
        {
            return await _versionService.GetByCode(CompanyCode, Version);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SVersion model)
        {
            return await _versionService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SVersion model)
        {
            return await _versionService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(SVersion model)
        {
            return await _versionService.Delete(model);
        }
    }
}

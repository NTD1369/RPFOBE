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
    public class ClientDisallowanceController : ControllerBase    
    {

        IClientDisallowanceService _clientDisallowanceService;
        private readonly ILogger<ClientDisallowanceController> _logger;

        public ClientDisallowanceController(ILogger<ClientDisallowanceController> logger, IClientDisallowanceService clientDisallowanceService)
        {
            _logger = logger;
            _clientDisallowanceService = clientDisallowanceService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Id, string CounterId, string Keyword)
        {
            return await _clientDisallowanceService.GetAll( CompanyCode,  StoreId,  Id,  CounterId,  Keyword);
        }
      
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Id)
        {
            return await _clientDisallowanceService.GetByCode(CompanyCode, StoreId, Id);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SClientDisallowance model)
        {
            return await _clientDisallowanceService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SClientDisallowance model)
        {
            return await _clientDisallowanceService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(SClientDisallowance model)
        {
            return await _clientDisallowanceService.Delete(model);
        }
    }
}

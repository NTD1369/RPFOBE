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
    public class CurrencyRoundingOffController : ControllerBase    
    {

        ICurrencyRoundingOffService _roundingOffService;
        private readonly ILogger<CurrencyRoundingOffController> _logger;

        public CurrencyRoundingOffController(ILogger<CurrencyRoundingOffController> logger, ICurrencyRoundingOffService roundingOffService)
        {
            _logger = logger;
            _roundingOffService = roundingOffService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId)
        {
            return await _roundingOffService.GetAll(CompanyCode,  StoreId);
        }
       
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id)
        {
            return await _roundingOffService.GetById( CompanyCode,  StoreId, Id);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SCurrencyRoundingOff model)
        {
            return await _roundingOffService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SCurrencyRoundingOff model)
        {
            return await _roundingOffService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(SCurrencyRoundingOff model)
        {
            return await _roundingOffService.Delete(model);
        }
    }
}

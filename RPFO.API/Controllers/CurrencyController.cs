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
    public class CurrencyController : ControllerBase    
    {

        ICurrencyService _currencyService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyService currencyService)
        {
            _logger = logger;
            _currencyService = currencyService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll()
        {
            return await _currencyService.GetAll();
        }
        [HttpGet]
        [Route("GetRoundingMethod")]
        public async Task<GenericResult> GetRoundingMethod()
        {
            return await _currencyService.GetRoundingMethod();
        }
        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string Code)
        {
            return await _currencyService.GetByCode( Code);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MCurrency model)
        {
            return await _currencyService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MCurrency model)
        {
            return await _currencyService.Update(model);
        }
       
    }
}

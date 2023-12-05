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
    public class StoreCurrencyController : ControllerBase    
    {

        IStoreCurrencyService _currencyService;
        private readonly ILogger<StoreCurrencyController> _logger;

        public StoreCurrencyController(ILogger<StoreCurrencyController> logger, IStoreCurrencyService currencyService)
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId)
        {
            return await _currencyService.GetAll(CompanyCode, StoreId);
        }
        
        [HttpGet]
        [Route("GetByStoreWExchangeRate")]
        public async Task<GenericResult> GetByStoreWExchangeRate(string CompanyCode, string StoreId)
        {
            return await _currencyService.GetByStoreWExchangeRate(CompanyCode, StoreId);
        }
        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code)
        {
            return await _currencyService.GetByCode(CompanyCode, StoreId, Code);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MStoreCurrency model)
        {
            return await _currencyService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MStoreCurrency model)
        {
            return await _currencyService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CompanyCode, string StoreId, string CurrencyCode)
        {
            return await _currencyService.Delete(CompanyCode, StoreId, CurrencyCode);
        }
    }
}

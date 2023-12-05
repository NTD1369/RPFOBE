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
    public class ExchangeRateController : ControllerBase    
    {

        IExchangeRateService _rateService;
        private readonly ILogger<ExchangeRateController> _logger;

        public ExchangeRateController(ILogger<ExchangeRateController> logger, IExchangeRateService rateService)
        {
            _logger = logger;
            _rateService = rateService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Currency, DateTime? From, DateTime? To)
        {
            return await _rateService.GetAll(CompanyCode, StoreId, Currency, From , To);
        }
        [HttpGet]
        [Route("GetExchangeRateIsNullByDate")]
        public async Task<GenericResult> GetExchangeRateIsNullByDate(string CompanyCode, string StoreId,  DateTime? Date )
        {
            return await _rateService.GetExchangeRateIsNullByDate(CompanyCode, StoreId,Date);
        } 


        [HttpGet]
        [Route("GetExchangeRateByStore")]
        public async Task<GenericResult> GetExchangeRateByStore(string CompanyCode, string StoreId, string Currency)
        {
            return await _rateService.GetExchangeRateByStore(CompanyCode, StoreId, Currency);
        }
        [HttpGet]
        [Route("GetByCurrency")]
        public async Task<GenericResult> GetByCurrency(string CompanyCode, string StoreId, string CurrencyCode)
        {
            return await _rateService.GetByCurrency(CompanyCode, StoreId, CurrencyCode);
        }
        [HttpGet]
        [Route("GetByDate")]
        public async Task<GenericResult> GetByDate(string CompanyCode, string StoreId, DateTime? Date)
        {
            return await _rateService.GetByDate(CompanyCode, StoreId, Date);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MExchangeRate model)
        {
            return await _rateService.Create(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MExchangeRate model)
        {
            return await _rateService.Delete(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MExchangeRate model)
        {
            return await _rateService.Update(model);
        }
       
    }
}

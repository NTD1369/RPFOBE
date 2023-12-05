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
    public class BankInController : ControllerBase    
    {

        IBankInService _bankInService;
        private readonly ILogger<BankInController> _logger;

        public BankInController(ILogger<BankInController> logger, IBankInService bankInService)
        {
            _logger = logger;
            _bankInService = bankInService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string DailyId)
        {
            return await _bankInService.GetAll( CompanyCode, StoreId, DailyId);
        }
        
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string DailyId, string Id)
        {
            return await _bankInService.GetByCode(CompanyCode, StoreId,  DailyId, Id);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TBankIn model)
        {
            return await _bankInService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TBankIn model)
        {
            return await _bankInService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(TBankIn model)
        {
            return await _bankInService.Delete(model);
        }

    }
}

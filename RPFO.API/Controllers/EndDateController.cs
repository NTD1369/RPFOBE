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
    public class EndDateController : ControllerBase    
    {

        IEndDateService _endDateService;
        private readonly ILogger<EndDateController> _logger;

        public EndDateController(ILogger<EndDateController> logger, IEndDateService endDateService)
        {
            _logger = logger;
            _endDateService = endDateService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companyCode, string storeId, string Top)
        {
            return await _endDateService.GetAll(companyCode, storeId, Top);
        }
        [HttpGet]
        [Route("GetEndDateList")]
        public async Task<GenericResult> GetEndDateList(string companyCode, string storeId)
        {
            return await _endDateService.GetEndDateList(companyCode, storeId);
        }
        [HttpGet]
        [Route("EndDateSummary")]
        public async Task<GenericResult> EndDateSummary(string companyCode, string storeId, string transdate)
        {
            return await _endDateService.EndDateSummary(companyCode, storeId, transdate);
        }
        [HttpGet]
        [Route("EndDateSummaryByDepartment")]
        public async Task<GenericResult> EndDateSummaryByDepartment(string companyCode, string storeId, string Userlogin, string FDate, string TDate, string dailyId)
        {
            return await _endDateService.EndDateSummaryByDepartment(companyCode, storeId, Userlogin, FDate, TDate, dailyId);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("SummaryPaymentPrint")]
        public async Task<GenericResult> EndDateSummaryPaymentPrint(string companyCode, string storeId, string dailyId)
        {
            return await _endDateService.EndDateSummaryPaymentPrint(companyCode, storeId, dailyId);
        }
         
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string companyCode, string storeId, string Code)
        {
            return await _endDateService.GetByCode(companyCode, storeId,  Code);
        } 
        
        [HttpGet]
        [Route("CheckCounterConnection")]
        public async Task<GenericResult> CheckCounterConnection(string companyCode, string storeId,  string transdate)
        {
            return await _endDateService.CheckCOUNTER_CONNECT(companyCode, storeId, transdate);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TEndDate model)
        {
            return await _endDateService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TEndDate model)
        {
            return await _endDateService.Update(model);
        }
         
    }
}

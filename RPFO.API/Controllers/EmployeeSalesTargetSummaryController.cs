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
    [Route("api/SalarySummary")]
    public class EmployeeSalesTargetSummaryController : ControllerBase    
    {

        IEmployeeSalesTargetSummaryService _summaryService;
        private readonly ILogger<EmployeeSalesTargetSummaryController> _logger;

        public EmployeeSalesTargetSummaryController(ILogger<EmployeeSalesTargetSummaryController> logger, IEmployeeSalesTargetSummaryService summaryService)
        {
            _logger = logger;
            _summaryService = summaryService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string Employee, string Position, DateTime? FromDate, DateTime? ToDate, string ViewType)
        {
            return await _summaryService.GetAll(CompanyCode, Employee, Position, FromDate, ToDate, ViewType);
        }
        
        
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TEmployeeSalesTargetSummary model)
        {
            return await _summaryService.Create(model);
        }

        [HttpPost]
        [Route("CreateByList")]
        public async Task<GenericResult> CreateByList(List<TEmployeeSalesTargetSummary> models)
        {
            return await _summaryService.CreateByList(models);
        }


        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TEmployeeSalesTargetSummary model)
        {
            return await _summaryService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(TEmployeeSalesTargetSummary model)
        {
            return await _summaryService.Delete(model);
        }

    }
}

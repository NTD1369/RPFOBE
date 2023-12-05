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
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesPlanController : ControllerBase    
    {

        ISalesPlanService _salesPlanService;
        private readonly ILogger<SalesPlanController> _logger;

        public SalesPlanController(ILogger<SalesPlanController> logger, ISalesPlanService salesPlanService)
        {
            _logger = logger;
            _salesPlanService = salesPlanService;
        }

        [HttpGet]
        [Route("GetItems")]
        public async Task<GenericResult> GetItems(string CompanyCode, string Id, string Name, string Keyword, DateTime? FromDate, DateTime? ToDate)
        {
            return await _salesPlanService.GetItems(CompanyCode, Id,  Name,  Keyword, FromDate, ToDate);
        }

        [HttpGet]
        [Route("GetItemById")]
        public async Task<GenericResult> GetItemById(string CompanyCode, string Id)
        {
            return await _salesPlanService.GetItemById(CompanyCode, Id);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MSalesPlanHeader model)
        {
            return await _salesPlanService.Create(model, false);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MSalesPlanHeader model)
        {
            return await _salesPlanService.Update(model);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MSalesPlanHeader model)
        {
            return await _salesPlanService.Delete(model);
        }

    }
}

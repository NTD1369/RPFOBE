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
    public class PriorityPriceListController : ControllerBase    
    {

        IPriorityPriceListService _prioService;
        private readonly ILogger<PriceListController> _logger;

        public PriorityPriceListController(ILogger<PriceListController> logger, IPriorityPriceListService prioService)
        {
            _logger = logger;
            _prioService = prioService;
        }
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string CusGrpId, string Id)
        {
            return await _prioService.GetAll(CompanyCode, CusGrpId, Id);
        }

       
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string Id)
        {
            return await _prioService.GetByCode(CompanyCode, Id);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPriorityPriceList model)
        {
            return await _prioService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPriorityPriceList model)
        {
            return await _prioService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MPriorityPriceList model)
        {
            return await _prioService.Delete(model);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DatasourceEditController : ControllerBase    
    {

        IDatasourceEditService _controlService;
        private readonly ILogger<DatasourceEditController> _logger;
        SemaphoreSlim _slim = new SemaphoreSlim(1);
        SemaphoreWaiter _semaphoreWaiter = new SemaphoreWaiter();
        public DatasourceEditController(ILogger<DatasourceEditController> logger, IDatasourceEditService controlService )
        {
            _logger = logger;
            _controlService = controlService;
            //_semaphoreWaiter = semaphoreWaiter;
        }
       
        [HttpGet]
        [Route("GetAll")]
       
        public async Task<IActionResult>  GetAll(string CompanyCode, string Datasource)
        {
            
                var response = await _controlService.GetAll(CompanyCode, Datasource);
                return Ok(response);
          
        }
        

        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string Datasource, string id)
        {
            return await _controlService.GetByCode(CompanyCode, Datasource, id);
        }
      
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SDatasourceEdit model)
        {
            return await _controlService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SDatasourceEdit model)
        {
            return await _controlService.Update(model);
        }
         
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CompanyCode, string Id)
        {
            return await _controlService.Delete(CompanyCode, Id);
        }

    }
}

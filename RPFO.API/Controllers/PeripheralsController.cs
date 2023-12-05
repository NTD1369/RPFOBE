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
    public class PeripheralsController : ControllerBase    
    {

        IPeripherallService _periService;
        private readonly ILogger<PeripheralsController> _logger;
        SemaphoreSlim _slim = new SemaphoreSlim(1);
        SemaphoreWaiter _semaphoreWaiter = new SemaphoreWaiter();
        public PeripheralsController(ILogger<PeripheralsController> logger, IPeripherallService periService)
        {
            _logger = logger;
            _periService = periService;
            //_semaphoreWaiter = semaphoreWaiter;
        }
        //[Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        //[AllowAnonymous]
        //Task<IEnumerable<MControl>>
        public async Task<IActionResult>  GetAll(string CompanyCode)
        {
           
                var response = await _periService.GetAll(CompanyCode);
                return Ok(response);
            
        }
       

        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string Code)
        {
            return await _periService.GetByCode(CompanyCode, Code);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPeripherals model)
        {
            return await _periService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPeripherals model)
        {
            return await _periService.Update(model);
        }
       
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MPeripherals model)
        {
            return await _periService.Delete(model);
        }

    }
}

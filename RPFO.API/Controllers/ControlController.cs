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
    public class ControlController : ControllerBase    
    {

        IControlService _controlService;
        private readonly ILogger<ControlController> _logger;
        SemaphoreSlim _slim = new SemaphoreSlim(1);
        SemaphoreWaiter _semaphoreWaiter = new SemaphoreWaiter();
        public ControlController(ILogger<ControlController> logger, IControlService controlService )
        {
            _logger = logger;
            _controlService = controlService;
            //_semaphoreWaiter = semaphoreWaiter;
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        //Task<IEnumerable<MControl>>
        public async Task<IActionResult>  GetAll(string CompanyCode)
        {
            //await _slim.WaitAsync();
            //var response = await _controlService.GetAll();
            //_slim.Release();
            //return Ok(response);
            //using (_semaphoreWaiter.WaitForLinuxDocker())
            //{
                // Call backend service here
                var response = await _controlService.GetAll(CompanyCode);
                return Ok(response);
            //}

        }
        [HttpGet]
        [Route("GetControlByFunction")]
        public async Task<GenericResult> GetControlByFunction(string CompanyCode, string FunctionId)
        {
            return await _controlService.GetControlByFunction(CompanyCode, FunctionId);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _controlService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id , string Function)
        {
            return await _controlService.GetByCode(CompanyCode, id, Function);
        }
        [HttpGet]
        [Route("GetByMer")]
        public async Task<IActionResult> GetByMer([FromQuery] UserParams userParams)
        {
            var data = await _controlService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MControl model)
        {
            return await _controlService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MControl model)
        {
            return await _controlService.Update(model);
        }
        [HttpPut]
        [Route("UpdateOrderNum")]
        public async Task<GenericResult> UpdateOrderNum(List<MControl> list)
        {
            return await _controlService.UpdateOrderNum(list);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _controlService.Delete(Id);
        }

    }
}

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
    public class TimeFrameController : ControllerBase    
    {

        ITimeFrameService _timeframeService;
        private readonly ILogger<TimeFrameController> _logger;

        public TimeFrameController(ILogger<TimeFrameController> logger, ITimeFrameService timeframeService)
        {
            _logger = logger;
            _timeframeService = timeframeService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companyCode)
        {
            return await _timeframeService.GetAll(companyCode);
        }
        [HttpGet]
        [Route("GetTimeFrame")]
        public async Task<GenericResult> GetTimeFrame(string companyCode, string timeframeId)
        {
            return await _timeframeService.GetTimeFrame(companyCode, timeframeId);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _timeframeService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companyCode, string id)
        {
            return await _timeframeService.GetByCode(companyCode, id);
        }
        
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _timeframeService.Import(model);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MTimeFrame model)
        {
            return await _timeframeService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MTimeFrame model)
        {
            return await _timeframeService.Update(model);
        }
        //[HttpDelete]
        //[Route("Delete")]
        //public async Task<GenericResult> Delete(string Id)
        //{
        //    return await _timeframeService.Delete(Id);
        //}

    }
}

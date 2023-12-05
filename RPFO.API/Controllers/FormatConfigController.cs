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
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FormatConfigController : ControllerBase    
    {

        IFormatConfigService _formatService;
        private readonly ILogger<FormatConfigController> _logger;

        public FormatConfigController(ILogger<FormatConfigController> logger, IFormatConfigService formatService)
        {
            _logger = logger;
            _formatService = formatService;
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _formatService.GetAll(CompanyCode);
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            try
            {
                var data = await _formatService.GetPagedList(userParams);

                Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return Ok(new GenericResult(false, ex.Message));
            }
          
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId)
        {
            return await _formatService.GetByStore(CompanyCode, StoreId);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _formatService.GetByCode(CompanyCode, id);
        }
        [HttpGet]
        [Route("GetByMer")]
        public async Task<IActionResult> GetByMer([FromQuery] UserParams userParams)
        {
            var data = await _formatService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SFormatConfig model)
        {
            return await _formatService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SFormatConfig model)
        {
            return await _formatService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _formatService.Delete(Id);
        }

    }
}

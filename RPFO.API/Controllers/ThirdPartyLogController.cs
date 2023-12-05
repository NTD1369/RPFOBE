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
    public class ThirdPartyLogController : ControllerBase    
    {

        IThirdPartyLogService _logService;
        private readonly ILogger<ThirdPartyLogController> _logger;

        public ThirdPartyLogController(ILogger<ThirdPartyLogController> logger, IThirdPartyLogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId)
        {
            return await _logService.GetAll(CompanyCode, StoreId);
        }

        //[HttpGet]
        //[Route("GetPagedList")]
        //public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        //{
        //    var data = await _uomService.GetPagedList(userParams);
            
        //    Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
        //    return Ok(data);
        //}
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string StoreId , string Id)
        {
            return await _logService.GetById(CompanyCode, StoreId, Id);
        }
        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    return await _uomService.Import(model);
        //}
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SThirdPartyLog model)
        {
            return await _logService.Create(model);
        }
        //[HttpPut]
        //[Route("Update")]
        //public async Task<GenericResult> Update(MUom model)
        //{
        //    return await _uomService.Update(model);
        //}
         
    }
}

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
    public class FunctionController : ControllerBase
    {
        IFunctionService _functionService;
        private readonly ILogger<FunctionController> _logger;

        public FunctionController(ILogger<FunctionController> logger, IFunctionService functionService)
        {
            _logger = logger;
            _functionService = functionService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            _logger.LogInformation("GET Pages.PrivacyModel called.");
            return await _functionService.GetAll(CompanyCode);

        }

        [HttpGet]
        [Route("GetFuntionMenuShow")]
        public async Task<GenericResult> GetFuntionMenuShow(string CompanyCode)
        {
            _logger.LogInformation("GET Pages.PrivacyModel called.");
            return await _functionService.GetFunctionMenuShow(CompanyCode);

        }
        [HttpGet]
        [Route("GetNodeAll")]
        public async Task<GenericResult> GetNodeAll(string CompanyCode)
        {
             
                return await _functionService.GetNodeAll(CompanyCode);
               
           
        }
        //[Cached(600)]
        [HttpGet]
        [Route("GetFunctionExpandAll")]
        public async Task<IActionResult> GetFunctionExpandAll(string CompanyCode, string userId)
        {
            try
            {
                var Data = await _functionService.GetFunctionExpandAll(CompanyCode, userId);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _functionService.GetByCode(CompanyCode, id);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _functionService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MFunction model)
        {
            return await _functionService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MFunction model)
        {
            return await _functionService.Update(model);
        }

        [HttpPut]
        [Route("UpdateMenuShow")]
        public async Task<GenericResult> UpdateMenuShow(List<MFunction> model)
        {
            return await _functionService.UpdateMenuShow(model);
        }
    }
}

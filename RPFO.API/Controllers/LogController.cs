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
    public class LogController : ControllerBase    
    {

        ILocalLogService _logService;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger, ILocalLogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string DbType, string User,  string CompanyCode, string StoreId, string Type, DateTime? FromDate, DateTime? ToDate)
        {
            return await _logService.GetAll(DbType, User, CompanyCode, StoreId, Type, FromDate, ToDate);
        }

        //[HttpGet]
        //[Route("GetPagedList")]
        //public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        //{
        //    var data = await _logService.GetPagedList(userParams);
            
        //    Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
        //    return Ok(data);
        //}
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(string DbType, SLog model)
        {
            return await _logService.Create( model, DbType);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(string DbType, SLog model)
        {
            return await _logService.Update( model, DbType);
        }
         
    }
}

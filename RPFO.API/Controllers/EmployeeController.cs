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
    public class EmployeeController : ControllerBase    
    {

        IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _employeeService.GetAll(CompanyCode);
        }
        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId, bool? CheckAvailable)
        {
            return await _employeeService.GetByStore(CompanyCode, StoreId, CheckAvailable);
        }
        [HttpGet]
        [Route("GetByUser")]
        public async Task<GenericResult> GetByUser(string CompanyCode, string UserCode)
        {
            return await _employeeService.GetByUser(CompanyCode, UserCode);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _employeeService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _employeeService.Import(models);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _employeeService.GetByCode(CompanyCode, id);
        }
        //[HttpGet]
        //[Route("GetPagedList")]
        //public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        //{
        //    var data = await _employeeService.GetPagedList(userParams);
        //    Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
        //    return Ok(data);
        //}
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MEmployee model)
        {
            return await _employeeService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MEmployee model)
        {
            return await _employeeService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _employeeService.Delete(Id);
        }

    }
}

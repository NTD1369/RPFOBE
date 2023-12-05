using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeSalaryController : ControllerBase    
    {

        IEmployeeSalaryService _salaryService;
        private readonly ILogger<EmployeeSalaryController> _logger;

        public EmployeeSalaryController(ILogger<EmployeeSalaryController> logger, IEmployeeSalaryService salaryService)
        {
            _logger = logger;
            _salaryService = salaryService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string Employee, string Id, DateTime? FromDate, DateTime? ToDate, string ViewType)
        {
            return await _salaryService.GetAll(CompanyCode, Employee, Id, FromDate, ToDate, ViewType);
        }
        
        
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MEmployeeSalary model)
        {
            return await _salaryService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MEmployeeSalary model)
        {
            return await _salaryService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MEmployeeSalary model)
        {
            return await _salaryService.Delete(model);
        }

    }
}

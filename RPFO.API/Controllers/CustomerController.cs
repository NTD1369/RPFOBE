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
    public class CustomerController : ControllerBase    
    {

        ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _customerService.Import(models);
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _customerService.GetAll(CompanyCode);
        }
        [HttpGet]
        [Route("GetByCompany")]
        //public async Task<IEnumerable<MCustomer>> GetByCompany(string companyCode, string Type)
        //{
        //    return await _customerService.GetByCompany(companyCode, Type);
        //}
        public async Task<GenericResult> GetByCompany(string CompanyCode, string Type, string CustomerGrpId, string CustomerId, string Status
           , string Keyword, string CustomerName, string CustomerRank, string Address, string Phone, DateTime? DOB,decimal? Display)
        {
            return await _customerService.GetByCompany(CompanyCode, Type, CustomerGrpId, CustomerId, Status
           , Keyword, CustomerName,  CustomerRank, Address, Phone, DOB,Display);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _customerService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companyCode, string id)
        {
            return await _customerService.GetByCode(companyCode, id);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MCustomer model)
        {
            return await _customerService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MCustomer model)
        {
            return await _customerService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _customerService.Delete(Id);
        }
    }
}

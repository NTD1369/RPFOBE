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
    public class PrepaidCardController : ControllerBase    
    {

        IPrepaidCardService _prepaidCardService;
        private readonly ILogger<CustomerController> _logger;

        public PrepaidCardController(ILogger<CustomerController> logger, IPrepaidCardService prepaidCardService)
        {
            _logger = logger;
            _prepaidCardService = prepaidCardService;
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _prepaidCardService.Import(models);
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult>  GetAll(string CompanyCode, string Status)
        {
            return await _prepaidCardService.GetAll(CompanyCode, Status);
        }
         
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companyCode, string code)
        {
            return await _prepaidCardService.GetByCode(companyCode, code);
        }
        [HttpGet]
        [Route("GetHistoryById")]
        public async Task<GenericResult> GetHistoryById(string companyCode, string code)
        {
            return await _prepaidCardService.GetHistoryByCode(companyCode, code);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPrepaidCard model)
        {
            return await _prepaidCardService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPrepaidCard model)
        {
            return await _prepaidCardService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _prepaidCardService.Delete(Id);
        }
    }
}

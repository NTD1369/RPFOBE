using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.OMSModels;
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
    public class TerminalPeripheralsController : ControllerBase
    {

        ITerminalPeripherallService _TerminalService;
        private readonly ILogger<TerminalPeripheralsController> _logger;

        public TerminalPeripheralsController(ILogger<TerminalPeripheralsController> logger, ITerminalPeripherallService TerminalService)
        {
            _logger = logger;
            _TerminalService = TerminalService;
        }
        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport models)
        //{
        //    return await _customerService.Import(models);
        //}
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string TerminalId, string IsSetup)
        {
            return await _TerminalService.GetAll(CompanyCode, StoreId,  TerminalId, IsSetup);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companyCode, string StoreId, string TerminalId, string PeripheralCode)
        {
            return await _TerminalService.GetByCode(companyCode, StoreId,  TerminalId,  PeripheralCode);
        }

        //[HttpGet]
        //[Route("GetByCounter")]
        //public async Task<GenericResult> GetByCounter(string companyCode, string StoreId, string CounterId)
        //{
        //    return await _bankTerminalService.GetByCounter(companyCode, StoreId, CounterId);
        //}
        [HttpPost]
        [Route("Apply")]
        public async Task<GenericResult> Apply(MTerminalPeripherals model)
        {
            return await _TerminalService.Apply(model);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MTerminalPeripherals model)
        {
            return await _TerminalService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MTerminalPeripherals model)
        {
            return await _TerminalService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MTerminalPeripherals model)
        {
            return await _TerminalService.Delete(model);
        }

      
    }
}

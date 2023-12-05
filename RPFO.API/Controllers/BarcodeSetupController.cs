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
    public class BarcodeSetupController : ControllerBase    
    {

        IBarcodeSetupService _barcodeService;
        private readonly ILogger<BarcodeSetupController> _logger;

        public BarcodeSetupController(ILogger<BarcodeSetupController> logger, IBarcodeSetupService barcodeService)
        {
            _logger = logger;
            _barcodeService = barcodeService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string Keyword)
        {
            return await _barcodeService.GetAll(CompanyCode, Keyword);
        }
        
        
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string Id)
        {
            return await _barcodeService.GetById(CompanyCode, Id);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SBarcodeSetup model)
        {
            return await _barcodeService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SBarcodeSetup model)
        {
            return await _barcodeService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(SBarcodeSetup model)
        {
            return await _barcodeService.Delete(model);
        }
    }
}

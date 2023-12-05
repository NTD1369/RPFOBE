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
    public class ShippingController : ControllerBase    
    {

        IShippingService _shippingService;
        private readonly ILogger<ShippingController> _logger;

        public ShippingController(ILogger<ShippingController> logger, IShippingService shippingService)
        {
            _logger = logger;
            _shippingService = shippingService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string Keyword)
        {
            return await _shippingService.GetAll(CompanyCode, Keyword);
        }

         
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode,string Code)
        {
            return await _shippingService.GetByCode(CompanyCode, Code);
        }
        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    return await _shippingService.Import(model);
        //}
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MShipping model)
        {
            return await _shippingService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MShipping model)
        {
            return await _shippingService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MShipping model)
        {
            return await _shippingService.Delete(model);
        }
    }
}

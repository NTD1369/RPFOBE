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
    public class ShippingDivisionController : ControllerBase    
    {

        IShippingDivisionService _shippingService;
        private readonly ILogger<ShippingDivisionController> _logger;

        public ShippingDivisionController(ILogger<ShippingDivisionController> logger, IShippingDivisionService shippingService)
        {
            _logger = logger;
            _shippingService = shippingService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string FromDate, string ToDate)
        {
            return await _shippingService.GetAll(CompanyCode, FromDate, ToDate);
        }
        [HttpGet]
        [Route("GetDivisionToShip")]
        public async Task<GenericResult> GetDivisionToShip(string CompanyCode, string id, string date)
        {
            return await _shippingService.GetDivisionToShip(CompanyCode, id, date);
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
        public async Task<GenericResult> Create(TShippingDivisionHeader model)
        {
            return await _shippingService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TShippingDivisionHeader model)
        {
            return await _shippingService.Update(model);
        }
        //[HttpPost]
        //[Route("Delete")]
        //public async Task<GenericResult> Delete(TShippingDivisionHeader model)
        //{
        //    return await _shippingService.Delete(model);
        //}
    }
}

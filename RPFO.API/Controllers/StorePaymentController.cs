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
    public class StorePaymentController : ControllerBase    
    {

        IStorePaymentService _storePaymentService;
        private readonly ILogger<StorePaymentController> _logger;

        public StorePaymentController(ILogger<StorePaymentController> logger, IStorePaymentService storePaymentService)
        {
            _logger = logger;
            _storePaymentService = storePaymentService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _storePaymentService.GetAll(CompanyCode);
        }
        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId, string CounterId, bool? IsSetup)
        {

            return await _storePaymentService.GetByStore(CompanyCode, StoreId, CounterId, IsSetup);

           
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _storePaymentService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _storePaymentService.GetByCode(CompanyCode, id);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _storePaymentService.Import(model);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MStorePayment model)
        {
            return await _storePaymentService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MStorePayment model)
        {
            return await _storePaymentService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
      
        public async Task<GenericResult> Delete(string StoreId, string PaymentCode)
        {
            return await _storePaymentService.Delete(StoreId, PaymentCode);
        }

    }
}

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
    public class PaymentMethodController : ControllerBase
    {

        IPaymentMethodService _paymentService;
        private readonly ILogger<PaymentMethodController> _logger;

        public PaymentMethodController(ILogger<PaymentMethodController> logger, IPaymentMethodService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _paymentService.GetAll(CompanyCode);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _paymentService.GetPagedList(userParams);

            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string StoreId, string Code)
        {
            return await _paymentService.GetByCode(CompanyCode, StoreId, Code);
        }
        //[Cached(600)]
        [HttpGet]
        [Route("GetByStore")]
        public async Task<IActionResult> GetByStore(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _paymentService.GetByStore(CompanyCode, StoreId);

                return Ok(data);
            }
            catch (Exception ex)
            {

                return Ok(new { status = "failed", error = ex.Message });
            }
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetPaymentType")]
        public async Task<GenericResult> GetPaymentType()
        {

            var data = await _paymentService.GetPaymentType();
            return data;

        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPaymentMethod model)
        {
            return await _paymentService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPaymentMethod model)
        {
            return await _paymentService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _paymentService.Delete(Id);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _paymentService.Import(model);
        }

        [HttpGet]
        [Route("MwiGet")]
        public async Task<IEnumerable<MPaymentMethod>> GetMPayments(string companyCode, string paymentCode, string stored, string status)
        {
            return await _paymentService.GetMPayments(companyCode, paymentCode, stored, status);
        }
    }
}

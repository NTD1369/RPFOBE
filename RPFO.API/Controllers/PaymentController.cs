using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase    
    {

        IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        SemaphoreSlim _slim = new SemaphoreSlim(1);
        SemaphoreWaiter _semaphoreWaiter = new SemaphoreWaiter();
        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
            //_semaphoreWaiter = semaphoreWaiter;
        }
        
        [HttpGet]
        [Route("GetAll")]
        //Task<IEnumerable<MControl>>
        public async Task<GenericResult>  GetAll(string CompanyCode, string CusId, string FromDate, string ToDate, string Status, string top)
        {
              return await _paymentService.GetAll( CompanyCode,  CusId,  FromDate,  ToDate, Status,  top); 
        }
         
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id  )
        {
            return await _paymentService.GetByCode(CompanyCode, id );
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TPaymentHeader model)
        {
            return await _paymentService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TPaymentHeader model)
        {
            return await _paymentService.Update(model);
        }
       
        //[HttpDelete]
        //[Route("Delete")]
        //public async Task<GenericResult> Delete(string CompanyCode, string Id)
        //{
        //    return await _paymentService.Delete(CompanyCode, Id);
        //}

    }
}

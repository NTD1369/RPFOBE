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
    public class InvoiceInforController : ControllerBase    
    {

        IInvoiceInforService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceInforController(ILogger<InvoiceController> logger, IInvoiceInforService invoiceService)
        {
            _logger = logger;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string CustomerId, string Phone, string Email, string TaxCode)
        {
            return await _invoiceService.GetAll( CompanyCode,  CustomerId,  Phone,  Email, TaxCode);
        }

        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            return await _invoiceService.GetByCode(CompanyCode, Code);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MInvoiceInfor model)
        {
            return await _invoiceService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MInvoiceInfor model)
        {
            return await _invoiceService.Update(model);
        }
         
    }
}

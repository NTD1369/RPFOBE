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
    public class DeliveryInforController : ControllerBase    
    {

        IDeliveryInforService _deliverService;
        private readonly ILogger<InvoiceController> _logger;

        public DeliveryInforController(ILogger<InvoiceController> logger, IDeliveryInforService deliverService)
        {
            _logger = logger;
            _deliverService = deliverService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string CustomerId, string Phone, string Email, string TaxCode)
        {
            return await _deliverService.GetAll( CompanyCode,  CustomerId,  Phone,  Email, TaxCode);
        }

        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            return await _deliverService.GetByCode(CompanyCode, Code);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MDeliveryInfor model)
        {
            return await _deliverService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MDeliveryInfor model)
        {
            return await _deliverService.Update(model);
        }
         
    }
}

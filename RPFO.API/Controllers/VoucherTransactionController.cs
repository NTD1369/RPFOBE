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
    public class VoucherTransactionController : ControllerBase    
    {

        IVoucherTransactionService _voucherService;
        private readonly ILogger<VoucherTransactionController> _logger;

        public VoucherTransactionController(ILogger<VoucherTransactionController> logger, IVoucherTransactionService voucherService)
        {
            _logger = logger;
            _voucherService = voucherService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _voucherService.GetAll(CompanyCode);
        }
  
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string ItemCode, string VoucherNo, string VoucherType)
        {
            return await _voucherService.GetByCode(CompanyCode,  ItemCode,  VoucherNo, VoucherType);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TVoucherTransaction model)
        {
            return await _voucherService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TVoucherTransaction model)
        {
            return await _voucherService.Update(model);
        }
         
    }
}

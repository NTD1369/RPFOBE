using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
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
    public class ItemRejectPaymentController : ControllerBase    
    {

        IItemRejectPaymentService _rejectService;
        private readonly ILogger<ItemRejectPaymentController> _logger;

        public ItemRejectPaymentController(ILogger<ItemRejectPaymentController> logger, IItemRejectPaymentService rejectService)
        {
            _logger = logger;
            _rejectService = rejectService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string ItemCode, string Status)
        {
            return await _rejectService.GetAll(CompanyCode,  ItemCode,  Status);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MItemRejectPayment model)
        {
            return await _rejectService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MItemRejectPayment model)
        {
            return await _rejectService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MItemRejectPayment model)
        {
            return await _rejectService.Delete(model);
        }
        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    return await _itemUomService.Import(model);
        //}
    }
}

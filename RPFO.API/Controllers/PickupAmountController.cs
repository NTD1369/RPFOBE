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
    public class PickupAmountController : ControllerBase    
    {

        IPickupAmountService _pickupService;
        private readonly ILogger<PickupAmountController> _logger;
        SemaphoreSlim _slim = new SemaphoreSlim(1);
        SemaphoreWaiter _semaphoreWaiter = new SemaphoreWaiter();
        public PickupAmountController(ILogger<PickupAmountController> logger, IPickupAmountService pickupService)
        {
            _logger = logger;
            _pickupService = pickupService;
            //_semaphoreWaiter = semaphoreWaiter;
        }
        //[Cached(600)]
        //[HttpGet]
        //[Route("GetAll")]
        ////Task<IEnumerable<MControl>>
        //public async Task<IActionResult>  GetAll(string CompanyCode)
        //{ 
        //    var response = await _pickupService.GetAll(CompanyCode);
        //    return Ok(response); 
        //}
        
        [HttpGet]
        [Route("GetItems")]
        public async Task<GenericResult> GetItems(string CompanyCode, string StoreId, string DailyId, string CounterId, string ShiftId, string PickupBy, string CreatedBy, DateTime? FDate, DateTime? TDate, string Id)
        {
            return await _pickupService.GetItems(CompanyCode, StoreId, DailyId, CounterId, ShiftId, PickupBy, CreatedBy, FDate , TDate, Id);
        }
        
        [HttpGet]
        [Route("GetItem")]
        public async Task<GenericResult> GetItem(string CompanyCode, string StoreId, string DailyId, string CounterId, string ShiftId, string Id, string NumOfList)
        {
            return await _pickupService.GetItem(CompanyCode, StoreId, DailyId, CounterId, ShiftId,   Id, NumOfList);
        }
 
        [HttpGet]
        [Route("GetPickupAmountLst")]
        public async Task<GenericResult> GetPickupAmountLst(string CompanyCode, string StoreId, string DailyId, string ShiftId, string IsSales, DateTime? FDate, DateTime? TDate)
        {
            return await _pickupService.GetPickupAmountLst(CompanyCode, StoreId, DailyId, ShiftId, IsSales, FDate, TDate);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TPickupAmount model)
        {
            return await _pickupService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TPickupAmount model)
        {
            return await _pickupService.Update(model);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(TPickupAmount model)
        {
            return await _pickupService.Delete(model);
        }

    }
}

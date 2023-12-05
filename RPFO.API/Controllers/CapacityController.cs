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
    public class CapacityController : ControllerBase    
    {

        ICapacityService _capacityService;
        private readonly ILogger<CapacityController> _logger;

        public CapacityController(ILogger<CapacityController> logger, ICapacityService capacityService)
        {
            _logger = logger;
            _capacityService = capacityService;
        }

        [HttpGet]
        [Route("GetCapacity")]
        public async Task<GenericResult> GetCapacity(string CompanyCode, DateTime TransDate, int? Quantity ,string StoreId, string StoreAreaId, string TimeFrameId)
        { 
            var data =  await _capacityService.GetCapacity(CompanyCode, TransDate, Quantity,  StoreId, StoreAreaId, TimeFrameId); 
            return data;
        }
        [HttpGet]
        [Route("GetCapacityFromTo")]
        public async Task<GenericResult> GetCapacityFromTo(string CompanyCode, DateTime FromDate, DateTime ToDate, int? Quantity ,string StoreId, string StoreAreaId, string TimeFrameId)
        { 
            var data =  await _capacityService.GetCapacityFromTo(CompanyCode, FromDate, ToDate, Quantity,  StoreId, StoreAreaId, TimeFrameId); 
            return data;
        }
        [HttpGet]
        [Route("GetCapacityByStore")]
        public async Task<GenericResult> GetCapacityByStore(string CompanyCode, DateTime TransDate, int? Quantity, string StoreId)
        {
            var data = await _capacityService.GetCapacityByStore(CompanyCode, TransDate, Quantity, StoreId);
            return data;
        }
        [HttpGet]
        [Route("GetCapacityAreaStore")]
        public async Task<GenericResult> GetCapacityAreaStore(string CompanyCode,  string StoreId)
        {
            var data = await _capacityService.GetCapacityAreaStore(CompanyCode,   StoreId);
            return data;
        }
        [HttpGet]
        [Route("GetCapacityByAreaStore")]
        public async Task<GenericResult> GetCapacityByAreaStore(string CompanyCode, string StoreId, string StoreAreaId)
        {
            var data = await _capacityService.GetCapacityByAreaStore(CompanyCode, StoreId, StoreAreaId);
            return data;
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MStoreCapacity capacity)
        {
            var updatebasket = await _capacityService.Create(capacity);
            return updatebasket;
        }

        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MStoreCapacity capacity)
        {
            var updatebasket = await _capacityService.Update(capacity);
            return updatebasket;
        }
 


        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MStoreCapacity capacity)
        {
            var updatebasket = await _capacityService.Delete(capacity);
            return updatebasket;
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _capacityService.Import(models);
        }
    }
}

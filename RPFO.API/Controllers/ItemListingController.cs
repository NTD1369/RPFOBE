using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemListingController : ControllerBase    
    {

        IItemListingService _listingService;
        private readonly ILogger<ItemListingController> _logger;

        public ItemListingController(ILogger<ItemListingController> logger, IItemListingService listingService)
        {
            _logger = logger;
            _listingService = listingService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode)
        {
            return await _listingService.GetAll(CompanyCode, StoreId, ItemCode);
        }

        [HttpGet]
        [Route("GetItemListingStore")]
        public async Task<GenericResult> GetItemListingStore(string CompanyCode, string ItemCode, string UserCode)
        {
            return await _listingService.GetItemListingStore(CompanyCode,   ItemCode, UserCode);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MItemStoreListing model)
        {
            return await _listingService.Create(model);
        } 
        
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MItemStoreListing model)
        {
            return await _listingService.Delete(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MItemStoreListing model)
        {
            return await _listingService.Update(model);
        }
       
    }
}

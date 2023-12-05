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
    public class ItemStorageController : ControllerBase    
    {

        IItemStorageService _itemStorageService;
        private readonly ILogger<ItemStorageController> _logger;

        public ItemStorageController(ILogger<ItemStorageController> logger, IItemStorageService itemStorageService)
        {
            _logger = logger;
            _itemStorageService = itemStorageService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companyCode, string storeiI, string slocId, string itemCode, string uomCode)
        {
            return await _itemStorageService.GetAll(companyCode, storeiI, slocId, itemCode, uomCode);
        }
 
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string companyCode, string storeiI, string slocId, string itemCode, string uomCode)
        {
            return await _itemStorageService.GetByCode(companyCode, storeiI, slocId, itemCode, uomCode);
        }
        
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _itemStorageService.Import(model);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TItemStorage model)
        {
            return await _itemStorageService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TItemStorage model)
        {
            return await _itemStorageService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _itemStorageService.Delete(Id);
        }

    }
}

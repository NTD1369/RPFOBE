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
    public class ItemVariantController : ControllerBase    
    {

        IItemVariantService _itemVariantService;
        private readonly ILogger<ItemVariantController> _logger;

        public ItemVariantController(ILogger<ItemVariantController> logger, IItemVariantService itemVariantService)
        {
            _logger = logger;
            _itemVariantService = itemVariantService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(DateTime? FromDate, DateTime? ToDate, string Status, string Keyword)
        {
            return await _itemVariantService.GetAll(FromDate,ToDate,  Status,  Keyword);
        }
        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string Code)
        {
            return await _itemVariantService.GetByCode(Code);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MItemVariant model)
        {
            return await _itemVariantService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MItemVariant model)
        {
            return await _itemVariantService.Update(model);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MItemVariant model)
        {
            return await _itemVariantService.Delete(model);
        }
    }
}

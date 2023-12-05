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
    public class StoreClientController : ControllerBase    
    {

        IStoreClientService _clientService;
        private readonly ILogger<StoreClientController> _logger;

        public StoreClientController(ILogger<StoreClientController> logger, IStoreClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, DateTime? From, DateTime? To)
        {
            return await _clientService.GetAll(CompanyCode, StoreId,   From , To);
        }
        [HttpGet]
        [Route("GetCounterSalesInDay")]
        public async Task<GenericResult> GetCounterSalesInDay(string CompanyCode, string StoreId, DateTime? Date)
        {
            return await _clientService.GetCounterSalesInDay(CompanyCode, StoreId, Date);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id, string LocalIP, string PublicIP)
        {
            return await _clientService.GetById( CompanyCode, StoreId, Id,  LocalIP,  PublicIP);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SStoreClient model)
        {
            return await _clientService.Create(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(SStoreClient model)
        {
            return await _clientService.Delete(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SStoreClient model)
        {
            return await _clientService.Update(model);
        } 
        [HttpPut]
        [Route("UpdateByPublicId")]
        public async Task<GenericResult> UpdateByPublicId(SStoreClient model)
        {
            return await _clientService.UpdateByPublicId(model);
        }
        
    }
}

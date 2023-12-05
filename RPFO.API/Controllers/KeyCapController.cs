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
    public class KeyCapController : ControllerBase    
    {

        IKeyCapService _keycapService;
        private readonly ILogger<CurrencyController> _logger;

        public KeyCapController(ILogger<CurrencyController> logger, IKeyCapService keycapService)
        {
            _logger = logger;
            _keycapService = keycapService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll()
        {
            return await _keycapService.GetAll();
        }
        
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetByCode(string Id)
        {
            return await _keycapService.GetByCode(Id);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MKeyCap model)
        {
            return await _keycapService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MKeyCap model)
        {
            return await _keycapService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _keycapService.Delete(Id);
        }

    }
}

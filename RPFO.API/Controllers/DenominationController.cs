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
    public class DenominationController : ControllerBase    
    {

        IDenominationService _service;
        private readonly ILogger<DenominationController> _logger;

        public DenominationController(ILogger<DenominationController> logger, IDenominationService service)
        {
            _logger = logger;
            _service = service;
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CurrencyCode)
        {
            return await _service.GetAll(CurrencyCode);
        }
        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string Code)
        {
            return await _service.GetByCode(Code);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MDenomination model)
        {
            return await _service.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MDenomination model)
        {
            return await _service.Update(model);
        }
       
    }
}

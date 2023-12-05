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
    public class SalesTypeController : ControllerBase    
    {

        ISaleTypeService _salesTypeService;
        private readonly ILogger<SalesTypeController> _logger;

        public SalesTypeController(ILogger<SalesTypeController> logger, ISaleTypeService salesTypeService)
        {
            _logger = logger;
            _salesTypeService = salesTypeService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IEnumerable<SSalesType>> GetAll()
        {
            return await _salesTypeService.GetAll();
        }

        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<SSalesType> GetByCode(string Code)
        {
            return await _salesTypeService.GetByCode(Code);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SSalesType model)
        {
            return await _salesTypeService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SSalesType model)
        {
            return await _salesTypeService.Update(model);
        }
         
    }
}

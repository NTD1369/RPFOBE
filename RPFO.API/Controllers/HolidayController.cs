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
    public class HolidayController : ControllerBase    
    {

        IHolidayService _holidayService;
        private readonly ILogger<HolidayController> _logger;

        public HolidayController(ILogger<HolidayController> logger, IHolidayService holidayService)
        {
            _logger = logger;
            _holidayService = holidayService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companyCode)
        {
            return await _holidayService.GetAll(companyCode);
        }

        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string companyCode, string Code)
        {
            return await _holidayService.GetByCode(companyCode, Code);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MHoliday model)
        {
            return await _holidayService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MHoliday model)
        {
            return await _holidayService.Update(model);
        }
         
    }
}

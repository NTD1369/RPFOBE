using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Implements;
using RPFO.Application.Interfaces;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTimekeepingController : ControllerBase
    {

        IEmployeeTimekeepingService _employeeTimekeepingService;
        private readonly ILogger<CompanyController> _logger;
        private string LogPath;

        public EmployeeTimekeepingController(ILogger<CompanyController> logger, IEmployeeTimekeepingService employeeTimekeepingService )
        {
            _logger = logger;
            _employeeTimekeepingService = employeeTimekeepingService;
        }

        [HttpGet]
        [Route("GetEmployeeInfor")]
        public async Task<GenericResult> GetEmployeeInfor(string access_token, string code)
        {
            GenericResult result = new GenericResult();
            try
            {
                return await _employeeTimekeepingService.GetEmployeeInfor(access_token, code);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return result;
            }
        }

    }
}

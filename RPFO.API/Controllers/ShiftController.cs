using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftController : ControllerBase    
    {

        IShiftService _shiftService;
        private readonly ILogger<ShiftController> _logger;

        public ShiftController(ILogger<ShiftController> logger, IShiftService shiftService)
        {
            _logger = logger;
            _shiftService = shiftService;
        }
         
        [HttpGet]
        [Route("getNewShiftCode")]
        public async Task<string> getNewShiftCode(string CompanyCode, string StoreCode, string TerminalId)
        {
            return await _shiftService.GetNewShiftCode(CompanyCode, StoreCode, TerminalId);
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _shiftService.GetAll(CompanyCode);
        }
        [HttpGet]
        [Route("LoadOpenShift")]
        public async Task<GenericResult> LoadOpenShift(string CompanyCode, string StoreId, string TransDate, string UserId, string CounterId)
        {
            return await _shiftService.LoadOpenShift(CompanyCode,StoreId, TransDate, UserId, CounterId);
        }
        [HttpPost]
        [Route("EndShift")]
        public async Task<GenericResult> EndShift(TShiftHeader model)
        {
            return await _shiftService.EndShift(model);
        }
       
        [HttpGet]
        [Route("GetEndShiftSummary")]
        public async Task<GenericResult> GetEndShiftSummary(string companyCode, string storeId, string shiftId)
        {
            return await _shiftService.GetEndShiftSummary(companyCode, storeId, shiftId);
        }

        [HttpGet]
        [Route("GetOpenShiftSummary")]
        public async Task<GenericResult> GetOpenShiftSummary(string companyCode, string storeId, DateTime? Date)
        {
            return await _shiftService.GetOpenShiftSummary(companyCode, storeId, Date);
        } 
        [HttpGet]
        [Route("ShiftSummaryByDepartment")]
        public async Task<GenericResult> ShiftSummaryByDepartment(string companyCode, string storeId, string Userlogin, string FDate, string TDate, string dailyId, string shiftId)
        {
            return await _shiftService.ShiftSummaryByDepartment( companyCode,  storeId,  Userlogin,  FDate,  TDate,  dailyId,  shiftId);
        }
        //Task<GenericResult> GetOpenShiftSummary(string companyCode, string storeId, DateTime? Date)



        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string companyCode, string StoreId, string top)
        {
            return await _shiftService.GetByStore(companyCode, StoreId, top);
        }
        [HttpPost]
        [Route("CreateShift")]
        public async Task<GenericResult> CreateShift(TShiftHeader model)
        { 
            return await _shiftService.Create(model);
        } 
        
        [HttpPost]
        [Route("EndShiftNew")]
        public async Task<GenericResult> EndShift(EndShiftPrintViewModel model)
        {
            return await _shiftService.Update(model);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _shiftService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companyCode, string id)
        {
            return await _shiftService.GetByCode(companyCode, id);
        }

         

        public static PhysicalAddress GetMacAddress()
        {
            var myInterfaceAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Select(n => n.GetPhysicalAddress())
                .FirstOrDefault();

            return myInterfaceAddress;
        }

    }
}

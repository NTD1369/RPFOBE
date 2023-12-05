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
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase    
    {

        IInvoiceService _InvoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger, IInvoiceService InvoiceService)
        {
            _logger = logger;
            _InvoiceService = InvoiceService;
        }

       [HttpGet]
        [Route("getNewNum")]
        public async Task<string> getNewNum(string companyCode, string storeId)
        {
            return await _InvoiceService.GetNewOrderCode(companyCode, storeId);
        }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)
        {
            return await _InvoiceService.GetOrderById(id, companycode, storeid);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("CheckARExistedBySoId")]
        public async Task<GenericResult> CheckARExistedBySoId(string SOId, string companycode, string storeid)
        {
            return await _InvoiceService.CheckARExistedBySoId(SOId, companycode, storeid);
        } 
        //Task  GetCheckedPayment(string TransId, string EventId, string CompanyCode, string StoreId);
        [HttpGet]
        [Route("GetCheckedPayment")]
        public async Task<GenericResult> GetCheckedPayment(string TransId, string EventId, string CompanyCode, string StoreId)
        {
            return await _InvoiceService.GetCheckedPayment(TransId, EventId, CompanyCode, StoreId);
        }

        [HttpGet]
        [Route("GetCheckOutByEvent")]
        public async Task<GenericResult> GetCheckOutByEvent(string EventId, string CompanyCode, string StoreId)
        {
            return await _InvoiceService.GetCheckOutList(EventId, CompanyCode, StoreId);
        }

        [HttpGet]
        [Route("GetByType")]
        
        public async Task<GenericResult> GetByType(string companycode, string storeId, string Type, string fromdate, string todate, string top)
        {
            return await _InvoiceService.GetByType( companycode, storeId, Type, fromdate, todate, top);
        }

        [HttpGet]
        [Route("GetEcomARList")]

        public async Task<GenericResult> GetEcomARList(string companycode, string storeId, string Type, string fromdate, string todate)
        {
            return await _InvoiceService.GetByType(companycode, storeId, Type, fromdate, todate, "");
        }
        
        [HttpPost]
        [Route("CreateInvoice")]
        public async Task<GenericResult> CreateInvoice(InvoiceViewModel model)
        {
            //return await _InvoiceService.CreateInvoice(model);
            return await _InvoiceService.CreateInvoiceByTableType(model);
        }
        //Task<GenericResult> SaveImage(InvoiceViewModel model)
        [HttpPost]
        [Route("SaveImage")]
        public async Task<GenericResult> SaveImage(InvoiceViewModel model)
        {
            return await _InvoiceService.SaveImage(model);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _InvoiceService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
    }
}

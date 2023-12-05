using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MWI.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private string CompanyCode;

        public InvoiceController(IInvoiceService invoiceService, IConfiguration config)
        {
            this._invoiceService = invoiceService;
            this.CompanyCode = RPFO.Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("CompanyCode"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
        }


        [HttpGet("Invoice")]
        public IActionResult GetInvoices(string transId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var items = _invoiceService.GetInvoices(this.CompanyCode, transId);
                if (items != null)
                {
                    result.Success = true;
                    result.Data = items;
                    if (items.Count == 0)
                    {
                        result.Message = "Cannot found data.";
                    }
                }
                else
                {
                    result.Message = "Cannot found data. (Exception)";
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return BadRequest(result);
            }
        }


        [HttpPost("Invoice")]
        public IActionResult CreateInvoices([FromBody] TInvoiceViewModel invoice)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (invoice == null)
                {
                    result.Message = "Data must be not null.";
                    return BadRequest(result);
                }
                //if (string.IsNullOrEmpty(invoice.CompanyCode))
                //{
                //    result.Message = "CompanyCode be not empty.";
                //    return BadRequest(result);
                //}
                if (string.IsNullOrEmpty(invoice.StoreId))
                {
                    result.Message = "StoreId be not empty.";
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(invoice.CompanyCode))
                {
                    invoice.CompanyCode = this.CompanyCode;
                }
                var items = _invoiceService.CreateInvoice(invoice, out string msg);
                if (!string.IsNullOrEmpty(items))
                {
                    result.Success = true;
                    result.Data = items;
                    result.Message = "Create Invoice Successfuly.";
                }
                else
                {
                    result.Message = "Cannot create invoice. Message: " + msg;
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

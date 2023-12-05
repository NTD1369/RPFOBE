using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.OMSModels;
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
    public class BankTerminalController : ControllerBase
    {

        IBankTerminalService _bankTerminalService;
        private readonly ILogger<BankTerminalController> _logger;

        public BankTerminalController(ILogger<BankTerminalController> logger, IBankTerminalService bankTerminalService)
        {
            _logger = logger;
            _bankTerminalService = bankTerminalService;
        }
        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport models)
        //{
        //    return await _customerService.Import(models);
        //}
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _bankTerminalService.GetAll(CompanyCode);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companyCode, string id)
        {
            return await _bankTerminalService.GetByCode(companyCode, id);
        }

        [HttpGet]
        [Route("GetByCounter")]
        public async Task<GenericResult> GetByCounter(string companyCode, string StoreId, string CounterId)
        {
            return await _bankTerminalService.GetByCounter(companyCode, StoreId, CounterId);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MBankTerminal model)
        {
            return await _bankTerminalService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MBankTerminal model)
        {
            return await _bankTerminalService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MBankTerminal model)
        {
            return await _bankTerminalService.Delete(model);
        }

        #region Bank terminal

        //[AllowAnonymous]
        [HttpPost]
        [Route("SendPayment")]
        public IActionResult SendPaymentToTerminal(string type, string bankName, string portName, double amount, string invoiceNo, int timeOut, string orderId)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(bankName))
                {
                    bankName = "CIMBBANK";
                }

                if (string.IsNullOrEmpty(portName))
                {
                    portName = "COM1";
                }

                Data.Models.TerminalDataModel response = _bankTerminalService.SendPaymentToTerminal(type, bankName, portName, amount, invoiceNo, timeOut, orderId, out string msg);
                if (response != null)
                {
                    if (string.IsNullOrEmpty(response.CardIssuerID))
                    {
                        response.CardIssuerID = response.CardIssuerName;
                    }

                    if (!string.IsNullOrEmpty(msg))
                    {
                        result.Data = response;
                        result.Success = false;
                        result.Message = msg;
                    }
                    else
                    {
                        result.Data = response;
                        result.Success = true;
                    }
                }
                else
                {
                    result.Message = msg;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = "Exception: " + ex.Message;

                return BadRequest(result);
            }
            return Ok(result);
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("ReadData")]
        public IActionResult ReadData(string type, string bankName, string portName, double amount, string invoiceNo, int timeOut)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(bankName))
                {
                    bankName = "CIMBBANK";
                }

                if (string.IsNullOrEmpty(portName))
                {
                    portName = "COM1";
                }

                Data.Models.TerminalDataModel response = _bankTerminalService.TestReadData(type, bankName, portName, amount, invoiceNo, timeOut, out string msg);
                if (response != null)
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        result.Data = response;
                        result.Success = false;
                        result.Message = msg;
                    }
                    else
                    {
                        result.Data = response;
                        result.Success = true;
                    }
                }
                else
                {
                    result.Message = msg;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = "Exception: " + ex.Message;

                return BadRequest(result);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CheckConnect")]
        public IActionResult CheckConnectBankDevice(string bankName, string portName)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(bankName))
                {
                    bankName = "CIMBBANK";
                }

                if (string.IsNullOrEmpty(portName))
                {
                    portName = "COM1";
                }

                bool response = _bankTerminalService.CheckConnectBankTerminal(bankName, portName, out string msg);
                result.Success = response;
                result.Message = msg;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = "Exception: " + ex.Message;

                return BadRequest(result);
            }
            return Ok(result);
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("GenerateRSAKey")]
        public IActionResult GenerateRSAKey(int keySize, bool isPem)
        {
            GenericResult result = new GenericResult();
            try
            {
                var rsaKey = _bankTerminalService.AssignNewRSAKey(keySize, isPem);
                if (!string.IsNullOrEmpty(rsaKey.Item1) && !string.IsNullOrEmpty(rsaKey.Item2))
                {
                    result.Success = true;

                    dynamic exo = new System.Dynamic.ExpandoObject();
                    ((IDictionary<String, Object>)exo).Add("PrivateKey", rsaKey.Item1);
                    ((IDictionary<String, Object>)exo).Add("PublicKey", rsaKey.Item2);
                    result.Data = exo;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = "Exception: " + ex.Message;

                return BadRequest(result);
            }
            return Ok(result);
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("CreateOrderPayoo")]
        public IActionResult CreateOrderPayoo([FromBody] PayooDataModel payooModel)
        {
            GenericResult result = new GenericResult();
            try
            {
                PayooResponseModel response = _bankTerminalService.CreateOrderPayoo(payooModel, out string msg);
                if (response != null && response.ReturnCode == 0)
                {
                    result.Success = true;
                    result.Data = response;
                    result.Message = msg;
                }
                else
                {
                    result.Data = response;
                    result.Message = msg;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = "Exception: " + ex.Message;

                return BadRequest(result);
            }
            return Ok(result);
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("GetOrderPayoo")]
        public IActionResult GetOrderPayoo(string orderCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var dataModel = _bankTerminalService.GetOrderPayoo(orderCode, out string msg);
                if (dataModel != null)
                {
                    result.Success = true;
                    result.Data = dataModel;
                    result.Message = msg;
                }
                else
                {
                    result.Data = dataModel;
                    result.Message = msg;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = "Exception: " + ex.Message;

                return BadRequest(result);
            }
            return Ok(result);
        }

        #endregion
    }
}

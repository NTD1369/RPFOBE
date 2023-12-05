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
    public class ShortcutController : ControllerBase
    {

        IShortcutService _shortcutService;
        private readonly ILogger<ShortcutController> _logger;

        public ShortcutController(ILogger<ShortcutController> logger, IShortcutService shortcutService)
        {
            _logger = logger;
            _shortcutService = shortcutService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _shortcutService.GetAll(CompanyCode);
        }
        [HttpGet]
        [Route("GetByFunction")]
        public async Task<GenericResult> GetByFunction(string CompanyCode, string FunctionCode)
        {
            return await _shortcutService.GetByFunction(CompanyCode, FunctionCode);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode , string Id)
        {
            return await _shortcutService.GetByCode(CompanyCode, Id);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MShortcutKeyboard model)
        {
            return await _shortcutService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MShortcutKeyboard model)
        {
            return await _shortcutService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CompanyCode, string Id)
        {
            return await _shortcutService.Delete(CompanyCode, Id);
        }
        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    return await _paymentService.Import(model);
        //}

        //[HttpGet]
        //[Route("MwiGet")]
        //public async Task<IEnumerable<MPaymentMethod>> GetMPayments(string companyCode, string paymentCode, string stored, string status)
        //{
        //    return await _paymentService.GetMPayments(companyCode, paymentCode, stored, status);
        //}
    }
}

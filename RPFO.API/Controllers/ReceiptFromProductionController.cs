using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPFO.Application.Interfaces;
using RPFO.Utilities.Dtos;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptFromProductionController : ControllerBase
    {
        private readonly IReceiptfromProductionService _ReceiptfromProductionService;

        public ReceiptFromProductionController(IReceiptfromProductionService receiptfromProductionService)
        {
            _ReceiptfromProductionService = receiptfromProductionService;
        }

        [HttpGet]
        [Route("GetId")]
        public async Task<GenericResult> GetById(string id)
        {
            return await _ReceiptfromProductionService.GetById(id);
        }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)

        {
            GenericResult result = new GenericResult();
            return await _ReceiptfromProductionService.GetOrderById(id, companycode, storeid);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByType")]

        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            return await _ReceiptfromProductionService.GetByType(companycode, storeId, fromdate, todate, key, status);
        }
    }
}

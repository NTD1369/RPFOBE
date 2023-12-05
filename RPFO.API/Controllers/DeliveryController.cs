//using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPFO.Application.Interfaces;
using RPFO.Utilities.Dtos;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController: ControllerBase
    {

        private readonly IDeliveryService _DeliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _DeliveryService = deliveryService;
        }

        [HttpGet]
        [Route("GetId")]
        public async Task<GenericResult> GetById(string id)
        {
            return await _DeliveryService.GetById(id);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)

        {
            GenericResult result = new GenericResult();
            return await _DeliveryService.GetOrderById(id, companycode, storeid);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByType")]

        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            return await _DeliveryService.GetByType(companycode, storeId, fromdate, todate, key, status);
        }
        
    }
}

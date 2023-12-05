using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.Application.Interfaces;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StoreWaseHouseController : ControllerBase
    {
        IStoreWareHouseService _storeWareHouseService;
        private readonly ILogger<StoreWaseHouseController> _logger;

        public StoreWaseHouseController(IStoreWareHouseService storeWareHouseService, ILogger<StoreWaseHouseController> logger)
        {
            _storeWareHouseService = storeWareHouseService;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetByStoreID")]
        public async Task<GenericResult> GetByStoreID(string storeid)
        {
            return await _storeWareHouseService.GetByStoreID(storeid);
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string StoreID)
        {
            return await _storeWareHouseService.GetAll(StoreID);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MStoreWarehouseModel model)
        {
            return await _storeWareHouseService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MStoreWarehouseModel model)
        {
            return await _storeWareHouseService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string storeid)
        {
            return await _storeWareHouseService.Delete(storeid);
        }
        [HttpGet]
        [Route("GetWhsbyStore")]
        public async Task<GenericResult> GetWhsByStore(string CompanyCode, string storeid)
        {
            return await _storeWareHouseService.GetWhsByStore(CompanyCode, storeid);
        }
    }
}
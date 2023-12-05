using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MWI.API.Helpers;
using Newtonsoft.Json;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.EntitiesMWI;
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
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterDataService MasterData;
        private readonly IRpfoAPIService RpfoAPI;
        private readonly string CompanyCode;

        public MasterDataController(IMasterDataService masterDataService, IRpfoAPIService rpfoAPIService, IConfiguration config)
        {
            this.MasterData = masterDataService;
            this.RpfoAPI = rpfoAPIService;
            this.CompanyCode = RPFO.Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("CompanyCode"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
        }

        [Cached(600)]
        [HttpGet("Items")]
        public IActionResult GetItems(string itemCode)
        {
            try
            {
                GenericResult result = new GenericResult();
                var items = MasterData.GetItems(this.CompanyCode, itemCode);
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
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("ItemUom")]
        //public IActionResult GetItemUom(string itemCode, string uomCode)
        //{
        //    try
        //    {
        //        GenericResult result = new GenericResult();
        //        var items = MasterData.GetItemUom(itemCode, uomCode);
        //        if (items != null)
        //        {
        //            result.Success = true;
        //            result.Data = items;
        //            if (items.Count == 0)
        //            {
        //                result.Message = "Cannot found data.";
        //            }
        //        }
        //        else
        //        {
        //            result.Message = "Cannot found data. (Exception)";
        //        }
        //        return Ok(result);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [Cached(600)]
        [HttpGet("Store")]
        public IActionResult GetStore(string storeId)
        {
            try
            {
                GenericResult result = new GenericResult();
                var items = MasterData.GetStore(this.CompanyCode, storeId);
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
                return BadRequest(ex.Message);
            }
        }

        [Cached(600)]
        [HttpGet("PriceList")]
        public IActionResult GetPriceList(string priceListId, string storeId)
        {
            try
            {
                GenericResult result = new GenericResult();
                var items = MasterData.GetPriceList(this.CompanyCode, priceListId, storeId);
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
                return BadRequest(ex.Message);
            }
        }

        [Cached(600)]
        [HttpGet("PriceCheck")]
        public IActionResult GetPriceCheck(string storeId, string itemCode, string uomCode, string barCode, string date)
        {
            try
            {
                GenericResult result = new GenericResult();
                var items = MasterData.GetPriceCheck(this.CompanyCode, storeId, itemCode, uomCode, barCode, date);
                if (items != null)
                {
                    result.Success = true;
                    if (items.Count == 0)
                    {
                        result.Message = "Cannot found data.";
                    }
                    else
                    {
                        result.Data = items.FirstOrDefault();
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
                return BadRequest(ex.Message);
            }
        }

        [Cached(600)]
        [HttpGet("Warehouse")]
        public IActionResult GetWarehouse(string whsCode)
        {
            try
            {
                GenericResult result = new GenericResult();
                var items = MasterData.GetWarehouse(this.CompanyCode, whsCode);
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
                return BadRequest(ex.Message);
            }
        }

        [Cached(600)]
        [HttpGet("Capacity")]
        public async Task<IActionResult> GetCapacityAsync(DateTime? transDate, int? quantity, string storeId, string storeAreaId, string timeFrameId)
        {
            try
            {
                GenericResult result = new GenericResult();
                if (transDate == null)
                {
                    result.Message = "TransDate must be not null.";
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(storeId))
                {
                    result.Message = "StoreId must be not null.";
                    return BadRequest(result);
                }

                var response = await RpfoAPI.GetCapacitiesAsync(transDate.Value, quantity, storeId, storeAreaId, timeFrameId);
                //transDate = new DateTime(2020, 12, 22);
                //var items = await RpfoAPI.GetCapacitiesAsync("CP001", transDate.Value, quantity, "ST001", "SA002", "TF001");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        var items = JsonConvert.DeserializeObject<List<CapacityViewModelMwi>>(res.Data.ToString());
                        result.Success = res.Success;
                        result.Data = items;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                        if (items.Count == 0)
                        {
                            result.Message = "Cannot found data.";
                        }
                    }
                    else
                    {
                        result = res;
                    }
                }
                else
                {
                    result.Code = (int)response.StatusCode;
                    result.Message = "Cannot found data. (Exception)";
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Cached(600)]
        [HttpGet("TimeFrame")]
        public async Task<IActionResult> GetTimeFrameAsync(string timeFrameId)
        {
            try
            {
                GenericResult result = new GenericResult();

                var response = await RpfoAPI.GetTimeFrameAsync(this.CompanyCode, timeFrameId);
                //transDate = new DateTime(2020, 12, 22);
                //var items = await RpfoAPI.GetCapacitiesAsync("CP001", transDate.Value, quantity, "ST001", "SA002", "TF001");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        var items = JsonConvert.DeserializeObject<List<TimeFrameViewModel>>(res.Data.ToString());
                        result.Success = res.Success;
                        result.Data = items;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                        if (items.Count == 0)
                        {
                            result.Message = "Cannot found data.";
                        }
                    }
                    else
                    {
                        result = res;
                    }
                }
                else
                {
                    result.Code = (int)response.StatusCode;
                    result.Message = "Cannot found data. (Exception)";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[AllowAnonymous]
        [Cached(600)]
        [HttpGet("ItemStock")]
        public async Task<IActionResult> GetItemStockAsync(string storeId, string slocId, string itemCode, string uomCode, string barCode, string serialNum)
        {
            try
            {
                GenericResult result = new GenericResult();

                //var response = await RpfoAPI.GetItemStockAsync(storeId, slocId, itemCode, uomCode, barCode, serialNum);
                var response = await RpfoAPI.GetItemStockAsync(storeId, slocId, itemCode, uomCode, barCode, serialNum);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        var items = JsonConvert.DeserializeObject<List<ItemStockViewModel>>(res.Data.ToString());
                        result.Success = res.Success;
                        result.Data = items;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                        if (items.Count == 0)
                        {
                            result.Message = "Cannot found data.";
                        }
                    }
                    else
                    {
                        result = res;
                    }
                }
                else
                {
                    result.Code = (int)response.StatusCode;
                    result.Message = "Cannot found data. (Exception)";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Cached(600)]
        [AllowAnonymous]
        [HttpGet("PaymentMethod")]
        public async Task<IActionResult> GetPaymentMethodAsync(string paymentCode, string storeId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var response = await RpfoAPI.GetPaymentMethodAsync(paymentCode, storeId, "A");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<MPaymentMethod>>(responseString);
                    if (items != null && items.Count > 0)
                    {
                        result.Data = items;
                    }
                    else
                    {
                        result.Message = "Cannot found data.";
                    }
                    result.Success = true;

                    //GenericResult res = JsonConvert.DeserializeObject<GenericResult>(responseString);
                    //if (res.Success)
                    //{
                    //    var items = JsonConvert.DeserializeObject<List<MPaymentMethod>>(res.Data.ToString());
                    //    result.Success = res.Success;
                    //    result.Data = items;
                    //    result.Code = (int)System.Net.HttpStatusCode.OK;
                    //    if (items.Count == 0)
                    //    {
                    //        result.Message = "Cannot found data.";
                    //    }
                    //}
                    //else
                    //{
                    //    result = res;
                    //}
                }
                else
                {
                    result.Code = (int)response.StatusCode;
                    result.Message = "Cannot found data. (Exception)";
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

    }
}

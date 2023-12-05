using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
 
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {

        IItemService _itemService;
        private readonly ILogger<ItemController> _logger;
        private readonly IHostingEnvironment _appEnvironment;
        public ItemController(ILogger<ItemController> logger, IItemService itemService, IHostingEnvironment appEnvironment)
        {
            _logger = logger;
            _itemService = itemService;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        [Route("CheckMasterData")]
        public async Task<GenericResult> CheckMasterData(string companyCode, string storeId, string keyword, string CustomerGroupId)
        {
            return await _itemService.CheckMasterData(companyCode, storeId, keyword, CustomerGroupId);
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(string CompanyCode, string StoreId, string ItemCode, string Keyword, string UomCode, string BarCode, string Merchandise, string Group,
            string ItemCate1, string ItemCate2, string ItemCate3, string CustomF1, string CustomF2, string CustomF3, string CustomF4, string CustomF5
            , string CustomF6, string CustomF7, string CustomF8, string CustomF9, string CustomF10, string ValidFrom, string ValidTo, bool? IsSerial
            , bool? IsBOM, bool? IsVoucher, bool? IsCapacity, string CustomerGroupId, string PriceListId, string PLU, decimal? PriceFrom, decimal? PriceTo,decimal? Display)
            {
            return Ok(await _itemService.GetAll(CompanyCode, StoreId, ItemCode, Keyword, UomCode, BarCode, Merchandise, Group, ItemCate1, ItemCate2, ItemCate3, CustomF1, CustomF2, CustomF3, CustomF4, CustomF5
            , CustomF6, CustomF7, CustomF8, CustomF9, CustomF10, ValidFrom, ValidTo, IsSerial, IsBOM, IsVoucher, IsCapacity, CustomerGroupId, PriceListId, PLU, PriceFrom, PriceTo, null, Display));
        }

        //[Cached(600)]
        [HttpPost]
        [Route("GetAllByList")]
        public async Task<IActionResult> GetAllByList(ItemFilterModel  model)
        {
            
            return Ok(await _itemService.GetAll(model.CompanyCode, "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""
            , "", "", "", "", "", null, null, null, null, null, null, "", "", "", null, null, model.ItemList));
        }


        [Cached(600)]
        [HttpGet]
        [Route("GetItemOtherViewList")]
        public async Task<IActionResult> GetItemOtherViewList(string Company, string Store, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, string CustomerGroupId)
        {
            return Ok(await _itemService.GetItemOtherViewList(Company, Store, ItemCode, UomCode, BarCode, Keyword, Merchandise, Type, CustomerGroupId));
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetItemViewList")]
        public async Task<IActionResult> GetItemViewList(string Company, string Store, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, string CustomerGroupId, string PriceListId,string licensePlate)
        {
            return Ok(await _itemService.GetItemViewList(Company, Store, ItemCode, UomCode, BarCode, Keyword, Merchandise, Type, CustomerGroupId,  PriceListId,licensePlate)) ;
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetItemWithoutPriceList")]
        public async Task<IActionResult> GetItemWithoutPriceList(string Company, string Store, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type,Boolean? iscount = false)
        {
            return Ok(await _itemService.GetItemWithoutPriceList(Company, Store, ItemCode, UomCode, BarCode, Keyword, Merchandise, Type,iscount));
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetItemFilter")]
        public async Task<IActionResult> GetItemFilter(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Group,
            string ItemCate1, string ItemCate2, string ItemCate3, string CustomF1, string CustomF2, string CustomF3, string CustomF4, string CustomF5
            , string CustomF6, string CustomF7, string CustomF8, string CustomF9, string CustomF10, string ValidFrom, string ValidTo, bool? IsSerial
            , bool? IsBOM, bool? IsVoucher, bool? IsCapacity, string CustomerGroupId, string PriceListId, string PLU, decimal? PriceFrom, decimal? PriceTo, bool? isScanner)
        {
            List<ItemCheckModel> FilterItemList = null;
            return Ok(await _itemService.GetItemFilter(CompanyCode, StoreId, ItemCode, UomCode, BarCode, Keyword, Merchandise, Group, ItemCate1, ItemCate2, ItemCate3, CustomF1, CustomF2, CustomF3, CustomF4, CustomF5
            , CustomF6, CustomF7, CustomF8, CustomF9, CustomF10, ValidFrom, ValidTo, IsSerial, IsBOM, IsVoucher, IsCapacity, CustomerGroupId, PriceListId, PLU, PriceFrom, PriceTo, isScanner,  FilterItemList));
        }
        [Cached(600)]
        [HttpPost]
        [Route("GetItemFilterByList")]
        public async Task<IActionResult> GetItemFilterByList(string CompanyCode, string StoreId, string CustomerGroupId, string PriceListId, List<ItemCheckModel> FilterItemList)
        {
             
            return Ok(await _itemService.GetItemFilter(CompanyCode, StoreId, "", "", "", "", "", "", "", "", "", "", "", "", "", ""
            , "", "", "", "", "", "", "", false, false, false, false, CustomerGroupId, PriceListId, "", null, null, false, FilterItemList));
        }

        [Cached(600)]
        [HttpGet]
        [Route("GetItemInfor")]
        public async Task<IActionResult> GetItemInfor(string Company,  string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, string PriceListId)
        {
            return Ok(await _itemService.GetItemInfor(Company, ItemCode, UomCode, BarCode, Keyword, Merchandise, Type, PriceListId));
        }
          


        [HttpGet]
        [Route("GetVariantItem")]
        public async Task<GenericResult> GetVariantItem(string CompanyCode, string ItemCode, string UomCode, string BarCode, string Keyword)
        {
            return await _itemService.GetVariantItem(CompanyCode, ItemCode, UomCode, BarCode, Keyword);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _itemService.Import(models);
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetItemWPriceList")]
        public async Task<IActionResult> GetItemWPriceList([FromQuery] UserParams userParams)
        {
            var data = await _itemService.GetItemWPriceList(userParams);

            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _itemService.GetPagedList(userParams);
            if(data.Success)
            {
                var dataPage = data.Data as PagedList<ItemViewModel>;
                Response.AddPagination(dataPage.CurrentPage, dataPage.PageSize, dataPage.TotalCount, dataPage.TotalPages);
                return Ok(dataPage);
            }
            return Ok(data);

        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string id)
        {
            return await _itemService.GetByCode(id);
        }
        [HttpGet]
        [Route("GetItemByCode")]
        public async Task<GenericResult> GetItemByCode(string companyCode, string itemCode)
        {
            return await _itemService.GetItemByCode(companyCode, itemCode);
        } 
        [HttpGet]
        [Route("GetItemPrice")]
        public async Task<GenericResult> GetItemPrice(string CompanyCode, string StoreId, string ItemCode, string UomCode)
        {
            return await _itemService.GetItemPrice( CompanyCode,  StoreId,  ItemCode,  UomCode);
        }
        [HttpGet]
        [Route("GetByMer")]
        public async Task<IActionResult> GetByMer([FromQuery] UserParams userParams)
        {
            var data = await _itemService.GetPagedList(userParams);
            if (data.Success)
            {
                var dataPage = data.Data as PagedList<ItemViewModel>;
                Response.AddPagination(dataPage.CurrentPage, dataPage.PageSize, dataPage.TotalCount, dataPage.TotalPages);
                return Ok(dataPage);
            }
            return Ok(data);
            //var data = await _itemService.GetPagedList(userParams);
            //Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            //return Ok(data);
        }
        [HttpGet]
        [Route("GetPagedListSerialByItem")]
        public async Task<IActionResult> GetPagedListSerialByItem([FromQuery] UserParams userParams)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _itemService.GetPagedListSerialByItem(userParams);
                Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return Ok(new { status = "failed", error = ex.Message });
            }

        }
   

        [HttpGet]
        [Route("GetSerialByItem")]
        public async Task<IActionResult> GetSerialByItem(string CompanyCode, string ItemCode, string Keyword, string SlocId)
        {
            return Ok(await _itemService.GetSerialByItem(CompanyCode, ItemCode, Keyword, SlocId)); ;
            //GenericResult result = new GenericResult();
            //try
            //{
            //    var data = await _itemService.GetSerialByItem(CompanyCode, ItemCode);
            //    return Ok(data);
            //}
            //catch (Exception ex)
            //{

            //    return Ok(new { status = "failed", error = ex.Message });
            //}

        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MItem model)
        {
            return await _itemService.Create(model);
        }
     
        [HttpPost]
        [Route("ApplyStoreListing")]
        public async Task<GenericResult> ApplyStoreListing(StoreListingModel model)
        {
            return await _itemService.ApplyStoreListing(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MItem model)
        {
            return await _itemService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _itemService.Delete(Id);
        }
        public class UploadImageModel
        {
            public string param { get; set; }
            //public string param2 { get; set; }
            //public string param3 { get; set; }
            //public string param4 { get; set; }
            //public string param5 { get; set; }

            //public string param6 { get; set; }

            public IFormFile image { get; set; }

            public List<IFormFile> images { get; set; }

        }
        [HttpPost("avartaupdate")]
        [DisableRequestSizeLimit]
        public async Task<GenericResult> UploadFile(string companyCode, string itemCode, IFormFile image)
        {
            GenericResult result = new GenericResult();
            //IFormFile image = null;
            if (string.IsNullOrEmpty(companyCode))
            {
                result.Success = false;
                result.Message = "CompanyCode is null";
                return result;
            }
            if (string.IsNullOrEmpty(itemCode))
            {
                result.Success = false;
                result.Message = "Item Code is null";
                return result;
            }
            if (image == null)
            {
                result.Success = false;
                result.Message = "Image not null";
                return result;
            }
            //else
            //{
            //    image = uploadModel.image;
            //}
            try
            {
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string fName = image.FileName;
                string folder = Path.Combine(rootPath, "images/items/");
                string path = Path.Combine(folder + image.FileName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                var rsUpdate = await _itemService.AvartaUpdate(companyCode, itemCode, image.FileName);
                result.Success = true;
                result.Message = "";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
        [HttpGet]
        [Route("ItemStock")]
        public async Task<IActionResult> GetItemStock(string companyCode, string storeId, string slocId, string itemCode, string uomCode, string barCode, string serialNum)
        {
            //GenericResult result = new GenericResult();
            return Ok( await _itemService.GetItemStockAsync(companyCode, storeId, slocId, itemCode, uomCode, barCode, serialNum));
            //try
            //{
            //    var data = await _itemService.GetItemStockAsync(companyCode, storeId, slocId, itemCode, uomCode, barCode, serialNum);
            //    if (data != null)
            //    {
            //        return Ok(data);
            //    }
            //    return Ok("Data not found.");
            //}
            //catch (Exception ex)
            //{
            //    //result.Message = "Exception: " + ex.Message;
            //    return Ok("Exception: " + ex.Message);
            //}

        }

        [HttpGet]
        [Route("GetItemByVoucherCollection")]
        public async Task<GenericResult> GetItemByVoucherCollection(string CompanyCode, string StoreId, string itemCode)
        {
            return await _itemService.GetItemByVoucherCollection(CompanyCode,  StoreId, itemCode);
        }

        [HttpGet]
        [Route("GetScanBarcode")]
        public async Task<GenericResult> GetScanBarcode(string CompanyCode, string UserId, string StoreId, string Keyword, string CustomerGroupId)
        {
            return await _itemService.GetScanBarcode(CompanyCode, UserId, StoreId, Keyword, CustomerGroupId);
        }
    }
}

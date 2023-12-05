using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceInforController : ControllerBase    
    {

        IPlaceInforService _placeService;
        private readonly ILogger<PlaceInforController> _logger;

        public PlaceInforController(ILogger<PlaceInforController> logger, IPlaceInforService placeService)
        {
            _logger = logger;
            _placeService = placeService;
        }
        [HttpGet]
        [Route("GetAll")]
        //<param name = "CompanyCode" > Mã Công ty(Bắt buộc)</param>
        //<param name = "StoreId" > Mã Cửa hàng(Bắt buộc)</param>
        /// <summary>
        /// This is method summary I want displayed
        /// </summary>
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Keyword)
        {
            return await _placeService.GetAll(CompanyCode, StoreId, Keyword);
        }
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string TableId)
        {
            return await _placeService.GetByCode(CompanyCode, StoreId, TableId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //<param name = "CompanyCode" > Mã Công ty(Bắt buộc)</param>
        //<param name = "StoreId" > Mã Cửa hàng(Bắt buộc)</param>
        //<param name = "PlaceId" > Mã Sàn(Bắt buộc)</param>
        //<param name = "PlaceName" > Tên sàn(Bắt buộc)</param>
        //<param name = "Description" > Mô tả thông tin sàn(Không Bắt buộc)</param>
        //<param name = "Remark" > Ghi chú(Không Bắt buộc)</param>
        //<param name = "Height" > Chiều cao sàn (Không bắt buộc)</param>
        //<param name = "Width" > Chiều rộng sàn(Không bắt buộc)</param>
        //<param name = "Longs" > Chiều dài sàn(Không bắt buộc)</param>
        //<param name = "Slot" > Số lượng bàn(Không bắt buộc)</param>
        //<param name = "CustomField1" > Custom 1 (Không bắt buộc)</param>
        //<param name = "CustomField2" > Custom 2 (Không bắt buộc)</param>
        //<param name = "CustomField3" > Custom 3 (Không bắt buộc)</param>
        //<param name = "CustomField4" > Custom 4 (Không bắt buộc)</param>
        //<param name = "CustomField5" > Custom 5 (Không bắt buộc)</param>
        //<param name = "CreatedBy" > Người tạo (Bắt buộc)</param>
        //<param name = "CreatedOn" > Ngày giờ tạo (Bắt buộc)</param>
        //<param name = "ModifiedBy" > Người cập nhật (Không bắt buộc)</param>
        //<param name = "ModifiedOn" > Ngày giờ cập nhật (không bắt buộc)</param>
        //<param name = "Status" > Trạng thái (Bắt buộc - Mặc định là 'A')</param>
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPlaceInfor model)
        {
            return await _placeService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPlaceInfor model)
        {
            return await _placeService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MPlaceInfor model)
        {
            return await _placeService.Delete(model);
        }
    }
}

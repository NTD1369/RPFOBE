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
    public class TablePlaceController : ControllerBase    
    {

        ITablePlaceService _tableService;
        private readonly ILogger<TablePlaceController> _logger;

        public TablePlaceController(ILogger<TablePlaceController> logger, ITablePlaceService tableService)
        {
            _logger = logger;
            _tableService = tableService;
        }

        [HttpGet]
        [Route("GetAll")]
        //<param name = "CompanyCode" > Mã Công ty(Bắt buộc)</param>
        //<param name = "StoreId" > Mã Cửa hàng(Bắt buộc)</param>
        /// <summary>
        /// This is method summary I want displayed
        /// </summary>
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string Keyword, string IsSetup, string IsDesign)
        {
            return await _tableService.GetAll(CompanyCode, StoreId, ShiftId, PlaceId, Keyword, IsSetup, IsDesign);
        }

        [HttpGet]
        [Route("GetAllTableNoActiveInPlace")]
        public async Task<GenericResult> GetAllTableNoActiveInPlace(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string Keyword)
        {
            return await _tableService.GetAllTableNoActiveInPlace(CompanyCode, StoreId, ShiftId, PlaceId, Keyword);
        }

        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string ShiftId, string PlaceId,  string TableId)
        {
            return await _tableService.GetByCode(CompanyCode, StoreId, ShiftId, PlaceId,TableId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //<param name = "CompanyCode" > Mã Công ty(Bắt buộc)</param>
        //<param name = "StoreId" > Mã Cửa hàng(Bắt buộc)</param>
        //<param name = "PlaceId" > Mã Sàn(Bắt buộc)</param>
        //<param name = "TableId" > Mã Sàn(Bắt buộc)</param> 
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
        //<param name = "Type" > Loại Sàn (Bắt buộc - Mặc định là 'A')</param>
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MTablePlace model)
        {
            return await _tableService.Create(model);
        }
        [HttpPost]
        [Route("Apply")]
        public async Task<GenericResult> Apply(MTablePlace model)
        {
            return await _tableService.Apply(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MTablePlace model)
        {
            return await _tableService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MTablePlace model)
        {
            return await _tableService.Delete(model);
        }
    }
}

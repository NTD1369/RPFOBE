using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
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
using System.Management;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlacePrintController : ControllerBase
    {
        private readonly IPlacePrintService _placePrintService;
        private readonly ILogger<TablePlaceController> _logger;
        public PlacePrintController(
            ILogger<TablePlaceController> logger,
            IPlacePrintService placePrintService)
        {
            _logger = logger;
            _placePrintService = placePrintService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId)
        {
            return await _placePrintService.GetAll(CompanyCode, StoreId);
        }

        [HttpGet]
        [Route("ViewItemByItemGroup")]
        [AllowAnonymous]
        public async Task<GenericResult> ViewItemByItemGroup(string CompanyCode, string StoreId,string itemGroup,string Status)
        {
            return await _placePrintService.ViewItemByItemGroup(CompanyCode, StoreId,itemGroup, Status);
        }


        [HttpGet]
        [Route("GetListItemGroup")]
        [AllowAnonymous]
        public async Task<GenericResult> GetListItemGroup(string CompanyCode, string StoreId)
        {
            return await _placePrintService.GetListItemGroup(CompanyCode , StoreId);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TPlacePrint model)
        {
            return await _placePrintService.Create(model);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TPlacePrint model)
        {
            return await _placePrintService.Update(model);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(int PrintId)
        {
            return await _placePrintService.Delete(PrintId);
        }

        [HttpGet]
        [Route("SyncPrintName")]
        public async Task<List<SyncPrintNameViewModel>> SyncPrintName()
        {
            var syncPrintNames = new List<SyncPrintNameViewModel>();
            try
            {
                ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Printer");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                foreach (ManagementObject printer in searcher.Get())
                {
                    var item = new SyncPrintNameViewModel();
                    item.PrintName = printer["Name"] as string;
                    item.PrinterStatus = printer["Status"] as string;
                    item.PortName = printer["PortName"] as string;
                    syncPrintNames.Add(item);
                }
            }
            catch (ManagementException e)
            {
                var item = new SyncPrintNameViewModel();
                item.ErrorMessage = e.Message;
                syncPrintNames.Add(item);
            }

            return await Task.FromResult(syncPrintNames);
        }
    }
}

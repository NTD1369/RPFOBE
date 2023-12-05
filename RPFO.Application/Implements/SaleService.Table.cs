using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RPFO.Data.OMSModels;
using System.IO;
using RPFO.Utilities.Helpers;
using DevExpress.XtraSpreadsheet.Model;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static iTextSharp.tool.xml.html.table.TableRowElement;
using static iTextSharp.text.pdf.AcroFields;
using System.Security.Policy;
using DevExpress.Printing.Utils.DocumentStoring;
using DevExpress.DataAccess.Native;

namespace RPFO.Application.Implements
{
    public partial class SaleService : ISaleService
    {
        private string PrefixCacheGetItem = "QAITM-{0}-{1}";
        public async Task<GenericResult> GetContractItem(string CompanyCode, string StoreId, string PlaceId, string ContractNo, string TransId, string ShiftId)
        {
            GenericResult result = new GenericResult();
            List<ItemViewModel> itemViewModels = new List<ItemViewModel>();

            try
            {
                List<ItemCheckModel> itemCheckModels = new List<ItemCheckModel>();

                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode ?? "");
                parameters.Add("StoreId", StoreId ?? "");
                parameters.Add("PlaceId", PlaceId ?? "");
                parameters.Add("ContractNo", ContractNo ?? "");
                parameters.Add("TransId", TransId ?? "");
                parameters.Add("ShiftId", ShiftId ?? "");
                string LineQuery = "USP_Get_ContractLineByTransId";

                var dataLines = await _saleLineRepository.GetAllAsync(LineQuery, parameters, commandType: CommandType.StoredProcedure);

                foreach (var line in dataLines)
                {
                    ItemCheckModel item = new ItemCheckModel();
                    item.ItemCode = line.ItemCode;
                    item.UomCode = line.UomCode;
                    item.Barcode = line.BarCode;
                    item.Quantity = (double)line.Quantity;

                    var itemOrderExist = itemCheckModels.Where(x => x.ItemCode == item.ItemCode && x.UomCode == item.ItemCode && x.Barcode == item.Barcode).FirstOrDefault();
                    if (itemOrderExist != null)
                    {
                        itemOrderExist.Quantity += item.Quantity;
                    }
                    else
                    {
                        itemCheckModels.Add(item);
                    }

                    if (string.IsNullOrEmpty(line.BomId))
                    {
                        itemViewModels.AddRange(GetItemPaymentForTable(CompanyCode, StoreId,
                            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                             false, false, false, false, "", "", "", null, null, false, item));
                    }
                    else
                    {
                        itemViewModels.AddRange(GetItemPaymentForTable(CompanyCode, StoreId,
                           "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                            false, true, false, false, "", "", "", null, null, false, item));
                    }                   
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            result.Success = true;
            result.Data = itemViewModels;

            return result;

        }

        public List<ItemViewModel> GetItemPaymentForTable(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Group,
           string ItemCate1, string ItemCate2, string ItemCate3, string CustomF1, string CustomF2, string CustomF3, string CustomF4, string CustomF5, string CustomF6, string CustomF7, string CustomF8, string CustomF9,
           string CustomF10, string ValidFrom, string ValidTo, bool? IsSerial, bool? IsBOM, bool? IsVoucher, bool? IsCapacity, string CustomerGroupId, string PriceListId, string PLU, decimal? PriceFrom, decimal? PriceTo,
           bool? isScanner, ItemCheckModel itemCheckModel)
        {
            List<ItemViewModel> itemViewModels = new List<ItemViewModel>();
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                    parameters.Add("Group", string.IsNullOrEmpty(Group) ? "" : Group);
                    parameters.Add("ItemCate1", string.IsNullOrEmpty(ItemCate1) ? "" : ItemCate1);
                    parameters.Add("ItemCate2", string.IsNullOrEmpty(ItemCate2) ? "" : ItemCate2);
                    parameters.Add("ItemCate3", string.IsNullOrEmpty(ItemCate3) ? "" : ItemCate3);
                    parameters.Add("CustomF1", string.IsNullOrEmpty(CustomF1) ? "" : CustomF1);
                    parameters.Add("CustomF2", string.IsNullOrEmpty(CustomF2) ? "" : CustomF2);
                    parameters.Add("CustomF3", string.IsNullOrEmpty(CustomF3) ? "" : CustomF3);
                    parameters.Add("CustomF4", string.IsNullOrEmpty(CustomF4) ? "" : CustomF4);
                    parameters.Add("CustomF5", string.IsNullOrEmpty(CustomF5) ? "" : CustomF5);
                    parameters.Add("CustomF6", string.IsNullOrEmpty(CustomF6) ? "" : CustomF6);
                    parameters.Add("CustomF7", string.IsNullOrEmpty(CustomF7) ? "" : CustomF7);
                    parameters.Add("CustomF8", string.IsNullOrEmpty(CustomF8) ? "" : CustomF8);
                    parameters.Add("CustomF9", string.IsNullOrEmpty(CustomF9) ? "" : CustomF9);
                    parameters.Add("CustomF10", string.IsNullOrEmpty(CustomF10) ? "" : CustomF10);
                    parameters.Add("ValidFrom", string.IsNullOrEmpty(ValidFrom) ? null : ValidFrom);
                    parameters.Add("ValidTo", string.IsNullOrEmpty(ValidTo) ? null : ValidTo);

                    parameters.Add("IsSerial", IsSerial);
                    parameters.Add("IsBOM", IsBOM);
                    parameters.Add("IsVoucher", IsVoucher);
                    parameters.Add("IsCapacity", IsCapacity);

                    if (!string.IsNullOrEmpty(CustomerGroupId))
                    {
                        parameters.Add("CustomerGroupId", string.IsNullOrEmpty(CustomerGroupId) ? "" : CustomerGroupId);
                    }

                    if (!string.IsNullOrEmpty(PriceListId))
                    {
                        parameters.Add("PriceListId", string.IsNullOrEmpty(PriceListId) ? "" : PriceListId);
                    }

                    if (PriceFrom.HasValue)
                    {
                        parameters.Add("PriceFrom", PriceFrom);
                    }

                    if (PriceTo.HasValue)
                    {
                        parameters.Add("PriceTo", PriceTo);
                    }

                    if (itemCheckModel != null)
                    {
                        parameters.Add("ItemCode", string.IsNullOrEmpty(itemCheckModel.ItemCode) ? "" : itemCheckModel.ItemCode);
                        parameters.Add("UomCode", string.IsNullOrEmpty(itemCheckModel.UomCode) ? "" : itemCheckModel.UomCode);
                        parameters.Add("BarCode", string.IsNullOrEmpty(itemCheckModel.Barcode) ? "" : itemCheckModel.Barcode);


                        if (!string.IsNullOrEmpty(itemCheckModel.PLU))
                        {
                            parameters.Add("PLU", PLU);
                        }
                        var itemData = db.Query<ItemViewModel>($"USP_GetItem_Filter", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                        if (itemData != null)
                        {
                            var itemAdd = itemData.FirstOrDefault();
                            if (itemAdd != null)
                            {
                                if (itemCheckModel?.Quantity != null)
                                {
                                    itemAdd.Quantity = (decimal)itemCheckModel.Quantity;
                                }
                                itemViewModels.Add(itemAdd);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return itemViewModels;
        }

        public GenericResult GetOrderByContractNo(string CompanyCode, string StoreId, string ContractNo, string ShiftId, string PlateId)
        {
            GenericResult result = new GenericResult();
            try
            {
                List<SaleViewModel> orders = new List<SaleViewModel>();

                string queryStr = $"select * from T_SalesHeader where CompanyCode = N'{CompanyCode}' and SalesType = N'Table' and StoreId = N'{StoreId}' and (ContractNo = N'{ContractNo}' or CustomF2 =N'{ContractNo}') and CustomF4 = 'NoPaymentOfTable' and Status = 'C' and IsCanceled = 'N' and TransId like 'SOT%' and CustomF1 =N'{PlateId}'";
                List<TSalesHeader> headers = _saleHeaderRepository.GetAll(queryStr, null, commandType: CommandType.Text);
                if (headers != null && headers.Count() > 0)
                {
                    foreach (var header in headers)
                    {
                        var order = GetOrderById(header.TransId, header.CompanyCode, header.StoreId).Result;
                        if (order.Success)
                        {
                            orders.Add(order.Data as SaleViewModel);
                        }
                        else
                        {
                            return order;
                        }
                    }
                    result.Success = true;
                    result.Data = orders;
                }
                else
                {
                    //result.Success = false;
                    //result.Message = "Contract No doesn't existed";
                    result.Success = true;
                    result.Data = null;
                }


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

        public async Task<string> CheckOrderInGroupTable(SaleViewModel model)
        {
            if (string.IsNullOrEmpty(model.ContractNo))
                return await Task.FromResult(string.Empty);

            var tableIdQuery = $"select * from T_TableGroup where CompanyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' and ShiftId = N'{model.ShiftId}' and TableId = N'{model.ContractNo}' and PlaceId = N'{model.CustomF1}' and Status ='A'";
            var tableGroups = await _tableGroupRepository.GetAllAsync(tableIdQuery, null, commandType: CommandType.Text);

            var orderInGroupTable = tableGroups.Any() ? tableGroups.Select(c => c.GroupKey).FirstOrDefault() : string.Empty;

            return await Task.FromResult(orderInGroupTable);
        }

        public async Task UpdatePaymentForTable(SaleViewModel model)
        {
            if (model.Payments != null && model.Payments.Any())
            {
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {


                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var salesHeaderQuery = $"select * from T_SalesHeader where SalesType='Table' and CompanyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' and (ContractNo = N'{model.ContractNo}' or CustomF2 =N'{model.CustomF2}') and CustomF4 = 'NoPaymentOfTable' and Status = 'C' and IsCanceled = 'N' and TransId like 'SOT%' and CustomF1 =N'{model.CustomF1}'";
                                var saleHeaders = await _saleHeaderRepository.GetAllAsync(salesHeaderQuery, null, commandType: CommandType.Text);

                                var groupTable = saleHeaders.Select(x => x.CustomF2).FirstOrDefault();

                                if (saleHeaders.Any())
                                {
                                    foreach (var item in saleHeaders)
                                    {
                                        string queryCheckAndCreate = $"update  T_SalesHeader set CustomF4='PaymentOfTable'  where TransId = '{item.TransId}'";
                                        db.Execute(queryCheckAndCreate, null, commandType: CommandType.Text, transaction: tran);
                                    }
                                }
                                if (!string.IsNullOrEmpty(groupTable))
                                {
                                    string deleteGroupTableAfterPayment = $"delete from T_TableGroup where CompanyCode='{model.CompanyCode}' and StoreId='{model.StoreId}' and ShiftId='{model.ShiftId}' and PlaceId='{model.CustomF1}' and GroupKey='{groupTable}'";
                                    db.Execute(deleteGroupTableAfterPayment, null, commandType: CommandType.Text, transaction: tran);
                                }
                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                }
            }
        }

        public async Task<GenericResult> MoveTable(string CompanyCode, string StoreId, string PlaceId, string FromTable, string ToTable, string TransIdList)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }

            if (string.IsNullOrEmpty(StoreId))
            {
                result.Success = false;
                result.Message = " Store not null.";
                return result;
            }

            try
            {
                //if (model.Payments.Count == 0 && model.SalesMode != "HOLD")
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                string Query = $"Update T_SalesHeader set ContractNo=N'{ToTable}' where StoreId=N'{StoreId}' and ContractNo= N'{FromTable}' and CompanyCode= N'{CompanyCode}'";
                                if (!string.IsNullOrEmpty(TransIdList))
                                {
                                    var lstTransIds = $"('{string.Join("', '", TransIdList)}')";
                                    Query = $"Update T_SalesHeader set ContractNo=N'{ToTable}' where TransId in {lstTransIds} and StoreId=N'{StoreId}' and ContractNo= N'{FromTable}' and CompanyCode= N'{CompanyCode}'";
                                }
                                db.Execute(Query, null, commandType: CommandType.Text, transaction: tran);
                                result.Success = true;
                                result.Message = "";

                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return await Task.FromResult(result);
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return await Task.FromResult(result);

            //throw new NotImplementedException();
        }

        public async Task<GenericResult> MergeTable(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string CreatedBy, List<string> TableList, string ToTable, bool? ClearTable)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }

            if (string.IsNullOrEmpty(StoreId))
            {
                result.Success = false;
                result.Message = " Store not null.";
                return result;
            }
            if (TableList == null || TableList.Count() == 0)
            {
                result.Success = false;
                result.Message = " Please select table to merge.";
                return result;
            }
            if (string.IsNullOrEmpty(ToTable))
            {
                result.Success = false;
                result.Message = " Please select to table .";
                return result;
            }
            try
            {
                //if (model.Payments.Count == 0 && model.SalesMode != "HOLD")
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {

                                //var parameters = new DynamicParameters();
                                string List = "";
                                string TableListStr = "";
                                foreach (var table in TableList)
                                {
                                    List += "'" + table + "',";
                                    TableListStr += table + ";";
                                }
                                if (List.Length > 0)
                                {
                                    List = List.Substring(0, List.Length - 1);
                                }
                                if (TableListStr.Length > 0)
                                {
                                    TableListStr = TableListStr.Substring(0, TableListStr.Length - 1);
                                }
                                List = "(" + List;
                                List += ")";

                                //Group color is #fa3142 
                                //ColorRandom();
                                string Color = "#fa3142";

                                //string Query = $"Update T_SalesHeader set ContractNo=N'{ToTable}' , CustomF3=N'{Color}' where StoreId=N'{StoreId}' and ContractNo in {List} and CompanyCode= N'{CompanyCode}'";
                                //if(ClearTable.HasValue && ClearTable.Value == false)
                                //{
                                //}    

                                string Query = $"Update T_SalesHeader set   CustomF2=N'{ToTable}', CustomF3=N'{Color}'  where StoreId=N'{StoreId}' and ContractNo in {List} and CompanyCode= N'{CompanyCode}' and IsCanceled ='N' and TransId like 'SOT%'";


                                db.Execute(Query, null, commandType: CommandType.Text, transaction: tran);
                                var parameters = new DynamicParameters();

                                parameters.Add("CompanyCode", CompanyCode);
                                parameters.Add("StoreId", StoreId);
                                parameters.Add("ShiftId", ShiftId);
                                parameters.Add("PlaceId", PlaceId);
                                parameters.Add("TableId", "");
                                parameters.Add("GroupKey", ToTable);
                                parameters.Add("Status", "A");
                                parameters.Add("CreatedBy", CreatedBy);
                                parameters.Add("CustomF1", Color);
                                parameters.Add("CustomF2", "");
                                parameters.Add("CustomF3", "");
                                parameters.Add("CustomF4", "");
                                parameters.Add("CustomF5", "");
                                parameters.Add("TableList", TableListStr);

                                var resultX = db.Execute($"USP_I_T_TableGroup", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                result.Success = true;
                                result.Message = "";

                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return await Task.FromResult(result);
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return await Task.FromResult(result);
            //throw new NotImplementedException();
        }

        public async Task<GenericResult> SplitTable(string companyCode, string storeId, string shiftId, string placeId, string tableId, string groupKey)
        {
            GenericResult result = new GenericResult();
            var numberTowTable = 2;

            if (string.IsNullOrEmpty(companyCode))
            {
                result.Success = false;
                result.Message = "Company Code is required";
                return result;
            }

            if (string.IsNullOrEmpty(storeId))
            {
                result.Success = false;
                result.Message = "Store is required";
                return result;
            }

            if (string.IsNullOrEmpty(placeId))
            {
                result.Success = false;
                result.Message = "Place is required";
                return result;
            }

            if (string.IsNullOrEmpty(tableId))
            {
                result.Success = false;
                result.Message = "Table is required";
                return result;
            }

            try
            {
                var tableGroupQuery = $"select * from T_TableGroup where CompanyCode='{companyCode}' and StoreId='{storeId}' and ShiftId='{shiftId}'  and PlaceId='{placeId}' and GroupKey='{groupKey}'";
                var tableGroups = await _tableGroupRepository.GetAllAsync(tableGroupQuery, null, commandType: CommandType.Text);

                if (tableGroups.Any())
                {
                    var checkTwoTable = tableGroups.GroupBy(x => x.TableId).Count();

                    var nextCurrentTableId = tableGroups.Where(x => x.TableId != int.Parse(tableId)).Select(c => c.TableId.ToString()).FirstOrDefault();

                    var salesHeaderQuery = $"select * from T_SalesHeader where CompanyCode = N'{companyCode}' and StoreId = N'{storeId}' and (ContractNo = N'{groupKey}' or CustomF2 =N'{groupKey}') and CustomF4 = 'NoPaymentOfTable'  and Status = 'C' and IsCanceled = 'N' and TransId like 'SOT%' and CustomF1 =N'{placeId}'";
                    var saleHeaders = await _saleHeaderRepository.GetAllAsync(salesHeaderQuery, null, commandType: CommandType.Text);

                    using (IDbConnection db = _saleHeaderRepository.GetConnection())
                    {
                        try
                        {
                            if (db.State == ConnectionState.Closed)
                                db.Open();

                            string delSelectedTableId = $"delete from T_TableGroup where CompanyCode='{companyCode}' and StoreId='{storeId}' and ShiftId='{shiftId}' and PlaceId='{placeId}' and TableId='{tableId}'";
                            db.Execute(delSelectedTableId);

                            db.Close();
                            result.Success = true;

                        }
                        catch (Exception ex)
                        {
                            result.Success = false;
                            result.Message = ex.Message;
                        }
                    }

                    if (checkTwoTable <= numberTowTable)
                    {
                        await _tableGroupRepository.GetAllAsync(
                              $"delete from T_TableGroup where CompanyCode='{companyCode}' and StoreId='{storeId}' and ShiftId='{shiftId}' and PlaceId='{placeId}' and TableId='{nextCurrentTableId}'"
                              , null, commandType: CommandType.Text);

                        using (IDbConnection db = _saleHeaderRepository.GetConnection())
                        {
                            try
                            {
                                if (db.State == ConnectionState.Closed)
                                    db.Open();

                                //string queryCheckAndCreate = $"update  T_SalesHeader set  ContractNo = {nextCurrentTableId} , CustomF2 = 'NULL'  where TransId = '{saleHeader.TransId}'";
                                if (saleHeaders.Any())
                                {
                                    foreach (var item in saleHeaders)
                                    {
                                        string queryCheckAndCreate = nextCurrentTableId != null ?
                                       $"update  T_SalesHeader set  ContractNo = {nextCurrentTableId} , CustomF2 = N'{nextCurrentTableId}'  where TransId = '{item.TransId}'" :
                                       $"update  T_SalesHeader set  ContractNo = {groupKey} , CustomF2 = 'NULL'  where TransId = '{item.TransId}'";
                                        db.Execute(queryCheckAndCreate);
                                    }
                                }

                                db.Close();
                                result.Success = true;

                            }
                            catch (Exception ex)
                            {
                                result.Success = false;
                                result.Message = ex.Message;
                            }
                        }
                    }
                    else
                    {
                        using (IDbConnection db = _saleHeaderRepository.GetConnection())
                        {
                            try
                            {
                                if (db.State == ConnectionState.Closed)
                                    db.Open();

                                if (tableId == groupKey)
                                {
                                    if (nextCurrentTableId != null)
                                    {
                                        string queryTableGroup = $"update  T_TableGroup set  GroupKey = {nextCurrentTableId}  where GroupKey = '{tableId}'";
                                        db.Execute(queryTableGroup);
                                    }

                                    if (saleHeaders.Any())
                                    {
                                        foreach (var item in saleHeaders)
                                        {
                                            string queryCheckAndCreate = nextCurrentTableId != null ?
                                           $"update  T_SalesHeader set  ContractNo = {nextCurrentTableId} , CustomF2 = N'{nextCurrentTableId}'  where TransId = '{item.TransId}'" :
                                           $"update  T_SalesHeader set  ContractNo = {groupKey} , CustomF2 = 'NULL'  where TransId = '{item.TransId}'";
                                            db.Execute(queryCheckAndCreate);
                                        }
                                    }
                                }
                                else
                                {
                                    if (saleHeaders.Any())
                                    {
                                        var tableNotKeyGroups = saleHeaders.Where(x => x.ContractNo == tableId);
                                        foreach (var tableNotKeyGroup in tableNotKeyGroups)
                                        {
                                            string queryCheckAndCreate = $"update  T_SalesHeader set  ContractNo = {nextCurrentTableId} , CustomF2 = N'{groupKey}'  where TransId = '{tableNotKeyGroup.TransId}'";
                                            db.Execute(queryCheckAndCreate);
                                        }
                                    }
                                }

                                db.Close();
                                result.Success = true;

                            }
                            catch (Exception ex)
                            {
                                result.Success = false;
                                result.Message = ex.Message;
                            }
                        }
                    }
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<GenericResult> CancelTableExclude(string CompanyCode, string StoreId, string TableId, string PlaceId, string TransId)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _saleHeaderRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //AssignStaff
                    string queryCheckAndCreate = "";
                    queryCheckAndCreate = $"update  T_SalesHeader set IsCanceled = N'Y'  where TransId not in {TransId} and ContractNo = N'{TableId}' and CompanyCode = N'{CompanyCode}' and StoreId = N'{StoreId}'";
                    db.Execute(queryCheckAndCreate);
                    db.Close();
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;


                }
            }

            return await Task.FromResult(result);
        }

        public async Task<GenericResult> GroupItemByPrint(List<string> lines, string companyCode, string storeId)
        {
            GenericResult result = new GenericResult();
            try
            {
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();

                                parameters.Add("CompanyCode", companyCode);
                                parameters.Add("StoreId", storeId);
                                parameters.Add("ItemCode", lines);

                                var resultX = db.Execute($"USP_I_T_TableGroup", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                result.Success = true;
                                result.Message = "";

                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return await Task.FromResult(result);
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Exception: " + ex.Message;
            }

            return result;
        }

        public async Task<GenericResult> PrintByTypeAsync(SaleViewModel model, string companyCode, string storeId, TablePlaceViewModel tablePlace, string size, string printName)
        {
            GenericResult result = new GenericResult();

            try
            {
                if (model != null)
                {
                    RPFO.Application.PrintLayout.PrintByType itemGroupByPrintName = new PrintLayout.PrintByType();
                    //itemGroupByPrintName.SetModel(model, tablePlace.TableName, tablePlace.PlaceName);
                    itemGroupByPrintName.CreateDocument();
                    itemGroupByPrintName.Print(printName);
                    await Task.Delay(1000);
                }
                else
                {
                    result.Success = false;
                    result.Code = -1;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Exception: " + ex.Message;
            }

            return result;
        }

        public async Task AutoPrintSetting(SaleViewModel model)
        {
            List<GroupItemByPrint> results = new List<GroupItemByPrint>();

            string settingPrintQuery = $"select SettingValue from S_GeneralSetting with (nolock) where SettingId ='SettingPrintByDiviceName' and CompanyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' ";
            settingPrintQuery = _saleHeaderRepository.GetScalar(settingPrintQuery, null, commandType: CommandType.Text);

            if (!string.IsNullOrEmpty(settingPrintQuery))
            {
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        try
                        {
                            string placePrintQuery = $"select Distinct t1.PrintName,t2.ItemCode,t2.ItemName from M_PlacePrint t1 with (nolock) " +
                                $"join M_Item t2 with (nolock) on t1.CompanyCode = t2.CompanyCode and t2.ItemCode in ('{string.Join("','", model.Lines.Select(x => x.ItemCode).ToList())}')" +
                                $"join M_ItemGroup t3 with (nolock) on t3.IGId = t2.ItemGroupId and t3.IGId = t1.GroupItem";
                            var placePrintViewModels = (await db.QueryAsync<PlacePrintViewModel>(placePrintQuery, null, commandType: CommandType.Text, commandTimeout: 3600)).ToList();

                            if (placePrintViewModels.Any())
                            {
                                results = placePrintViewModels
                                 .Join(model.Lines, result => result.ItemCode, item => item.ItemCode, (result, item) => new { Result = result, Item = item })
                                 .GroupBy(pair => pair.Result.PrintName) // Group by PrintName
                                 .Select(group => new GroupItemByPrint
                                 {
                                     CreatedOn = model.CreatedOn,
                                     StoreName = model.StoreName,
                                     SalesPersonName = model.SalesPersonName,
                                     CompanyCode = model.CompanyCode,
                                     Store = model.StoreId,
                                     TableId = model.ContractNo,
                                     TerminalId = model.TerminalId,
                                     PlaceId = model.CustomF1,
                                     PrintName = group.Key,
                                     Items = group.Select(pair => new GroupedItem
                                     {
                                         ItemCode = pair.Result.ItemCode,
                                         ItemName = pair.Result.ItemName,
                                         Quantity = pair.Item.Quantity
                                     }).ToList()
                                 }).ToList();

                                if (results.Any())
                                {
                                    foreach (var item in results)
                                    {
                                        string tablePrintQuery = $"select Distinct t2.TableName,t3.PlaceName from M_TablePlace t1 with (nolock) " +
                                           $"join M_TableInfor t2 with (nolock) on t1.TableId = t2.TableId " +
                                           $"join M_PlaceInfor t3 with (nolock) on t3.PlaceId = t1.PlaceId " +
                                           $"where t1.PlaceId = '{item.PlaceId}' and t1.TableId = '{item.TableId}' and t1.CompanyCode = '{item.CompanyCode}' and t1.StoreId = '{item.Store}' ";
                                        var tablePlace = (await db.QueryAsync<TablePlaceViewModel>(tablePrintQuery, null, commandType: CommandType.Text, commandTimeout: 3600)).FirstOrDefault();
                                        if (tablePlace != null)
                                        {
                                            item.TableName = tablePlace.TableName;
                                            item.PlaceName = tablePlace.PlaceName;

                                            RPFO.Application.PrintLayout.PrintByType itemGroupByPrintName = new PrintLayout.PrintByType();
                                            itemGroupByPrintName.SetModel(item);

                                            itemGroupByPrintName.CreateDocument();
                                            itemGroupByPrintName.Print(item.PrintName);

                                            await Task.Delay(1000);
                                        }
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                }
            }
        }
    }
}

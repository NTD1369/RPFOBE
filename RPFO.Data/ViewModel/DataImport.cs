using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    
    public class CutomerGroupResultViewModel : MCustomerGroup 
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class CutomerResultViewModel : MCustomer 
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class EmployeeResultViewModel : EmployeeViewModel
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class ItemResultViewModel : MItem
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class IItemUomResultViewModel : MItemUom
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }

    public class ItemGroupResultViewModel : MItemGroup
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }

    public class MerchandiseResultViewModel : MMerchandiseCategory
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class PaymentMethodResultViewModel : MPaymentMethod
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class PriceListResultViewModel : MPriceList
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class ProductResultViewModel : MProduct
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class StorageResultViewModel : MStorage
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class StoreResultViewModel : MStore
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class StoreAreaResultViewModel : MStoreArea
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class StoreCapacityResultViewModel : MStoreCapacity
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class StoreGroupResultViewModel : MStoreGroup
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class StorePaymentResultViewModel : MStorePayment
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }

    public class TaxResultViewModel : MTax
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class UOMResultViewModel : MUom
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    } 
    public class UserResultViewModel : UserViewModel
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class UserStoreResultViewModel : MUserStore
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class WarehouseResultViewModel : MWarehouse
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class ItemSerialResultViewModel : MItemSerial
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class ItemSerialStockResultViewModel : MItemSerialStock
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class TItemStorageResultViewModel : TItemStorage
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class MPrepaidCardResultViewModel : MPrepaidCard
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
     public class SaleViewModelResultViewModel : SaleViewModel
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }

    public class PromotionResultViewModel : PromotionViewModel
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    } 
    
    public class GoodReceiptLineResultViewModel : TGoodsIssueLineImport
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class LicensePlateResultViewModel : LicensePlateViewModel
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class TableReseachViewModel : MTableInfor
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }
    public class DataImport
    {
        public string CreatedBy { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public List<BOMViewModel> BOM { get; set; }
        public List<MCustomerGroup> CustomerGroup { get; set; }
        public List<MCustomer> Customer { get; set; }
        public List<MItem> Item { get; set; }
        public List<MItemSerial> ItemSerial { get; set; }
        public List<MItemSerialStock> ItemSerialStock { get; set; }
        public List<MItemUom> ItemUom { get; set; }
        public List<EmployeeViewModel> Employee { get; set; }
        public List<MItemGroup> ItemGroup { get; set; }
        public List<MMerchandiseCategory> Merchandise  { get; set; }
        public List<MPaymentMethod> PaymentMethod { get; set; }
        public List<MPriceList> PriceList { get; set; }
        public List<MProduct> Product { get; set; }
        public List<MStorage> Storage { get; set; }
        public List<MStore> Store { get; set; }
        public List<MStoreArea> StoreArea { get; set; }
        public List<MStoreCapacity> StoreCapacity { get; set; }
        public List<MStoreGroup> StoreGroup { get; set; }
        public List<MStorePayment> StorePayment { get; set; }
        public List<MTax> Tax { get; set; }
        public List<MUom> Uom { get; set; }
        public List<UserViewModel> User { get; set; }
        public List<MUserStore> UserStore { get; set; }
        public List<MWarehouse> Warehouse { get; set; }

        public List<TItemStorage> ItemStorage { get; set; }
        public List<MPrepaidCard> PrepaidCard { get; set; }
        public List<SaleViewModel> SO { get; set; }
        public List<PromotionViewModel> Promotion { get; set; }
        public List<TGoodsIssueLineImport> GoodsReceiptImport { get; set; }
        public List<MLicensePlate> licensePlate { get; set; }
        public List<LicensePlateViewModel> LicensePlateImport { get; set; }
        public List<MTableInfor> TableInfor  { get; set; }

    }
}

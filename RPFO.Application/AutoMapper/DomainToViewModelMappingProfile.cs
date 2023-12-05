using AutoMapper;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using static RPFO.Application.Implements.ReportService;

namespace RPFO.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<TSalesHeader, SaleViewModel>();
            CreateMap<TInvoiceHeader, InvoiceViewModel>();
            CreateMap<TGoodsReceiptPoheader, GRPOViewModel>();
            CreateMap<TPurchaseOrderHeader, PurchaseOrderViewModel>();
            CreateMap<TPurchaseRequestHeader, PurchaseRequestViewModel>();
            CreateMap<TGoodsReturnheader, GReturnPOViewModel>();
            CreateMap<MFunction, NodeFunctionViewModel>();
            CreateMap<MBomheader, BOMViewModel>();
            CreateMap<MTimeFrame, TimeFrameViewModel>();
            CreateMap<TShiftHeader, ShiftViewModel>();

            CreateMap<TGoodsIssueHeader, GoodsIssueViewModel>(); 
            CreateMap<TGoodsIssueLine, GoodIssueLineViewModel>();
            CreateMap<TGoodsIssueLineImport, GoodReceiptLineViewModel>();
            CreateMap<MStore, GeneralSettingStore>();
            CreateMap<SPromoHeaderRpt, PromotionViewModelRpt>();

            CreateMap<TGoodsReceiptHeader, GoodReceiptViewModel>();
         
            CreateMap<TGoodsReceiptLine, GoodReceiptLineViewModel>();
            CreateMap<TInventoryHeader, InventoryViewModel>();
            CreateMap<TInventoryLine, InventoryLineViewModel>();
            CreateMap<TInventoryHeader, TInvoiceViewModel>();

            CreateMap<TInventoryCountingHeader, InventoryCountingViewModel>();
            CreateMap<TInventoryCountingLine, InventoryCountingLineViewModel>();
            CreateMap<BOMViewModel, BOMResultViewModel>();

            CreateMap<TInventoryPostingHeader, InventoryPostingViewModel>();
            CreateMap<TInventoryPostingLine, InventoryPostingLineViewModel>();

            CreateMap<TInventoryTransferHeader, InventoryTransferViewModel>();
            CreateMap<TInventoryTransferLine, InventoryTransferLineViewModel>();

            CreateMap<SPromoHeader, PromotionViewModel>();
            CreateMap<SPromoSchema, SchemaViewModel>();
            CreateMap<SaleViewModel, InvoiceViewModel>();
            CreateMap<TSalesLineViewModel, TInvoiceLineViewModel>();
            CreateMap<TSalesPromoViewModel, TInvoicePromoViewModel>();
            CreateMap<TSalesLineSerialViewModel, TInvoiceLineSerialViewModel>();
            CreateMap<TSalesPayment, TInvoicePayment>();
            CreateMap<SLoyaltyHeader, LoyaltyViewModel>();



            //To result
            CreateMap<MWarehouse, WarehouseResultViewModel>();
            CreateMap<MUserStore, UserStoreResultViewModel>();
            CreateMap<UserViewModel, UserResultViewModel>();
            CreateMap<MUom, UOMResultViewModel>();
            CreateMap<MTax, TaxResultViewModel>();
            CreateMap<MStorePayment, StorePaymentResultViewModel>();
            CreateMap<MStoreGroup, StoreGroupResultViewModel>();
            CreateMap<MStoreCapacity, StoreCapacityResultViewModel>();
            CreateMap<MStoreArea, StoreAreaResultViewModel>();
            CreateMap<MStore , StoreResultViewModel>();
            CreateMap<MStorage , StorageResultViewModel>();
            CreateMap<MProduct , ProductResultViewModel>();
            CreateMap<MPriceList , PriceListResultViewModel>();
            CreateMap<MPaymentMethod , PaymentMethodResultViewModel>();
            CreateMap<MMerchandiseCategory, MerchandiseResultViewModel>();
            CreateMap<MItemGroup, ItemGroupResultViewModel>();
            CreateMap<MItemUom, IItemUomResultViewModel>();
            CreateMap<MItemSerial, ItemSerialResultViewModel>();
            CreateMap<MItemSerialStock, ItemSerialStockResultViewModel>();
            CreateMap<MItem, ItemResultViewModel>();
            CreateMap<MEmployee, EmployeeResultViewModel>();
            CreateMap<EmployeeViewModel, EmployeeResultViewModel>();
            CreateMap<MCustomer, CutomerResultViewModel>();
            CreateMap<MCustomerGroup, CutomerGroupResultViewModel>();
            CreateMap<TItemStorage, TItemStorageResultViewModel>();
            CreateMap<MPrepaidCard, MPrepaidCardResultViewModel>();
            CreateMap<SaleViewModel, SaleViewModelResultViewModel>(); 
            CreateMap<PromotionViewModel, PromotionResultViewModel>();
            CreateMap<MStore, StoreViewModel>();
            CreateMap<MLicensePlate, LicensePlateResultViewModel>();

            CreateMap<RPFO.Data.EntitiesMWI.TSalesHeader, TSalesViewModel>();
            CreateMap<RPFO.Data.EntitiesMWI.TInvoiceHeader, TInvoiceViewModel>();
            CreateMap<T_DeliveryHeader, DeliveryModel>();

            CreateMap<TReturnHeader, GReturnDeliveryViewModel>();

            CreateMap<TReceiptfromProductionHeader, ReceiptfromProductionViewModel>();
            CreateMap<TProductionOrderHeader, ProductionOrderViewModel>();

            CreateMap< MTableInfor, TableReseachViewModel> ();

            CreateMap<LicensePlateViewModel, LicensePlateResultViewModel>();



        }
    }
}

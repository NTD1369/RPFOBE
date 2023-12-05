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
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {

            CreateMap<SaleViewModel, TSalesHeader>();
            CreateMap<InvoiceViewModel, TInvoiceHeader>();
            CreateMap<NodeFunctionViewModel, MFunction>();
            CreateMap<GRPOViewModel, TGoodsReceiptPoheader>();
            CreateMap<PurchaseOrderViewModel, TPurchaseOrderHeader>();
            CreateMap<GReturnPOViewModel, TGoodsReturnheader>();
            CreateMap<PurchaseRequestViewModel, TPurchaseRequestHeader>();
            CreateMap<BOMViewModel, MBomheader>();
            CreateMap<GoodsIssueViewModel, TGoodsIssueHeader>();
            CreateMap<GoodIssueLineViewModel, TGoodsIssueLine>();
            CreateMap<TimeFrameViewModel, MTimeFrame>();
            CreateMap<ShiftViewModel, TShiftHeader > ();
            CreateMap<GoodReceiptViewModel, TGoodsReceiptHeader>();
            CreateMap<GoodReceiptLineViewModel, TGoodsReceiptLine>();
            CreateMap<InventoryViewModel, TInventoryHeader>();
            CreateMap<InventoryLineViewModel, TInventoryLine>();
            CreateMap<TInvoiceViewModel, TInventoryHeader>();
            CreateMap<SPromoOTGroupRpt, SPromoOTGroup>();
       

            CreateMap<InventoryCountingViewModel, TInventoryCountingHeader>();
            CreateMap<InventoryCountingLineViewModel, TInventoryCountingLine>();

            CreateMap<InventoryPostingViewModel, TInventoryPostingHeader>();
            CreateMap<InventoryPostingLineViewModel, TInventoryPostingLine>();

            CreateMap<InventoryTransferViewModel, TInventoryTransferHeader>();
            CreateMap<InventoryTransferLineViewModel, TInventoryTransferLine>();

            CreateMap<PromotionViewModel, SPromoHeader>();
            CreateMap<SchemaViewModel, SPromoSchema>();

            CreateMap<LicensePlateHearder, LicensePlateViewModel>();
            CreateMap<LicensePlateLine, LicensePlateLineViewModel>();


            CreateMap<TSalesViewModel, RPFO.Data.EntitiesMWI.TSalesHeader>();
            CreateMap<TInvoiceViewModel, RPFO.Data.EntitiesMWI.TInvoiceHeader>();
            CreateMap<LoyaltyViewModel, SLoyaltyHeader>();
            CreateMap<DeliveryModel, T_DeliveryHeader>();

            CreateMap< GReturnDeliveryViewModel, TReturnHeader> ();

            CreateMap<ReceiptfromProductionViewModel, TReceiptfromProductionHeader> ();
            CreateMap< ProductionOrderViewModel, TProductionOrderHeader> ();
            CreateMap<TableReseachViewModel, MTableInfor>();














        }
    }
}

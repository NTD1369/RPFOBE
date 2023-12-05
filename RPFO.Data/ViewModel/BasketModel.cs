
using RPFO.Data.Entities;
using RPFO.Data.OMSModels;
using RPFO.Data.ViewModel;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public class BasketViewModel 
    {
        public BasketViewModel()
        {
          

        }
        public BasketViewModel(string id )
        {
            Id = id;
           
        }
       
        public string Id { get; set; }
        public Nullable<Boolean> ReturnMode { get; set; }
        public string SalesType { get; set; }
        public string ContractNo { get; set; }
        public string DiscountType { get; set; }
        public string PromotionId { get; set; }
        public Nullable<Boolean> clearPromotion { get; set; }
        public Nullable<Decimal> DiscountValue { get; set; }
        public MCustomer Customer { get; set; }
        public TSalesInvoice Invoice { get; set; }
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public List<BasketItemViewModel> TmpItems { get; set; } = new List<BasketItemViewModel>();
        public List<BasketPaymentViewModel> Payments { get; set; } = new List<BasketPaymentViewModel>();
        public List<BasketItemViewModel> PromotionItems { get; set; } = new List<BasketItemViewModel>();
        public List<BasketItemViewModel> PromoItemApply { get; set; } = new List<BasketItemViewModel>(); 
        public List<TapTapVoucherDetails> VoucherApply { get; set; } = new List<TapTapVoucherDetails>();

        public List<PromotionViewModel> PromotionApply { get; set; } = new List<PromotionViewModel>();
        public MEmployee Employee { get; set; }
    }
}




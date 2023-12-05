using DevExpress.XtraSpreadsheet.Model;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    public partial class SaleController : ControllerBase
    {
        private readonly string HavePromotionIsPromo = "1";

        [HttpPost]
        [Route("AddOrUpdateItem")]
        public async Task<AddOrUpdateViewModel> AddOrUpdateItem(AddOrUpdateViewModel addOrUpdateViewModel)
        {
            if (addOrUpdateViewModel != null && addOrUpdateViewModel.Items != null && addOrUpdateViewModel.Items.Any() && addOrUpdateViewModel.addItemOrder != null)
            {
                decimal quantity = 0M;
                var basketAddItemViewModel = addOrUpdateViewModel.Items.Where((i) => i.Id == addOrUpdateViewModel.addItemOrder.Id && i.Uom == addOrUpdateViewModel.addItemOrder.Uom
                     && i?.BookletNo == addOrUpdateViewModel.addItemOrder?.BookletNo && i.Barcode == addOrUpdateViewModel.addItemOrder.Barcode
                     && i?.IsNegative == addOrUpdateViewModel.addItemOrder?.IsNegative && i?.Custom1 == addOrUpdateViewModel.addItemOrder?.Custom1 && i?.BaseLine == addOrUpdateViewModel.addItemOrder?.BaseLine
                     && ((i.PromotionIsPromo != HavePromotionIsPromo && addOrUpdateViewModel.addItemOrder?.PromotionType != SalesType.FixedQuantity.GetDescription().ToUpper()) || (i.PromotionIsPromo == HavePromotionIsPromo && i.isVoucher == true && i.isVoucher == addOrUpdateViewModel.addItemOrder.isVoucher))
                     && i.IsWeightScaleItem != true).FirstOrDefault();

                var basketAddItemhavePromotionViewModel = addOrUpdateViewModel.Items.Where((i) => i.Id == addOrUpdateViewModel.addItemOrder.Id
                     && i.Uom == addOrUpdateViewModel.addItemOrder.Uom
                     && i?.BookletNo == addOrUpdateViewModel.addItemOrder?.BookletNo
                     && i.Barcode == addOrUpdateViewModel.addItemOrder.Barcode
                     && i?.IsNegative == addOrUpdateViewModel.addItemOrder?.IsNegative
                     && i?.Custom1 == addOrUpdateViewModel.addItemOrder?.Custom1
                     && i?.BaseLine == addOrUpdateViewModel.addItemOrder?.BaseLine
                     && i.PromotionIsPromo != HavePromotionIsPromo
                     && i.IsWeightScaleItem != true && i.PromotionType.ToUpper() == SalesType.FixedQuantity.GetDescription().ToUpper()).FirstOrDefault();

                if (basketAddItemViewModel != null)
                {
                    if (addOrUpdateViewModel.addItemOrder?.IsFixedQty != null && addOrUpdateViewModel.addItemOrder?.IsFixedQty == true
                        && addOrUpdateViewModel.addItemOrder?.DefaultFixedQty != null && addOrUpdateViewModel.addItemOrder?.DefaultFixedQty != 0)
                    {
                        quantity = (addOrUpdateViewModel.addItemOrder.AllowSalesNegative ?? false) == true ? -
                            (addOrUpdateViewModel.addItemOrder?.DefaultFixedQty ?? 1) : (addOrUpdateViewModel.addItemOrder?.DefaultFixedQty ?? 1);
                    }

                    addOrUpdateViewModel.addItemOrder.Quantity = quantity;
                    var lineTotalCal = quantity * addOrUpdateViewModel.addItemOrder.Price;

                    if ((addOrUpdateViewModel.addItemOrder.AllowSalesNegative ?? false) == true)
                    {
                        addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis = addOrUpdateViewModel.addItemOrder.Price;
                        addOrUpdateViewModel.addItemOrder.PromotionLineTotal = quantity * addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis;
                    }

                    if (addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Exchange.GetDescription().ToUpper()
                        || addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Ex.GetDescription().ToUpper()
                        || addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Return.GetDescription().ToUpper())
                    {
                        addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis = addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis ?? addOrUpdateViewModel.addItemOrder.Price;
                        addOrUpdateViewModel.addItemOrder.PromotionLineTotal = quantity * addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis;//itemtoAdd.price;
                    }

                    addOrUpdateViewModel.addItemOrder.isSerial = addOrUpdateViewModel.addItemOrder.isSerial != null && addOrUpdateViewModel.addItemOrder.isSerial == true &&
                        addOrUpdateViewModel.addItemOrder?.CustomField1.ToString().ToUpper() == SalesType.Retail.GetDescription().ToUpper() ? true : false;

                    addOrUpdateViewModel.addItemOrder.isVoucher = (addOrUpdateViewModel.addItemOrder.isVoucher ?? false) == true;
                    addOrUpdateViewModel.addItemOrder.isBOM = (addOrUpdateViewModel.addItemOrder.isBOM ?? false) == true;

                    if ((addOrUpdateViewModel.addItemOrder.isCapacity ?? false) == true)
                    {
                        addOrUpdateViewModel.addItemOrder.LineItems.Add(addOrUpdateViewModel.addItemOrder);
                        foreach (var itemCapa in addOrUpdateViewModel.addItemOrder.LineItems)
                        {
                            if (itemCapa.CapacityValue == null || itemCapa.CapacityValue == default)
                            {
                                itemCapa.CapacityValue = 1;
                            }
                            itemCapa.Quantity = itemCapa.Quantity * itemCapa.CapacityValue;
                        }

                    }

                    if (!string.IsNullOrEmpty(addOrUpdateViewModel.addItemOrder.CustomField1)
                         && (addOrUpdateViewModel.addItemOrder?.CustomField1?.ToUpper() == SalesType.Member.GetDescription().ToUpper()
                         || addOrUpdateViewModel.addItemOrder?.CustomField1?.ToUpper() == SalesType.ClassCss.GetDescription().ToUpper()))
                    {
                        addOrUpdateViewModel.addItemOrder.Quantity = quantity;
                        addOrUpdateViewModel.addItemOrder.LineItems.ForEach(itemMember =>
                        {
                            itemMember.Id = addOrUpdateViewModel.addItemOrder.Id;
                            itemMember.Uom = addOrUpdateViewModel.addItemOrder.Uom;
                            itemMember.Price = addOrUpdateViewModel.addItemOrder.Price;
                            itemMember.Barcode = addOrUpdateViewModel.addItemOrder.Barcode;
                            itemMember.SlocId = addOrUpdateViewModel.addItemOrder.SlocId;
                            itemMember.CustomField1 = addOrUpdateViewModel.addItemOrder.CustomField1;
                            itemMember.CustomField2 = addOrUpdateViewModel.addItemOrder.CustomField2;

                            if (!string.IsNullOrEmpty(addOrUpdateViewModel.addItemOrder.CustomField2))
                            {
                                itemMember.MemberValue = addOrUpdateViewModel.addItemOrder.CustomField2 == null ? 1M : Convert.ToDecimal(addOrUpdateViewModel.addItemOrder.CustomField2);
                            }
                            itemMember.Quantity = 1;
                            itemMember.StartDate = addOrUpdateViewModel.addItemOrder.MemberDate;
                            itemMember.MemberDate = addOrUpdateViewModel.addItemOrder.MemberDate;

                            itemMember.Phone = itemMember.Phone;
                            itemMember.Name = itemMember.Name;

                            DateTime? startDate = itemMember?.StartDate;
                            decimal? monthsToAdd = itemMember.MemberValue * itemMember.Quantity;
                            itemMember.EndDate = startDate?.AddMonths(Convert.ToInt32(monthsToAdd));
                        });
                    }

                    if (!string.IsNullOrEmpty(addOrUpdateViewModel.addItemOrder.CustomField1) && addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Member.GetDescription().ToUpper())
                    {
                        addOrUpdateViewModel.addItemOrder.LineItems.Add(addOrUpdateViewModel.addItemOrder);
                        addOrUpdateViewModel.addItemOrder.Quantity = quantity;
                    }
                    addOrUpdateViewModel.Items.Add(addOrUpdateViewModel.addItemOrder);
                }
                else
                {
                    if (!string.IsNullOrEmpty(addOrUpdateViewModel.addItemOrder.CustomField1) && addOrUpdateViewModel.addItemOrder.CustomField1?.ToUpper() == SalesType.Card.GetDescription().ToUpper())
                    {
                        var isReplace = false;
                        basketAddItemViewModel.LineItems.ForEach(item =>
                        {
                            if (item.Id == addOrUpdateViewModel.addItemOrder.Id && item.Uom == addOrUpdateViewModel.addItemOrder.Uom
                             && item.Barcode == addOrUpdateViewModel.addItemOrder.Barcode && item.PrepaidCardNo == addOrUpdateViewModel.addItemOrder.PrepaidCardNo)
                            {
                                item.Quantity = addOrUpdateViewModel.addItemOrder.Quantity;
                                isReplace = true;
                            }
                        });

                        if (isReplace == false)
                            basketAddItemViewModel.LineItems.Add(addOrUpdateViewModel.addItemOrder);

                        basketAddItemViewModel.Quantity = 0;
                        basketAddItemViewModel.LineTotal = 0;

                        basketAddItemViewModel.LineItems.ForEach(element =>
                        {
                            basketAddItemViewModel.Quantity += element.Quantity;
                        });

                        basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;

                        if (!string.IsNullOrEmpty(basketAddItemViewModel.WeightScaleBarcode) && basketAddItemViewModel.WeightScaleBarcode?.Length > 0
                             && basketAddItemViewModel.PriceWScaleWithCfg == "true")
                        {
                            basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;
                        }

                        if (addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Exchange.GetDescription().ToUpper()
                            || addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Ex.GetDescription().ToUpper())
                        {
                            basketAddItemViewModel.PromotionLineTotal = basketAddItemViewModel.LineTotal;
                        }
                    }
                    else
                    {
                        var isCanUpdate = true;

                        if (addOrUpdateViewModel.addItemOrder.IsFixedQty == true && addOrUpdateViewModel.addItemOrder.DefaultFixedQty != 0M)
                        {
                            var addItemOrder = addOrUpdateViewModel.Items.Where((i) => i.Id == addOrUpdateViewModel.addItemOrder.Id && i.Uom == addOrUpdateViewModel.addItemOrder.Uom
                              && i?.BookletNo == addOrUpdateViewModel.addItemOrder.BookletNo && i.Barcode == addOrUpdateViewModel.addItemOrder.Barcode
                              && i?.IsNegative == addOrUpdateViewModel.addItemOrder.IsNegative
                              && i?.Custom1 == addOrUpdateViewModel.addItemOrder.Custom1 && i?.BaseLine == addOrUpdateViewModel.addItemOrder.BaseLine
                              && ((i.PromotionIsPromo != HavePromotionIsPromo && i.PromotionType.ToUpper() != SalesType.FixedQuantity.GetDescription().ToUpper())
                              || (i.PromotionIsPromo == HavePromotionIsPromo && i.isVoucher == true && i.isVoucher == addOrUpdateViewModel.addItemOrder.isVoucher))
                              && i.IsWeightScaleItem != true).FirstOrDefault();

                            if (addItemOrder != null)
                            {
                                if (addOrUpdateViewModel.addItemOrder.DefaultFixedQty == addItemOrder.Quantity)
                                {
                                    isCanUpdate = false;
                                }
                            }
                        }

                        if (isCanUpdate)
                        {
                            if (!string.IsNullOrEmpty(addOrUpdateViewModel.addItemOrder.CustomField1)
                                && (addOrUpdateViewModel.addItemOrder?.CustomField1?.ToUpper() == SalesType.Member.GetDescription().ToUpper()
                                || addOrUpdateViewModel.addItemOrder?.CustomField1.ToUpper() == SalesType.ClassCss.GetDescription().ToUpper()))
                            {
                                var isReplace = false;
                                basketAddItemViewModel.LineItems.ForEach(element =>
                                {
                                    var addItemOrderBySerialNumber = addOrUpdateViewModel.addItemOrder.LineItems.FirstOrDefault(y => y.SerialNum == element.SerialNum);
                                    if (addItemOrderBySerialNumber != null)
                                    {
                                        if (element.Id == addOrUpdateViewModel.addItemOrder.Id
                                        && element.Uom == addOrUpdateViewModel.addItemOrder.Uom
                                        && element.Barcode == addOrUpdateViewModel.addItemOrder.Barcode
                                        && element.SerialNum == addItemOrderBySerialNumber.SerialNum
                                        && (element.MemberDate?.ToString("DD-MM-YYYY") == addOrUpdateViewModel.addItemOrder.MemberDate?.ToString("DD-MM-YYYY")))
                                        {
                                            element.Quantity = quantity;
                                            element.Phone = addItemOrderBySerialNumber.Phone;
                                            element.Name = addItemOrderBySerialNumber.Name;
                                            isReplace = true;
                                        }
                                    }
                                });

                                if (!isReplace)
                                {
                                    addOrUpdateViewModel.addItemOrder.LineItems.ForEach(itemMember =>
                                    {
                                        itemMember.Id = addOrUpdateViewModel.addItemOrder.Id;
                                        itemMember.Uom = addOrUpdateViewModel.addItemOrder.Uom;
                                        itemMember.Price = addOrUpdateViewModel.addItemOrder.Price;
                                        itemMember.Barcode = addOrUpdateViewModel.addItemOrder.Barcode;
                                        itemMember.SlocId = addOrUpdateViewModel.addItemOrder.SlocId;
                                        itemMember.CustomField1 = addOrUpdateViewModel.addItemOrder.CustomField1;
                                        itemMember.CustomField2 = addOrUpdateViewModel.addItemOrder.CustomField2;
                                        if (!string.IsNullOrEmpty(addOrUpdateViewModel.addItemOrder.CustomField2))
                                            itemMember.MemberValue = addOrUpdateViewModel.addItemOrder.CustomField2 == null ? 1 : Convert.ToDecimal(addOrUpdateViewModel.addItemOrder.CustomField2);
                                        itemMember.Quantity = 1;
                                        itemMember.StartDate = addOrUpdateViewModel.addItemOrder.MemberDate;
                                        itemMember.MemberDate = addOrUpdateViewModel.addItemOrder.MemberDate;
                                        itemMember.Phone = itemMember.Phone;
                                        itemMember.Name = itemMember.Name;

                                        DateTime? startDate = itemMember?.StartDate;
                                        decimal? monthsToAdd = itemMember.MemberValue * itemMember.Quantity;
                                        itemMember.EndDate = startDate?.AddMonths(Convert.ToInt32(monthsToAdd));

                                        basketAddItemViewModel.LineItems.Add(itemMember);
                                    });
                                }

                                basketAddItemViewModel.Quantity = 0;
                                basketAddItemViewModel.LineTotal = 0;

                                basketAddItemViewModel.LineItems.ForEach(element =>
                                {
                                    element.Quantity = element.Quantity;
                                    element.StartDate = element.MemberDate;

                                    if (!string.IsNullOrEmpty(element.CustomField2))
                                        element.MemberValue = element.CustomField2 == null ? 1 : Convert.ToDecimal(element.CustomField2);

                                    element.Phone = element.Phone;
                                    element.Name = element.Name;

                                    DateTime? startDate = element?.StartDate;
                                    decimal? monthsToAdd = element.MemberValue * element.Quantity;

                                    element.EndDate = startDate?.AddMonths(Convert.ToInt32(monthsToAdd));
                                    basketAddItemViewModel.Quantity += element.Quantity;
                                });

                                basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;

                                if (!string.IsNullOrEmpty(basketAddItemViewModel.WeightScaleBarcode) &&
                                    basketAddItemViewModel.WeightScaleBarcode?.Length > 0
                                    && addOrUpdateViewModel.PriceWScaleWithCfg == "true")
                                {
                                    basketAddItemViewModel.Price = basketAddItemViewModel.Price;
                                    basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;
                                }

                                if (addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Exchange.GetDescription().ToUpper()
                                    || addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Ex.GetDescription().ToUpper())
                                {
                                    basketAddItemViewModel.PromotionLineTotal = basketAddItemViewModel.LineTotal;

                                }
                            }
                            else
                            {
                                addOrUpdateViewModel.addItemOrder.isSerial = addOrUpdateViewModel.addItemOrder.isSerial != null && addOrUpdateViewModel.addItemOrder.isSerial == true ? true : false;
                                addOrUpdateViewModel.addItemOrder.isVoucher = (addOrUpdateViewModel.addItemOrder.isVoucher ?? false) == true;
                                addOrUpdateViewModel.addItemOrder.isBOM = (addOrUpdateViewModel.addItemOrder.isBOM ?? false) == true;

                                if ((addOrUpdateViewModel.addItemOrder.isSerial ?? false) == true || (addOrUpdateViewModel.addItemOrder.isVoucher ?? false) == true)
                                {
                                    addOrUpdateViewModel.Quantity = quantity;
                                }
                                else
                                {
                                    addOrUpdateViewModel.Quantity += quantity;

                                    if (!string.IsNullOrEmpty(addOrUpdateViewModel.addItemOrder?.CustomField1)
                                       && (addOrUpdateViewModel.addItemOrder.CustomField1?.ToUpper() == SalesType.TP.GetDescription().ToUpper()
                                       || addOrUpdateViewModel.addItemOrder.CustomField1?.ToUpper() == SalesType.BP.GetDescription().ToUpper()
                                       || addOrUpdateViewModel.addItemOrder.CustomField1?.ToUpper() == SalesType.PN.GetDescription().ToUpper()))
                                    {
                                        basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;

                                        if (!string.IsNullOrEmpty(basketAddItemViewModel.WeightScaleBarcode)
                                                && basketAddItemViewModel.WeightScaleBarcode?.Length > 0
                                                && basketAddItemViewModel.PriceWScaleWithCfg == "true")
                                        {
                                            basketAddItemViewModel.Price = basketAddItemViewModel.Price;
                                            basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;
                                        }
                                        basketAddItemViewModel.PromotionLineTotal = basketAddItemViewModel.LineTotal;
                                        basketAddItemViewModel.PromotionUnitPrice = basketAddItemViewModel.Price;
                                    }

                                    if (addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Exchange.GetDescription().ToUpper()
                                        || addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Ex.GetDescription().ToUpper())
                                    {
                                        basketAddItemViewModel.PromotionPriceAfDis = addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis;
                                        var tempDis = addOrUpdateViewModel.addItemOrder.Price - addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis;

                                        if (addOrUpdateViewModel.addItemOrder.DiscountType.ToUpper() == SalesType.DiscountAmount.GetDescription().ToUpper())
                                        {
                                            basketAddItemViewModel.DiscountValue = tempDis * basketAddItemViewModel.Quantity;
                                        }

                                        basketAddItemViewModel.PromotionDisAmt = tempDis * basketAddItemViewModel.Quantity;
                                        basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;

                                        if (!string.IsNullOrEmpty(basketAddItemViewModel.WeightScaleBarcode)
                                             && basketAddItemViewModel.WeightScaleBarcode?.Length > 0
                                             && basketAddItemViewModel.PriceWScaleWithCfg == "true")
                                        {
                                            basketAddItemViewModel.Price = basketAddItemViewModel.Price;
                                            basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;
                                        }

                                        basketAddItemViewModel.PromotionLineTotal = basketAddItemViewModel.Quantity * addOrUpdateViewModel.addItemOrder.PromotionPriceAfDis;
                                    }


                                    if ((basketAddItemViewModel?.isBOM ?? false) == true)
                                    {
                                        basketAddItemViewModel.LineItems.ForEach((item) =>
                                        {
                                            item.LineTotal = item.Quantity * basketAddItemViewModel.Quantity;
                                        });
                                    }

                                }

                                if ((addOrUpdateViewModel.addItemOrder.isCapacity ?? false) == true)
                                {
                                    var isReplace = false;
                                    basketAddItemViewModel.LineItems.ForEach(element =>
                                    {
                                        if (element.Id == addOrUpdateViewModel.addItemOrder.Id
                                        && element.Uom == addOrUpdateViewModel.addItemOrder.Uom
                                        && element.Barcode == addOrUpdateViewModel.addItemOrder.Barcode
                                        && element.StoreAreaId == addOrUpdateViewModel.addItemOrder.StoreAreaId
                                        && element.TimeFrameId == addOrUpdateViewModel.addItemOrder.TimeFrameId
                                        && element.AppointmentDate == addOrUpdateViewModel.addItemOrder.AppointmentDate)
                                        {
                                            element.Quantity = addOrUpdateViewModel.addItemOrder.Quantity * addOrUpdateViewModel.addItemOrder.CapacityValue;
                                            isReplace = true;
                                        }
                                    });

                                    if (isReplace == false)
                                    {
                                        addOrUpdateViewModel.addItemOrder.LineItems.Add(addOrUpdateViewModel.addItemOrder);
                                    }
                                    if (isReplace)
                                    {
                                        if (basketAddItemhavePromotionViewModel == null)
                                        {
                                            basketAddItemhavePromotionViewModel.Quantity = 0;
                                            basketAddItemhavePromotionViewModel.LineTotal = 0;

                                            basketAddItemhavePromotionViewModel.LineItems.ForEach(itemX =>
                                            {
                                                basketAddItemhavePromotionViewModel.Quantity = basketAddItemhavePromotionViewModel.Quantity + itemX.Quantity;
                                            });
                                            basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;
                                        }
                                    }

                                    basketAddItemViewModel.Quantity = 0;
                                    basketAddItemViewModel.LineTotal = 0;
                                    basketAddItemViewModel.LineItems.ForEach(itemX =>
                                    {
                                        basketAddItemViewModel.Quantity = basketAddItemViewModel.Quantity + itemX.Quantity;
                                    });

                                    basketAddItemViewModel.LineTotal = basketAddItemViewModel.Quantity * basketAddItemViewModel.Price;
                                    if (addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Exchange.GetDescription().ToUpper()
                                        || addOrUpdateViewModel.addItemOrder.CustomField1.ToUpper() == SalesType.Ex.GetDescription().ToUpper())
                                    {
                                        basketAddItemViewModel.PromotionLineTotal = basketAddItemViewModel.LineTotal;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var LineNumber = 1;
            addOrUpdateViewModel.addItemOrder.LineItems.ForEach(item =>
            {
                item.LineNum = LineNumber;
                LineNumber++;
            });

            var jsonM = addOrUpdateViewModel.ToJson();
            RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.Log\\", "addOrUpdate_TypeScrpit", "addOrUpdate_TypeScrpit", addOrUpdateViewModel.ToJson());

            return await Task.FromResult(addOrUpdateViewModel);
        }
    }
}
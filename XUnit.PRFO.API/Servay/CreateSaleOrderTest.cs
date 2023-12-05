using Dapper;
using Newtonsoft.Json;
using NUglify.JavaScript;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using XUnit.PRFO.API;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static DevExpress.XtraPrinting.Export.Pdf.PdfImageCache;

namespace Test.PRFO.API.Survery
{

    public class CreateSaleOrderTest : IClassFixture<DependencyInjectionFixture>
    {
        protected DependencyInjectionFixture _fixture { get; set; }

        public CreateSaleOrderTest(DependencyInjectionFixture fixture
            )
        {
            _fixture = fixture;
        }

        [Fact]
        public async void RunCreateSaleOrder()
        {
            string folderPath = @"C:\RPFO.API.Log\SalesOrders\20230821";
            var modelSaleOrders = new List<SaleViewModel>();
            try
            {
                string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");
                foreach (string filePath in txtFiles)
                {
                    try
                    {
                        string txtContent = File.ReadAllText(filePath);
                        var sale = JsonConvert.DeserializeObject<SaleViewModel>(txtContent);
                        modelSaleOrders.Add(sale);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading {Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }
                foreach (var item in modelSaleOrders)
                {
                    await TestCreateSaleOrder(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task<GenericResult> TestCreateSaleOrder(SaleViewModel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(model.DataSource))
                {
                    result.Success = false;
                    result.Message = "Data Source can't null.";
                    Assert.Equal(result.Success.ToString(), result.Message);
                }

                #region kiểm tra store có kiểm tra payment trong store và ( model.IsCanceled == "C" is "CÓ" || model.IsCanceled == "Y" is "YES" ) sau đó kiểm tra payments

                if (model.Payments != null && model.Payments.Count > 0)
                {
                    var paymentOfStore = await _fixture.PaymentMethodService.GetByStore(model.CompanyCode, model.StoreId);

                    if (paymentOfStore != null && paymentOfStore.Success)
                    {
                        var payments = paymentOfStore.Data as List<StorePaymentViewModel>;
                        if (payments != null && payments.Count > 0)
                        {
                            var PaymentInStore = payments.Select(x => x.PaymentCode).Distinct();
                            var PaymentInData = model.Payments.Select(x => x.PaymentCode).Distinct();

                            string checkStr = await CheckPaymentList(PaymentInStore, PaymentInData);
                            if (!string.IsNullOrEmpty(checkStr))
                            {
                                result.Success = false;
                                result.Message = "Payments:" + checkStr + " not in existed in Store: " + model.StoreId;
                                Assert.Equal(result.Success.ToString(), result.Message);
                            }

                            if (model.IsCanceled == "C" || model.IsCanceled == "Y")
                            {
                                foreach (var payment in model.Payments)
                                {
                                    var paymentMaster = payments.Where(x => x.PaymentCode == payment.PaymentCode).FirstOrDefault();
                                    if (paymentMaster != null)
                                    {
                                        if (paymentMaster.RejectVoid == true)
                                        {
                                            result.Success = false;
                                            result.Message = "Payments: " + paymentMaster.PaymentDesc + " reject void.";
                                            Assert.Equal(result.Success.ToString(), result.Message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                #region General Setting Store

                var settingData = await _fixture.SettingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                List<GeneralSettingStore> settingList = settingData.Success ?
                           settingData.Data as List<GeneralSettingStore> : new List<GeneralSettingStore>();

                #endregion

                #region nếu TransId bằng null và  model.DataSource == "GRAB"

                if (string.IsNullOrEmpty(model.TransId) && model.DataSource == "GRAB")
                {
                    string socheck = _fixture.SaleService.RunGRABTest(model);
                    model.TransId = !string.IsNullOrEmpty(socheck) ? socheck : model.TransId;
                }

                #endregion

                #region Trường hợp model.DataSource.ToLower() == "pos" và nó khác "h" và "hold"

                if (model.DataSource.ToLower() == "pos"
                    && model.Status.ToLower() != "h"
                    && model.Status.ToLower() != "hold")
                {
                    var Data = await _fixture.ShiftService.GetByCode(model.CompanyCode, model.ShiftId);
                    var shift = Data.Data as ShiftViewModel;
                    if (shift == null || shift.Status == "C")
                    {
                        result.Success = false;
                        result.Message = "Shift " + model.ShiftId + " not exist or has closed.";
                        Assert.Equal(result.Success.ToString(), result.Message);
                    }
                    var salesZeroValue = settingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "SalesZeroValue").FirstOrDefault();
                    if (salesZeroValue != null && (salesZeroValue.SettingValue == "false" || salesZeroValue.SettingValue == "0"))
                    {
                        var checkItem = model.Lines.Where(x => x.Price == 0 && x.IsPromo != "1" && (x.BomId == null || x.BomId == "")).ToList();
                        if (checkItem != null && checkItem.Count > 0)
                        {
                            result.Success = false;
                            result.Message = "Can't completed order. Price of item " + checkItem[0].ItemName + " invalid";
                            Assert.Equal(result.Success.ToString(), result.Message);
                        }
                    }
                }

                #endregion

                #region validation of companyCode and Lines and StoreId and CusId , SaleModel

                if (model.CompanyCode == null || string.IsNullOrEmpty(model.CompanyCode))
                {
                    result.Success = false;
                    result.Message = "Company Code cannot null.";
                    Assert.Equal(result.Success.ToString(), result.Message);
                }
                if (model.Lines == null || model.Lines.Count() == 0)
                {
                    result.Success = false;
                    result.Message = "Doc line cannot null.";
                    Assert.Equal(result.Success.ToString(), result.Message);
                }
                if (model.StoreId == null || string.IsNullOrEmpty(model.StoreId))
                {
                    result.Success = false;
                    result.Message = "Store cannot null.";
                    Assert.Equal(result.Success.ToString(), result.Message);
                }
                if (string.IsNullOrEmpty(model.CusId))
                {
                    result.Success = false;
                    result.Message = "Customer cannot null.";
                    Assert.Equal(result.Success.ToString(), result.Message);
                }
                if (string.IsNullOrEmpty(model.SalesMode))
                {
                    result.Success = false;
                    result.Message = "SalesMode cannot null.";
                    Assert.Equal(result.Success.ToString(), result.Message);
                }

                #endregion

                #region model.Payments and  model.SalesMode.ToLower() != "return" 

                if (model.Payments != null && model.Payments.Count > 0
                    && model.SalesMode.ToLower() != "return"
                    && model.SalesMode.ToLower() != "ex")
                {
                    if (model.IsCanceled != "C")
                    {
                        var paymentChargables = model.Payments.Where(x => Math.Abs(x.ChargableAmount.HasValue ? x.ChargableAmount.Value : 0) == 0).ToList();
                        if (paymentChargables != null && paymentChargables.Count > 0)
                        {
                            var fpaymentChargables = paymentChargables.FirstOrDefault();
                            result.Success = false;
                            result.Message = "Please complete progress payment. " + fpaymentChargables.PaymentCode + " chargable value 0:";
                            Assert.Equal(result.Success.ToString(), result.Message);
                        }
                        var salesLineNegative = model.Lines.Where(x => !x.AllowSalesNegative.HasValue ? false : x.AllowSalesNegative == true).ToList();
                        if (salesLineNegative.Count <= 0)
                        {
                            var payment = model.Payments.Where(x => x.CollectedAmount <= 0).ToList();
                            if (payment != null && payment.Count > 0)
                            {
                                result.Success = false;
                                result.Message = "Please complete progress payment. Can't payment with value 0";
                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                    }

                }

                #endregion

                #region model.OrderId is not null check

                if (!string.IsNullOrEmpty(model.OrderId.ToString()))
                {
                    var socheck = await _fixture.SaleService.GetUSP_S_T_SalesHeaderByOrderIdAsync(model);

                    if (socheck != null && socheck.Count > 0)
                    {
                        var firstTran = socheck.FirstOrDefault();
                        if (firstTran != null)
                        {
                            result.Success = false;
                            result.Message = "Order has been created or processing. Trans Id: " + firstTran.TransId + " Order Id: " + model.OrderId;
                            string checkTransId = await _fixture.SaleService.CheckOrderDataTest(model.CompanyCode, model.StoreId, model.TransId, model.TotalAmount, model.Lines.Count(), model.Lines.Sum(x => x.Quantity));

                            if (!string.IsNullOrEmpty(checkTransId))
                            {
                                result.Data = checkTransId;
                            }
                            string Folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs");

                            if (!Directory.Exists(Folder))
                                Directory.CreateDirectory(Folder);

                            string filename = firstTran.TransId + "Double";
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Logs", "");
                            LogUtils.WriteLogData(path, "", filename, model.ToJson());

                            return result;
                        }

                    }
                }

                #endregion

                #region kiểm tra giá cả trong payments sum lại coi có đúng vs giá tổng thu hay không
                {
                    decimal? numOfPayment = 0;
                    foreach (var line in model.Payments)
                    {
                        numOfPayment += line.CollectedAmount;
                    }
                    if (Math.Abs((decimal)numOfPayment) != Math.Abs((decimal)model.TotalPayable))
                    {
                        result.Success = false;
                        result.Message = "Please check return amount. Return amount can't different collected amount.";
                        Assert.Equal(result.Success.ToString(), result.Message);
                    }

                }
                #endregion

                #region IsCanceled == "C" && Status != "H" && Status != "Hold"

                if (string.IsNullOrEmpty(model.IsCanceled))
                    model.IsCanceled = "N";

                if (model.IsCanceled == "C" && model.Status != "H" && model.Status != "Hold")
                {
                    string userCheck = model.ApprovalId ?? model.CreatedBy;
                    var checkCancel = await _fixture.PermissionService.CheckFunctionByUserName(model.CompanyCode, userCheck, "Spc_CancelOrder", "", "I");
                    if (!(checkCancel != null && checkCancel.Success))
                        return checkCancel;
                }

                #endregion

                #region model.DataSource.ToLower() != "pos" và kiểm tra các giá trị ko được null và empty trong model.lines            

                if (model.DataSource.ToLower() != "pos")
                {
                    #region Check validation is null or empty TotalAmount and TotalPayable and TotalDiscountAmt and TotalReceipt and PaymentDiscount and TotalTax

                    if (string.IsNullOrEmpty(model.TotalAmount.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Amount. Total Amount can't null";

                        Assert.Equal(result.Success.ToString(), result.Message);
                    }

                    if (string.IsNullOrEmpty(model.TotalPayable.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Payable. Total Payable can't null";

                        Assert.Equal(result.Success.ToString(), result.Message);
                    }

                    if (string.IsNullOrEmpty(model.TotalDiscountAmt.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Discount Amount. Total Discount Amount can't null";

                        Assert.Equal(result.Success.ToString(), result.Message);
                    }

                    if (string.IsNullOrEmpty(model.TotalReceipt.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Receipt. Total Receipt can't null";

                        Assert.Equal(result.Success.ToString(), result.Message);
                    }

                    if (string.IsNullOrEmpty(model.PaymentDiscount.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Payment Discount. Payment Discount can't null";

                        Assert.Equal(result.Success.ToString(), result.Message);
                    }

                    if (string.IsNullOrEmpty(model.TotalTax.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Tax. Total Tax can't null";

                        Assert.Equal(result.Success.ToString(), result.Message);
                    }

                    #endregion

                    #region Model lines 

                    if (model.Lines != null)
                    {
                        #region Kiểm tra số lượng bị null và empty or quantity bằng 0

                        var qtyCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Quantity.ToString()) || x.Quantity == 0).ToList();
                        string mes = "";

                        if (qtyCheck != null && qtyCheck.Count > 0)
                        {
                            foreach (var line in qtyCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Quantity of items: " + mes.Substring(0, mes.Length - 2);
                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                        #endregion

                        #region Kiểm tra Price is null or empty

                        var priceCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Price.ToString())).ToList();

                        if (priceCheck != null && priceCheck.Count > 0)
                        {
                            foreach (var line in priceCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Price of items: " + mes.Substring(0, mes.Length - 2);
                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                        #endregion

                        #region Kiểm tra LineTotal is null or empty

                        var lineTotalCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.LineTotal.ToString())).ToList();

                        if (lineTotalCheck != null && lineTotalCheck.Count > 0)
                        {
                            foreach (var line in lineTotalCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Line Total of items: " + mes.Substring(0, mes.Length - 2);

                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                        #endregion

                        #region Kiểm tra ItemCode is null or empty

                        var itemCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.ItemCode.ToString())).ToList();

                        if (itemCodeCheck != null && itemCodeCheck.Count > 0)
                        {
                            result.Success = false;
                            result.Message = "Please Input Item Code";

                            Assert.Equal(result.Success.ToString(), result.Message);
                        }

                        #endregion

                        #region Kiểm tra discount rate is null or empty

                        var DiscountRateCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.DiscountRate.ToString())).ToList();

                        if (DiscountRateCheck != null && DiscountRateCheck.Count > 0)
                        {
                            foreach (var line in DiscountRateCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Discount Rate of items: " + mes.Substring(0, mes.Length - 2);

                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                        #endregion

                        #region Kiểm tra TaxRate is null or empty

                        var TaxRateCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxRate.ToString())).ToList();

                        if (TaxRateCheck != null && TaxRateCheck.Count > 0)
                        {
                            foreach (var line in TaxRateCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Tax Rate of items: " + mes.Substring(0, mes.Length - 2);

                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                        #endregion

                        #region Kiêm tra is null or empty  

                        var TaxAmtCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxAmt.ToString())).ToList();

                        if (TaxAmtCheck != null && TaxAmtCheck.Count > 0)
                        {
                            foreach (var line in TaxAmtCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Tax Amt of items: " + mes.Substring(0, mes.Length - 2);

                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                        #endregion

                        #region Kiển tra uomCode is null or empty

                        var UOMCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.UomCode)).ToList();

                        if (UOMCodeCheck != null && UOMCodeCheck.Count > 0)
                        {
                            foreach (var line in UOMCodeCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input UOM Code of items: " + mes.Substring(0, mes.Length - 2);

                                Assert.Equal(result.Success.ToString(), result.Message);
                            }
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion

                #region Trường model.DataSource.ToLower() == "pos" vẫn kiểm tra  Quantity

                if (model.Lines != null)
                {
                    var qtyCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Quantity.ToString()) || x.Quantity == 0).ToList();
                    string mes = "";
                    if (qtyCheck != null && qtyCheck.Count > 0)
                    {
                        foreach (var line in qtyCheck)
                        {
                            mes += line.ItemCode + ", ";
                        }
                        if (!string.IsNullOrEmpty(mes))
                        {
                            result.Success = false;
                            result.Message = "Please Input Quantity of items: " + mes.Substring(0, mes.Length - 2);

                            Assert.Equal(result.Success.ToString(), result.Message);
                        }
                    }
                }

                #endregion

                #region model.IsCanceled nó không phải là "C" thì kiểm tra BomId và Taxcode 

                if (model.IsCanceled != "C")
                {
                    var TaxCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.BomId) && string.IsNullOrEmpty(x.TaxCode)).ToList();

                    if (TaxCodeCheck != null && TaxCodeCheck.Count > 0)
                    {
                        string mes = "";
                        foreach (var line in TaxCodeCheck)
                        {
                            mes += line.ItemCode + ", ";
                        }
                        if (!string.IsNullOrEmpty(mes))
                        {
                            result.Success = false;
                            result.Code = 701;
                            result.Message = "Tax Code of items: " + mes.Substring(0, mes.Length - 2) + " null.";

                            Assert.Equal(result.Success.ToString(), result.Message);
                        }
                    }
                }

                #endregion

                #region model.DataSource.ToLower() == "pos" && model.IsCanceled != "C" && model.Payments != null && model.Payments.Count > 0

                if (model.DataSource.ToLower() == "pos" && model.IsCanceled != "C" && model.Payments != null && model.Payments.Count > 0)
                {
                    decimal? numOfPayment = 0;
                    decimal? numOfLine = 0;
                    foreach (var line in model.Payments)
                    {
                        if (string.IsNullOrEmpty(line.PaymentMode))
                        {
                            line.PaymentMode = "";
                        }
                    }

                    foreach (var line in model.Payments.Where(x => x.PaymentMode.ToLower() != "change"))
                    {
                        numOfPayment += line.CollectedAmount;// * (line.Rate ?? 1);
                    }
                    var lineExcludeBOM = model.Lines.Where(x => string.IsNullOrEmpty(x.BomId) && x.IsPromo != "Y" && x.IsPromo != "1");
                    decimal discountLine = 0;
                    foreach (var line in lineExcludeBOM)
                    {
                        if (string.IsNullOrEmpty(line.DiscountType))
                        {
                            line.DiscountType = "";
                        }
                        if (!line.DiscountType.Contains("Bonus"))
                        {
                            decimal discountNum = line.DiscountAmt == null ? 0 : (decimal)line.DiscountAmt;
                            //line.LineTotal.Value
                            decimal linetotal = line.Quantity.Value * line.Price.Value;
                            var lineDiscount = Math.Abs(line.DiscountAmt ?? 0);
                            if (model.SalesMode.ToLower() == "return")
                            {
                                numOfLine += Math.Abs(linetotal) - Math.Abs(discountNum);
                            }
                            else
                            {
                                if (linetotal < 0)
                                {
                                    numOfLine += -(Math.Abs(linetotal) - Math.Abs(discountNum));
                                    lineDiscount = -lineDiscount;
                                }
                                else
                                {
                                    numOfLine += linetotal - Math.Abs(discountNum);
                                }

                            }
                            discountLine += lineDiscount;
                        }

                    }
                    decimal discountTotal = model.TotalDiscountAmt == null ? 0 : (decimal)model.TotalDiscountAmt;
                    decimal roudingoff = model.RoundingOff ?? 0;

                    decimal totalPayable = Math.Abs((decimal)numOfLine) - Math.Abs(discountTotal) + roudingoff;
                    decimal maxPercent = totalPayable - (totalPayable * 5) / 100;

                    if (Math.Abs((decimal)numOfPayment) < maxPercent) //&& model.SalesMode.ToLower() != "return"
                    {
                        result.Success = false;
                        result.Message = "501: Please check bill and amount. Collected Amount: " + numOfPayment + ", Total Payable " + totalPayable;
                        return result;
                    }

                    decimal discountCheck = Math.Round(discountLine + Math.Abs(model.DiscountAmount ?? 0), 0);

                    string currencyOff = await _fixture.SaleService.GetGetRoundingPaymentDifByDefCurStoreAsync(model);

                    decimal paymentDif = decimal.Parse(currencyOff);

                    if ((Math.Round(Math.Abs(discountTotal), 0) + paymentDif) < discountCheck)
                    {
                        result.Success = false;
                        result.Message = "502: Please check bill and amount. Discount Total: " + discountTotal + ", Discount Line " + discountCheck;
                        return result;
                    }
                }

                #endregion

                #region model.DataSource.ToLower() == "pos" && model.IsCanceled != "C" && string.IsNullOrEmpty(model.ContractNo)

                if (model.DataSource.ToLower() == "pos" && model.IsCanceled != "C" && string.IsNullOrEmpty(model.ContractNo))
                {
                    decimal roudingoff = model.RoundingOff ?? 0;
                    var TotalAmount = Math.Abs((model.TotalAmount ?? 0) - roudingoff);
                    var TotalDiscount = Math.Abs(model.TotalDiscountAmt.Value);
                    var TotalPayable = Math.Abs(model.TotalPayable.Value);
                    decimal perTotalPayable = Math.Abs(model.TotalPayable.Value) * 5 / 100;


                    if (TotalAmount - TotalDiscount > TotalPayable + perTotalPayable && model.SalesMode.ToLower() == "sales" && model.Status != "H")
                    {
                        result.Success = false;
                        result.Message = "101: Please check your receipt amount. Total Amount: " + model.TotalAmount.Value.ToString("C2") + " Total Discount Amt: " + model.TotalDiscountAmt.Value.ToString("C2") + " TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                    string currencyOff = await _fixture.SaleService.GetGetRoundingPaymentDifByDefCurStoreAsync(model);
                    decimal paymentDif = decimal.Parse(currencyOff);
                    if (model.TotalPayable > (model.TotalReceipt + paymentDif) && model.SalesMode.ToLower() == "sales" && model.Status != "H")
                    {
                        result.Success = false;
                        result.Message = "102: Please check your receipt amount. Total Payable: " + model.TotalPayable.Value.ToString("C2") + " Total Receipt: " + model.TotalReceipt.Value.ToString("C2") + " TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                    if (TotalAmount - TotalDiscount > TotalPayable && model.SalesMode.ToLower() != "sales" && model.Status == "H")
                    {
                        result.Success = false;
                        result.Message = "103: Please check your receipt amount. Total Amount: " + model.TotalAmount.Value.ToString("C2") + " Total Discount Amt: " + model.TotalDiscountAmt.Value.ToString("C2") + "  TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                }

                #endregion

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;                

                Assert.True("Show is not case error: " == ex.Message);
            }

            return await Task.FromResult(result);
        }

        private static async Task<string> CheckPaymentList<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            string resultStr = "";
            foreach (var item in l2)
            {
                if (!l1.Contains(item))
                {
                    resultStr += item + ",";

                }
            }
            if (resultStr.Length > 0)
            {
                resultStr = resultStr.Substring(0, resultStr.Length - 1);
            }
            return await Task.FromResult(resultStr);
        }
    }
}

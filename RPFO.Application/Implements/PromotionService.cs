using AutoMapper;
using Dapper;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess.Sql;
using Microsoft.VisualBasic;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Models;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace RPFO.Application.Implements
{
    //public class Variance
    //{
    //    public string PropertyName { get; set; }
    //    public object valA { get; set; }
    //    public object valB { get; set; }
    //}

    //public static class Comparision
    //{
    //    public static List<Variance> Compare<T>(this T val1, T val2)
    //    {
    //        var variances = new List<Variance>();
    //        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //        foreach (var property in properties)
    //        {
    //            var v = new Variance
    //            {
    //                PropertyName = property.Name,
    //                valA = property.GetValue(val1),
    //                valB = property.GetValue(val2)
    //            };
    //            if (v.valA == null && v.valB == null)
    //            {
    //                continue;
    //            }
    //            if (
    //                (v.valA == null && v.valB != null)
    //                ||
    //                (v.valA != null && v.valB == null)
    //            )
    //            {
    //                variances.Add(v);
    //                continue;
    //            }
    //            if (!v.valA.Equals(v.valB))
    //            {
    //                variances.Add(v);
    //            }
    //        }
    //        return variances;
    //    }
    //}

    public class PromotionService : IPromotionService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<SPromoHeader> sPromotion;

        private Dictionary<string, SchemaViewModel> dicPromoSchemas = new Dictionary<string, SchemaViewModel>();
        private Dictionary<string, PromotionViewModel> dicPromotions = new Dictionary<string, PromotionViewModel>();
        //private SchemaViewModel SchemaDefault = null;

        private double txtTotalPayable;
        int RoundLengthQty = 08;
        TimeSpan timeCachePromo = TimeSpan.FromMinutes(20);
        string PrefixCachePromo = "PROMOTION-{0}";
        string PrefixCacheSchema = "SCHEMA-{0}";

        private IResponseCacheService cacheService;
        private ICommonService commonService;

        public PromotionService(IMapper mapper, IGenericRepository<SPromoHeader> _sPromotion, IResponseCacheService responseCacheService, ICommonService _commonService)
        {
            this._mapper = mapper;
            this.sPromotion = _sPromotion;
            this.cacheService = responseCacheService;
            this.commonService = _commonService;

            string dbName = _sPromotion.GetConnection().Database;
            if (!string.IsNullOrEmpty(dbName))
            {
                PrefixCachePromo = "PROMOTION-{0}-" + dbName;
                PrefixCacheSchema = "SCHEMA-{0}-" + dbName;
            }
        }

        public List<PromotionResultViewModel> ImportData(DataImport model)
        {
            List<PromotionResultViewModel> resultlist = new List<PromotionResultViewModel>();
            foreach (var item in model.Promotion)
            {
                item.CreatedBy = model.CreatedBy;
                item.CompanyCode = model.CompanyCode;

                var itemResult = InsertUpdatePromotion(item, out string promoId, out string msg);
                PromotionResultViewModel itemRs = _mapper.Map<PromotionResultViewModel>(item);
                itemRs.Success = itemResult;
                itemRs.PromoId = promoId;
                itemRs.Message = msg;
                resultlist.Add(itemRs);
            }

            return resultlist;
        }

        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<PromotionResultViewModel> resultlist = new List<PromotionResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Promotion)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;

        //            var itemResult = InsertUpdatePromotion(item, out string msg);
        //            //if (itemResult.Success == false)
        //            //{
        //            PromotionResultViewModel itemRs = new PromotionResultViewModel();
        //            itemRs = _mapper.Map<PromotionResultViewModel>(item);
        //            //if(itemResult == true)
        //            //{

        //            //}   
        //            //else
        //            //{

        //            //}    
        //            itemRs.Success = itemResult;
        //            itemRs.Message = msg;
        //            resultlist.Add(itemRs);
        //            //}
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        //result.Data = failedlist;
        //    }
        //    return result;
        //}

        public List<MPromoType> GetPromoTypes(out string msg)
        {
            List<MPromoType> promoTypes = new List<MPromoType>();
            msg = "";
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var r = db.Query<MPromoType>("USP_S_M_PromoType", commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        promoTypes = r.ToList();
                    }
                }
                catch (Exception ex)
                {
                    msg += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return promoTypes;
        }

        public List<PromoHeaderViewModel> SearchPromo(string companyCode, string promoId, int? promotype, string promoName, string customerType, string customerValue, DateTime? validDateFrom, DateTime? validDateTo, int? validTimeFrom, int? validTimeTo, string isMon, string isTue, string isWed, string isThu, string isFri, string isSat, string isSun, string isCombine, string status)
        {
            List<PromoHeaderViewModel> promos = new List<PromoHeaderViewModel>();
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("PromoId", promoId);
                    parameters.Add("PromoType", promotype);
                    parameters.Add("PromoName", promoName);
                    parameters.Add("CustomerType", customerType);
                    parameters.Add("CustomerValue", customerValue);
                    parameters.Add("ValidDateFrom", validDateFrom == null ? null : validDateFrom.Value.ToString(StringFormatConst.SQLDateParam));
                    parameters.Add("ValidDateTo", validDateTo == null ? null : validDateTo.Value.ToString(StringFormatConst.SQLDateParam));
                    parameters.Add("ValidTimeFrom", validTimeFrom);
                    parameters.Add("ValidTimeTo", validTimeTo);
                    parameters.Add("IsMon", isMon);
                    parameters.Add("IsTue", isTue);
                    parameters.Add("IsWed", isWed);
                    parameters.Add("IsThu", isThu);
                    parameters.Add("IsFri", isFri);
                    parameters.Add("IsSat", isSat);
                    parameters.Add("IsSun", isSun);
                    parameters.Add("IsCombine", isCombine);
                    parameters.Add("Status", status);
                    string validDateFromStr = validDateFrom == null ? null : validDateFrom.Value.ToString(StringFormatConst.SQLDateParam);
                    string validDateToStr = validDateFrom == null ? null : validDateTo.Value.ToString(StringFormatConst.SQLDateParam);

                    //string query = $"USP_S_S_SearchPromo '{companyCode}','{promoId}','{promotype}','{promoName}','{customerType}'" +
                    //    $",'{customerValue}','{validDateFromStr}','{validDateToStr}','{validTimeFrom}','{validTimeTo}','{isMon}'" +
                    //    $",'{isTue}','{isWed}','{isThu}','{isFri}','{isSat}','{isSun}','{isCombine}','{status}'";
                    //var r = db.Query<PromoHeaderViewModel>(query, param: parameters, commandType: CommandType.Text);

                    var r = db.Query<PromoHeaderViewModel>("USP_S_S_SearchPromo", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        promos = r.ToList();
                    }
                }
                catch { }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return promos;
        }

        public bool CheckUsed(string companyCode, string promotionId)
        {
            bool result = false;
            using (IDbConnection db = sPromotion.GetConnection())
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("PromoId", promotionId);
                    var r = db.Query("USP_C_Promotion_CheckIsUsed", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Count() > 0)
                    {
                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        public async Task<GenericResult> Remove(string companyCode, string promotionId)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(companyCode))
            {
                result.Success = false;
                result.Message = "CompanyCode must be not null.";
                return result;
            }

            using (IDbConnection db = sPromotion.GetConnection())
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    bool checkRs = false;
                    var rcheck = await db.QueryAsync($"USP_C_Promotion_CheckIsUsed '{companyCode}','{promotionId}'", null, commandType: CommandType.Text);
                    if (rcheck.Count() > 0)
                    {
                        checkRs = true;
                    }
                    if (checkRs == false)
                    {
                        string q = $"USP_D_Promotion '{companyCode}','{promotionId}'";
                        var r = db.Execute(q, null, commandType: CommandType.Text);

                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Promotion is in use";
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            return result;
        }

        public PromotionViewModel GetPromotion(string companyCode, string promoId, out string msg)
        {
            PromotionViewModel promotion = null;
            msg = "";
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("PromoId", promoId);
                    var r = db.Query<SPromoHeader>("USP_S_S_PromoHeader", parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        promotion = _mapper.Map<PromotionViewModel>(r.FirstOrDefault());

                        var cus = db.Query<SPromoCustomer>("USP_S_S_PromoCustomer", parameters, commandType: CommandType.StoredProcedure);
                        if (cus.Any())
                        {
                            promotion.PromoCustomers = cus.ToList();
                        }
                        else
                        {
                            promotion.PromoCustomers = new List<SPromoCustomer>();
                        }
                        var store = db.Query<SPromoStore>("USP_S_S_PromoStore", parameters, commandType: CommandType.StoredProcedure);
                        if (store.Any())
                        {
                            promotion.PromoStores = store.ToList();
                        }
                        else
                        {
                            promotion.PromoStores = new List<SPromoStore>();
                        }
                        var buy = db.Query<SPromoBuy>("USP_S_S_PromoBuy", parameters, commandType: CommandType.StoredProcedure);
                        if (buy.Any())
                        {
                            promotion.PromoBuys = buy.ToList();
                        }
                        else
                        {
                            promotion.PromoBuys = new List<SPromoBuy>();
                        }

                        var get = db.Query<SPromoGet>("USP_S_S_PromoGet", parameters, commandType: CommandType.StoredProcedure);
                        if (get.Any())
                        {
                            promotion.PromoGets = get.ToList();
                        }
                        else
                        {
                            promotion.PromoGets = new List<SPromoGet>();
                        }

                        var buyOTGroup = db.Query<SPromoOTGroup>("USP_S_S_PromoBuyOTGroup", parameters, commandType: CommandType.StoredProcedure);
                        if (buyOTGroup.Any())
                        {
                            List<SPromoOTGroup> buyOTGroups = buyOTGroup.ToList();
                            foreach (SPromoBuy promoBuy in promotion.PromoBuys)
                            {
                                if (promoBuy.LineType == PromoLineType.OneTimeGroup)
                                {
                                    var otGroup = buyOTGroup.Where(s => s.GroupID == promoBuy.LineCode);
                                    if (otGroup.Any() && otGroup.Count() > 0)
                                    {
                                        promoBuy.Lines = otGroup.ToList();
                                    }
                                }
                            }
                        }

                        var getOTGroup = db.Query<SPromoOTGroup>("USP_S_S_PromoGetOTGroup", parameters, commandType: CommandType.StoredProcedure);
                        if (getOTGroup.Any())
                        {
                            List<SPromoOTGroup> buyOTGroups = getOTGroup.ToList();
                            foreach (SPromoGet promoGet in promotion.PromoGets)
                            {
                                if (promoGet.LineType == PromoLineType.OneTimeGroup)
                                {
                                    var otGroup = getOTGroup.Where(s => s.GroupID == promoGet.LineCode);
                                    if (otGroup.Any() && otGroup.Count() > 0)
                                    {
                                        promoGet.Lines = otGroup.ToList();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        msg = "Cannot find promotion id: " + promoId;
                    }
                }
                catch (Exception ex)
                {
                    msg += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            if (promotion != null)
            {
                cacheService.CacheData(promotion, string.Format(PrefixCachePromo, promoId), timeCachePromo);
            }

            return promotion;
        }

        public bool InsertUpdatePromotion(PromotionViewModel promotion, out string promotionId, out string msg)
        {
            bool result = false;
            promotionId = "";
            msg = "";
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    DataTable dtHeader = CreateTablePromoHeader();
                    DataRow drHeader = dtHeader.NewRow();

                    if (string.IsNullOrEmpty(promotion.PromoId))
                    {
                        string key = sPromotion.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('PR','{promotion.CompanyCode}', '')", null, commandType: CommandType.Text).ToString();
                        promotion.PromoId = key;
                    }
                    else
                    {
                        var paramCheck = new DynamicParameters();
                        paramCheck.Add("PromoId", promotion.PromoId);
                        paramCheck.Add("CompanyCode", promotion.CompanyCode);
                        var r = db.Query<SPromoHeader>("USP_S_S_PromoHeader", paramCheck, commandType: CommandType.StoredProcedure);
                        if (r.Any() && r.Count() > 0)
                        {
                            var rcheck = db.Query($"USP_C_Promotion_CheckIsUsed '{promotion.CompanyCode}','{promotion.PromoId}'", commandType: CommandType.Text);
                            if (rcheck.Count() > 0)
                            {
                                //  chỉ cho phép lưu status với trường hợp promotion đã được sử dụng
                                SPromoHeader promoHeader = r.FirstOrDefault();
                                if (promoHeader.Status == "Y" && promotion.Status == "N")
                                {
                                    var paramUpdate = new DynamicParameters();

                                    paramUpdate.Add("PromoId", promoHeader.PromoId);
                                    paramUpdate.Add("CompanyCode", promoHeader.CompanyCode);
                                    paramUpdate.Add("PromoType", promoHeader.PromoType);
                                    paramUpdate.Add("PromoName", promoHeader.PromoName);
                                    paramUpdate.Add("CustomerType", promoHeader.CustomerType);
                                    paramUpdate.Add("ValidDateFrom", promoHeader.ValidDateFrom == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : promoHeader.ValidDateFrom.Value.ToString(StringFormatConst.SQLDateParam));
                                    paramUpdate.Add("ValidDateTo", promoHeader.ValidDateTo == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : promoHeader.ValidDateTo.Value.ToString(StringFormatConst.SQLDateParam));
                                    paramUpdate.Add("ValidTimeFrom", promoHeader.ValidTimeFrom ?? 0);
                                    paramUpdate.Add("ValidTimeTo", promoHeader.ValidTimeTo ?? 2359);
                                    paramUpdate.Add("IsMon", promoHeader.IsMon);
                                    paramUpdate.Add("IsTue", promoHeader.IsTue);
                                    paramUpdate.Add("IsWed", promoHeader.IsWed);
                                    paramUpdate.Add("IsThu", promoHeader.IsThu);
                                    paramUpdate.Add("IsFri", promoHeader.IsFri);
                                    paramUpdate.Add("IsSat", promoHeader.IsSat);
                                    paramUpdate.Add("IsSun", promoHeader.IsSun);
                                    paramUpdate.Add("TotalBuyFrom", promoHeader.TotalBuyFrom);
                                    paramUpdate.Add("TotalBuyTo", promoHeader.TotalBuyTo);
                                    paramUpdate.Add("TotalGetType", promoHeader.TotalGetType);
                                    paramUpdate.Add("TotalGetValue", promoHeader.TotalGetValue);
                                    paramUpdate.Add("MaxTotalGetValue", promoHeader.MaxTotalGetValue);
                                    paramUpdate.Add("IsCombine", promoHeader.IsCombine);
                                    paramUpdate.Add("IsVoucher", promoHeader.IsVoucher);
                                    paramUpdate.Add("Status", promotion.Status);
                                    paramUpdate.Add("ModifiedBy", promotion.ModifiedBy);
                                    //paramUpdate.Add("CustomField1", promotion.CustomField1);
                                    //paramUpdate.Add("CustomField2", promotion.CustomField2);
                                    //paramUpdate.Add("CustomField3", promotion.CustomField3);

                                    db.Execute("USP_U_S_PromoHeader", param: paramUpdate, commandType: CommandType.StoredProcedure);

                                    promotionId = promoHeader.PromoId;
                                    result = true;
                                    msg += "Promotion is already in use so you can just update the status to Inactive.";
                                    return result;
                                }
                                else
                                {
                                    promotionId = promoHeader.PromoId;
                                    result = false;
                                    msg += "Promotion is already in use so you can just update the status to Inactive.";
                                    return result;
                                }
                            }
                        }
                    }

                    var checkName = db.Query<SPromoHeader>($"SELECT PromoId FROM S_PromoHeader WHERE CompanyCode = '{promotion.CompanyCode}' AND PromoId <> '{promotion.PromoId}' AND PromoName = N'{promotion.PromoName}'", commandType: CommandType.Text);
                    if (checkName.Any() && checkName.Count() > 0)
                    {
                        promotionId = promotion.PromoId;
                        result = false;
                        msg += "Name of promotion has existed.";

                        return result;
                    }

                    drHeader["PromoId"] = promotion.PromoId;
                    drHeader["CompanyCode"] = promotion.CompanyCode;
                    drHeader["PromoType"] = promotion.PromoType;
                    drHeader["PromoName"] = promotion.PromoName;
                    drHeader["CustomerType"] = promotion.CustomerType;
                    drHeader["ValidDateFrom"] = promotion.ValidDateFrom == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : promotion.ValidDateFrom.Value.ToString(StringFormatConst.SQLDateParam);
                    drHeader["ValidDateTo"] = promotion.ValidDateTo == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : promotion.ValidDateTo.Value.ToString(StringFormatConst.SQLDateParam);
                    drHeader["ValidTimeFrom"] = promotion.ValidTimeFrom ?? 0;
                    drHeader["ValidTimeTo"] = promotion.ValidTimeTo ?? 2359;
                    drHeader["IsMon"] = promotion.IsMon;
                    drHeader["IsTue"] = promotion.IsTue;
                    drHeader["IsWed"] = promotion.IsWed;
                    drHeader["IsThu"] = promotion.IsThu;
                    drHeader["IsFri"] = promotion.IsFri;
                    drHeader["IsSat"] = promotion.IsSat;
                    drHeader["IsSun"] = promotion.IsSun;
                    drHeader["TotalBuyFrom"] = promotion.TotalBuyFrom;
                    drHeader["TotalBuyTo"] = promotion.TotalBuyTo;
                    drHeader["TotalGetType"] = promotion.TotalGetType;
                    drHeader["TotalGetValue"] = promotion.TotalGetValue;
                    drHeader["MaxTotalGetValue"] = promotion.MaxTotalGetValue;
                    drHeader["IsCombine"] = promotion.IsCombine;
                    drHeader["IsVoucher"] = promotion.IsVoucher;
                    drHeader["CreatedBy"] = promotion.CreatedBy;
                    drHeader["CreatedOn"] = promotion.CreatedOn == null ? DateTime.Now.ToString(StringFormatConst.SQLDateTimeParam) : promotion.CreatedOn.Value.ToString(StringFormatConst.SQLDateTimeParam);
                    drHeader["ModifiedBy"] = promotion.ModifiedBy;
                    drHeader["ModifiedOn"] = promotion.ModifiedOn == null ? DateTime.Now.ToString(StringFormatConst.SQLDateTimeParam) : promotion.ModifiedOn.Value.ToString(StringFormatConst.SQLDateTimeParam);
                    drHeader["Status"] = promotion.Status;
                    drHeader["SAPPromoId"] = promotion.SAPPromoId;
                    drHeader["SAPBonusBuyId"] = promotion.SAPBonusBuyId;
                    drHeader["SchemaId"] = string.Empty;
                    drHeader["MaxQtyByReceipt"] = promotion.MaxQtyByReceipt;
                    drHeader["MaxQtyByStore"] = promotion.MaxQtyByStore;
                    //drHeader["CustomField1"] = promotion.CustomField1;
                    //drHeader["CustomField2"] = promotion.CustomField2;
                    //drHeader["CustomField3"] = promotion.CustomField3;

                    dtHeader.Rows.Add(drHeader);

                    DataTable dtPromoBuy = CreateTablePromoBuy();
                    DataTable dtPromoBuyOTGroup = CreateTablePromoOTGroup("PROMO_BUY_OTG");
                    DataTable dtPromoGetOTGroup = CreateTablePromoOTGroup("PROMO_GET_OTG");

                    int lineNum = 1;
                    //  insert promo buy
                    foreach (SPromoBuy promoBuy in promotion.PromoBuys)
                    {
                        if (promoBuy.LineNum == 0)
                        {
                            promoBuy.LineNum = lineNum++;
                        }

                        DataRow drPromoBuy = dtPromoBuy.NewRow();
                        drPromoBuy["PromoId"] = promotion.PromoId;
                        drPromoBuy["CompanyCode"] = promotion.CompanyCode;
                        drPromoBuy["LineNum"] = promoBuy.LineNum;
                        drPromoBuy["LineType"] = promoBuy.LineType;
                        drPromoBuy["LineCode"] = promoBuy.LineCode;
                        drPromoBuy["LineName"] = promoBuy.LineName;
                        drPromoBuy["LineUom"] = promoBuy.LineUom;
                        drPromoBuy["ValueType"] = promoBuy.ValueType;
                        drPromoBuy["Condition_1"] = promoBuy.Condition1;
                        drPromoBuy["Value_1"] = promoBuy.Value1;
                        drPromoBuy["Condition_2"] = promoBuy.Condition2;
                        drPromoBuy["Value_2"] = promoBuy.Value2;
                        ////drPromoBuy["InActive"] = promoBuy.InActive;
                        ////drPromoBuy["ModifiedDate"] = promoBuy.ModifiedDate;

                        dtPromoBuy.Rows.Add(drPromoBuy);

                        if (promoBuy.Lines != null && promoBuy.Lines.Count > 0)
                        {
                            foreach (SPromoOTGroup otGroup in promoBuy.Lines)
                            {
                                DataRow drOTGroup = dtPromoBuyOTGroup.NewRow();
                                drOTGroup["PromoId"] = promotion.PromoId;
                                drOTGroup["CompanyCode"] = promotion.CompanyCode;
                                drOTGroup["GroupID"] = otGroup.GroupID;
                                drOTGroup["LineNum"] = otGroup.LineNum;
                                drOTGroup["LineType"] = otGroup.LineType;
                                drOTGroup["LineCode"] = otGroup.LineCode;
                                drOTGroup["LineName"] = otGroup.LineName;
                                drOTGroup["LineUoM"] = otGroup.LineUom;

                                dtPromoBuyOTGroup.Rows.Add(drOTGroup);
                            }
                        }
                    }

                    DataTable dtPromoGet = CreateTablePromoGet();

                    lineNum = 1;
                    //  insert promo get
                    foreach (SPromoGet promoGet in promotion.PromoGets)
                    {
                        if (promoGet.LineNum == 0)
                        {
                            promoGet.LineNum = lineNum++;
                        }
                        DataRow drPromoGet = dtPromoGet.NewRow();
                        drPromoGet["PromoId"] = promotion.PromoId;
                        drPromoGet["CompanyCode"] = promotion.CompanyCode;
                        drPromoGet["LineNum"] = promoGet.LineNum;
                        drPromoGet["LineType"] = promoGet.LineType;
                        drPromoGet["LineCode"] = promoGet.LineCode;
                        drPromoGet["LineName"] = promoGet.LineName;
                        drPromoGet["LineUom"] = promoGet.LineUom;
                        drPromoGet["ConditionType"] = promoGet.ConditionType;
                        drPromoGet["Condition_1"] = promoGet.Condition1;
                        drPromoGet["Value_1"] = promoGet.Value1;
                        drPromoGet["Condition_2"] = promoGet.Condition2;
                        drPromoGet["Value_2"] = promoGet.Value2;
                        drPromoGet["ValueType"] = promoGet.ValueType;
                        drPromoGet["GetValue"] = promoGet.GetValue;
                        drPromoGet["MaxAmtDis"] = promoGet.MaxAmtDis;
                        drPromoGet["MaxQtyDis"] = promoGet.MaxQtyDis;
                        ////drPromoGet["InActive"] = promoGet.InActive;
                        ////drPromoGet["ModifiedDate"] = promoGet.ModifiedDate;

                        dtPromoGet.Rows.Add(drPromoGet);

                        if (promoGet.Lines != null && promoGet.Lines.Count > 0)
                        {
                            foreach (SPromoOTGroup otGroup in promoGet.Lines)
                            {
                                DataRow drOTGroup = dtPromoGetOTGroup.NewRow();
                                drOTGroup["PromoId"] = promotion.PromoId;
                                drOTGroup["CompanyCode"] = promotion.CompanyCode;
                                drOTGroup["GroupID"] = otGroup.GroupID;
                                drOTGroup["LineNum"] = otGroup.LineNum;
                                drOTGroup["LineType"] = otGroup.LineType;
                                drOTGroup["LineCode"] = otGroup.LineCode;
                                drOTGroup["LineName"] = otGroup.LineName;
                                drOTGroup["LineUoM"] = otGroup.LineUom;

                                dtPromoGetOTGroup.Rows.Add(drOTGroup);
                            }
                        }
                    }

                    DataTable dtPromoCustomer = CreateTablePromoCustomer();
                    //  insert customer
                    foreach (SPromoCustomer customer in promotion.PromoCustomers)
                    {
                        DataRow drCustomer = dtPromoCustomer.NewRow();
                        drCustomer["PromoId"] = promotion.PromoId;
                        drCustomer["CompanyCode"] = promotion.CompanyCode;
                        drCustomer["LineNum"] = customer.LineNum;
                        drCustomer["CustomerValue"] = customer.CustomerValue;
                        drCustomer["CustomerType"] = customer.CustomerType;

                        dtPromoCustomer.Rows.Add(drCustomer);
                    }

                    DataTable dtPromoStore = CreateTablePromoStore();
                    //  insert store
                    foreach (SPromoStore store in promotion.PromoStores)
                    {
                        DataRow drStore = dtPromoStore.NewRow();
                        drStore["PromoId"] = promotion.PromoId;
                        drStore["CompanyCode"] = promotion.CompanyCode;
                        drStore["LineNum"] = store.LineNum;
                        drStore["StoreValue"] = store.StoreValue;

                        dtPromoStore.Rows.Add(drStore);
                    }

                    string userId = string.IsNullOrEmpty(promotion.ModifiedBy) ? promotion.CreatedBy : promotion.ModifiedBy;
                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", userId);
                    parameters.Add("S_PromoHeader", dtHeader.AsTableValuedParameter());
                    parameters.Add("S_PromoBuy", dtPromoBuy.AsTableValuedParameter());
                    parameters.Add("S_PromoGet", dtPromoGet.AsTableValuedParameter());
                    parameters.Add("S_PromoStore", dtPromoStore.AsTableValuedParameter());
                    parameters.Add("S_PromoCustomer", dtPromoCustomer.AsTableValuedParameter());
                    parameters.Add("S_PromoBuyOTGroup", dtPromoBuyOTGroup.AsTableValuedParameter());
                    parameters.Add("S_PromoGetOTGroup", dtPromoGetOTGroup.AsTableValuedParameter());

                    var res = db.Query("SYNC_IU_S_Promotion", param: parameters, commandType: CommandType.StoredProcedure);
                    if (res.Any() && res.Count() > 0)
                    {
                        DataTable dataTable = res.ToDataTable();
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            string errorCode = dataTable.Rows[0]["ErrCode"].ToString();
                            if (errorCode == "0")
                            {
                                promotionId = promotion.PromoId;
                                result = true;
                                msg += dataTable.Rows[0]["ErrMsg"].ToString();
                            }
                            else
                            {
                                promotionId = promotion.PromoId;
                                result = false;
                                msg += $"{errorCode} - {dataTable.Rows[0]["ErrMsg"]}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    promotionId = promotion.PromoId;
                    result = false;
                    msg += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            ////  
            //if (result && !string.IsNullOrEmpty(promotion.PromoId))
            //{
            //    if (dicPromotions.ContainsKey(promotion.PromoId))
            //    {
            //        dicPromotions[promotion.PromoId] = promotion;
            //    }
            //    else
            //    {
            //        dicPromotions.Add(promotion.PromoId, promotion);
            //    }

            //    cacheService.CacheData(promotion, string.Format(PrefixCachePromo, promotion.PromoId), timeCachePromo);
            //}

            //  edited: 2022-10-14
            if (result)
            {
                SPromoHeader promoHeader = new SPromoHeader()
                {
                    PromoId = promotion.PromoId,
                    CompanyCode = promotion.CompanyCode
                };

                PromotionViewModel promotionView = GetPromotionViewModel(promoHeader);
                if (dicPromotions.ContainsKey(promotionView.PromoId))
                {
                    dicPromotions[promotionView.PromoId] = promotionView;
                }
                else
                {
                    dicPromotions.Add(promotionView.PromoId, promotionView);
                }
            }

            return result;
        }

        public List<SchemaHeaderViewModel> SearchSchema(string companyCode, string schemaId, string schemaName, string promoId, string status)
        {
            List<SchemaHeaderViewModel> schemas = new List<SchemaHeaderViewModel>();
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("SchemaId", schemaId);
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("SchemaName", schemaName);
                    parameters.Add("PromoId", promoId);
                    parameters.Add("Status", status);

                    var r = db.Query<SchemaHeaderViewModel>("USP_S_S_SearchSchema", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        schemas = r.ToList();
                    }
                }
                catch { }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return schemas;
        }

        public SchemaViewModel GetSchema(string companyCode, string schemaId, out string msg)
        {
            SchemaViewModel schemaView = null;
            msg = "";
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("SchemaId", schemaId);
                    var r = db.Query<SPromoSchema>("USP_S_S_PromoSchema", parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        schemaView = _mapper.Map<SchemaViewModel>(r.FirstOrDefault());

                        var line = db.Query<SSchemaLine>("USP_S_S_SchemaLine", parameters, commandType: CommandType.StoredProcedure);
                        if (line.Any())
                        {
                            schemaView.SchemaLines = line.ToList();
                        }
                        else
                        {
                            schemaView.SchemaLines = new List<SSchemaLine>();
                        }
                    }
                    else
                    {
                        msg = "Cannot find schema id: " + schemaId;
                    }
                }
                catch (Exception ex)
                {
                    msg += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return schemaView;
        }

        public SchemaViewModel GetSchemaPromo(SPromoSchema schema, out string msg)
        {
            SchemaViewModel schemaView = null;
            msg = "";
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", schema.CompanyCode);
                    parameters.Add("SchemaId", schema.SchemaId);

                    schemaView = _mapper.Map<SchemaViewModel>(schema);
                    var line = db.Query<SPromoHeader>("USP_S_S_SchemaLinePromo", parameters, commandType: CommandType.StoredProcedure);

                    if (line.Any())
                    {
                        schemaView.PromotionLines = line.ToList();
                    }
                    else
                    {
                        schemaView.PromotionLines = new List<SPromoHeader>();
                    }
                }
                catch (Exception ex)
                {
                    msg += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            if (schemaView != null)
            {
                cacheService.CacheData(schemaView, string.Format(PrefixCacheSchema, schemaView.SchemaId), timeCachePromo);
            }

            return schemaView;
        }

        private PromotionViewModel GetPromotionViewModel(SPromoHeader promoHeader)
        {
            PromotionViewModel promotion = null;
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    if (promoHeader != null && !string.IsNullOrEmpty(promoHeader.PromoId))
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyCode", promoHeader.CompanyCode);
                        parameters.Add("PromoId", promoHeader.PromoId);

                        promotion = _mapper.Map<PromotionViewModel>(promoHeader);

                        var cus = db.Query<SPromoCustomer>("USP_S_S_PromoCustomer", parameters, commandType: CommandType.StoredProcedure);
                        if (cus.Any())
                        {
                            promotion.PromoCustomers = cus.ToList();
                        }
                        else
                        {
                            promotion.PromoCustomers = new List<SPromoCustomer>();
                        }
                        var store = db.Query<SPromoStore>("USP_S_S_PromoStore", parameters, commandType: CommandType.StoredProcedure);
                        if (store.Any())
                        {
                            promotion.PromoStores = store.ToList();
                        }
                        else
                        {
                            promotion.PromoStores = new List<SPromoStore>();
                        }
                        var buy = db.Query<SPromoBuy>("USP_S_S_PromoBuy", parameters, commandType: CommandType.StoredProcedure);
                        if (buy.Any())
                        {
                            promotion.PromoBuys = buy.ToList();
                        }
                        else
                        {
                            promotion.PromoBuys = new List<SPromoBuy>();
                        }

                        var get = db.Query<SPromoGet>("USP_S_S_PromoGet", parameters, commandType: CommandType.StoredProcedure);
                        if (get.Any())
                        {
                            promotion.PromoGets = get.ToList();
                        }
                        else
                        {
                            promotion.PromoGets = new List<SPromoGet>();
                        }

                        var buyOTGroup = db.Query<SPromoOTGroup>("USP_S_S_PromoBuyOTGroup", parameters, commandType: CommandType.StoredProcedure);
                        if (buyOTGroup.Any())
                        {
                            List<SPromoOTGroup> buyOTGroups = buyOTGroup.ToList();
                            foreach (SPromoBuy promoBuy in promotion.PromoBuys)
                            {
                                if (promoBuy.LineType == PromoLineType.OneTimeGroup)
                                {
                                    var otGroup = buyOTGroup.Where(s => s.GroupID == promoBuy.LineCode);
                                    if (otGroup.Any() && otGroup.Count() > 0)
                                    {
                                        promoBuy.Lines = otGroup.ToList();
                                    }
                                }
                            }
                        }

                        var getOTGroup = db.Query<SPromoOTGroup>("USP_S_S_PromoGetOTGroup", parameters, commandType: CommandType.StoredProcedure);
                        if (getOTGroup.Any())
                        {
                            List<SPromoOTGroup> buyOTGroups = getOTGroup.ToList();
                            foreach (SPromoGet promoGet in promotion.PromoGets)
                            {
                                if (promoGet.LineType == PromoLineType.OneTimeGroup)
                                {
                                    var otGroup = getOTGroup.Where(s => s.GroupID == promoGet.LineCode);
                                    if (otGroup.Any() && otGroup.Count() > 0)
                                    {
                                        promoGet.Lines = otGroup.ToList();
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            if (promotion != null)
            {
                cacheService.CacheData(promotion, string.Format(PrefixCachePromo, promotion.PromoId), timeCachePromo);
            }

            return promotion;
        }

        public bool InsertUpdateSchema(SchemaViewModel schema, out string msg)
        {
            bool result = false;
            msg = "";
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    using (var trans = db.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            if (string.IsNullOrEmpty(schema.SchemaId))
                            {

                                string key = sPromotion.GetScalar($"select dbo.[fnc_AutoGenDocumentCode] ('PS','{schema.CompanyCode}', '')", null, commandType: CommandType.Text).ToString();
                                schema.SchemaId = key;
                            }
                            parameters.Add("SchemaId", schema.SchemaId);
                            parameters.Add("CompanyCode", schema.CompanyCode);
                            parameters.Add("SchemaName", schema.SchemaName);
                            parameters.Add("Status", string.IsNullOrEmpty(schema.Status) ? "N" : schema.Status);
                            parameters.Add("AllowChain", string.IsNullOrEmpty(schema.AllowChain) ? "N" : schema.AllowChain);

                            var paramCheck = new DynamicParameters();
                            paramCheck.Add("SchemaId", schema.SchemaId);
                            paramCheck.Add("CompanyCode", schema.CompanyCode);
                            var r = db.Query<SPromoSchema>("USP_S_S_PromoSchema", paramCheck, transaction: trans, commandType: CommandType.StoredProcedure);
                            if (r.Any() && r.Count() > 0)
                            {
                                //  update schema header
                                parameters.Add("ModifiedBy", schema.ModifiedBy);
                                db.Execute("USP_U_S_PromoSchema", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);
                            }
                            else
                            {
                                //  insert schema header
                                parameters.Add("CreatedBy", schema.CreatedBy);
                                db.Execute("USP_I_S_PromoSchema", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);
                            }

                            //  delete content for promo
                            db.Execute("USP_D_S_SchemaContent", paramCheck, transaction: trans, commandType: CommandType.StoredProcedure);
                            int lineNum = 0;
                            //  insert schema line
                            foreach (SSchemaLine schemaLine in schema.SchemaLines)
                            {
                                lineNum++;
                                schemaLine.LineNum = lineNum;
                                parameters = new DynamicParameters();
                                parameters.Add("SchemaId", schema.SchemaId);
                                parameters.Add("CompanyCode", schema.CompanyCode);
                                parameters.Add("LineNum", schemaLine.LineNum);
                                parameters.Add("PromoId", schemaLine.PromoId);
                                parameters.Add("Description", schemaLine.Description);
                                parameters.Add("Priority", schemaLine.Priority);
                                parameters.Add("IsApply", schemaLine.IsApply);

                                db.Execute("USP_I_S_SchemaLine", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);

                            }
                            db.Execute($"USP_UpdatePromoNotCombine '{schema.CompanyCode}'", null, transaction: trans, commandType: CommandType.Text);

                            trans.Commit();
                            result = true;

                            var parametersSelect = new DynamicParameters();
                            parametersSelect.Add("CompanyCode", schema.CompanyCode);
                            parametersSelect.Add("SchemaId", schema.SchemaId);

                            var linePromo = db.Query<SPromoHeader>("USP_S_S_SchemaLinePromo", parametersSelect, commandType: CommandType.StoredProcedure);

                            if (linePromo.Any())
                            {
                                schema.PromotionLines = linePromo.ToList();
                            }
                            else
                            {
                                schema.PromotionLines = new List<SPromoHeader>();
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            trans.Rollback();
                            msg += "Transaction Exception: " + ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    msg += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            if (result && schema != null && !string.IsNullOrEmpty(schema.SchemaId))
            {
                if (dicPromoSchemas.ContainsKey(schema.SchemaId))
                {
                    dicPromoSchemas[schema.SchemaId] = schema;
                }
                else
                {
                    dicPromoSchemas.Add(schema.SchemaId, schema);
                }

                cacheService.CacheData(schema, string.Format(PrefixCacheSchema, schema.SchemaId), timeCachePromo);
            }

            return result;
        }

        #region Promotion data process

        //private string BuilDayOfWeekPara(DayOfWeek day)
        //{
        //    List<string> lstDay = new List<string>
        //    {
        //        day == DayOfWeek.Monday ? "Y" : "''",
        //        day == DayOfWeek.Tuesday ? "Y" : "''",
        //        day == DayOfWeek.Wednesday ? "Y" : "''",
        //        day == DayOfWeek.Thursday ? "Y" : "''",
        //        day == DayOfWeek.Friday ? "Y" : "''",
        //        day == DayOfWeek.Saturday ? "Y" : "''",
        //        day == DayOfWeek.Sunday ? "Y" : "''"
        //    };
        //    return string.Join(",", lstDay);
        //}

        private bool CheckApplyPromoHeader(Document doc, PromotionViewModel promotion)
        {
            if (promotion != null)
            {
                if (promotion.CustomerType == PromoCustomerType.TypeCodeValue)
                {
                    var rcheck = promotion.PromoCustomers.Where(c => c.CustomerValue == doc.CardCode);
                    if (!rcheck.Any())
                    {
                        return false;
                    }
                }
                else if (promotion.CustomerType == PromoCustomerType.TypeGroupValue)
                {
                    var rcheck = promotion.PromoCustomers.Where(c => c.CustomerValue == doc.CardGroup);
                    if (!rcheck.Any())
                    {
                        return false;
                    }
                }
                else if (promotion.CustomerType == PromoCustomerType.TypeRankValue)
                {
                    var rcheck = promotion.PromoCustomers.Where(c => c.CustomerValue == doc.CustomerRank);
                    if (!rcheck.Any())
                    {
                        return false;
                    }
                }

                if (promotion.PromoStores.Any())
                {
                    var rcheck = promotion.PromoStores.Where(s => s.StoreValue == doc.StoreId);
                    if (!rcheck.Any())
                    {
                        return false;
                    }
                }

                if (promotion.ValidDateFrom != null && (promotion.ValidDateFrom.Value.Date - doc.DocDate.Value.Date).TotalDays > 0)
                {
                    return false;
                }

                if (promotion.ValidDateTo != null && (promotion.ValidDateTo.Value.Date - doc.DocDate.Value.Date).TotalDays < 0)
                {
                    return false;
                }

                TimeSpan tpNow = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                if (promotion.ValidTimeFrom != null && promotion.ValidTimeFrom.Value - tpNow.ToInt() > 0)
                {
                    return false;
                }

                if (promotion.ValidTimeTo != null && promotion.ValidTimeTo.Value - tpNow.ToInt() < 0)
                {
                    return false;
                }

                switch (doc.DocDate.Value.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (promotion.IsMon != "Y")
                        {
                            return false;
                        }
                        break;

                    case DayOfWeek.Tuesday:
                        if (promotion.IsTue != "Y")
                        {
                            return false;
                        }
                        break;

                    case DayOfWeek.Wednesday:
                        if (promotion.IsWed != "Y")
                        {
                            return false;
                        }
                        break;

                    case DayOfWeek.Thursday:
                        if (promotion.IsThu != "Y")
                        {
                            return false;
                        }
                        break;

                    case DayOfWeek.Friday:
                        if (promotion.IsFri != "Y")
                        {
                            return false;
                        }
                        break;

                    case DayOfWeek.Saturday:
                        if (promotion.IsSat != "Y")
                        {
                            return false;
                        }
                        break;

                    case DayOfWeek.Sunday:
                        if (promotion.IsSun != "Y")
                        {
                            return false;
                        }
                        break;
                }

                return true;
            }
            return false;
        }

        private ItemViewModel GetItemInfo(string companyCode, string storeId, string itemCode, string UoMCode, string barCode, string cardGroup)
        {
            ItemViewModel item = null;
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("ItemCode", itemCode);
                    parameters.Add("UoMCode", UoMCode);
                    parameters.Add("BarCode", barCode);
                    parameters.Add("Keyword", "");
                    parameters.Add("Merchandise", "");
                    parameters.Add("Type", "");
                    parameters.Add("PriceListId", "");
                    parameters.Add("CustomerGroupId", cardGroup);

                    //var dblist = db.Query<ItemViewModel>($"USP_GetItem", param: parameters, commandType: CommandType.StoredProcedure);
                    var dblist = db.Query<ItemViewModel>($"USP_GetItem_Promo", param: parameters, commandType: CommandType.StoredProcedure);

                    if (dblist.Any())
                    {
                        item = dblist.FirstOrDefault();
                    }
                }
                catch { }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            return item;
        }

        #endregion

        #region CheckVoucherPromotion

        public GenericResult CheckVoucherPromotion(Document srcDoc)
        {
            GenericResult result = new GenericResult();
            if (!string.IsNullOrEmpty(srcDoc.PromotionCode))
            {
                this.GetPromotion(srcDoc.UCompanyCode, srcDoc.PromotionCode, out string msg);
                if (!string.IsNullOrEmpty(msg))
                {
                    result.Success = false;
                    result.Message = msg;
                }
                else
                {
                    result.Success = true;
                }
            }
            return result;
        }

        #endregion

        #region Apply Promotion

        public Document ApplyPromotion(Document srcDoc)
        {
            Set_ReCalcGrid(ref srcDoc);
            Set_SumTotalAmt(ref srcDoc);

            //  nếu AllowChain = Y thì check Line đó nếu có promotion rồi sẽ k apply nữa
            double docTotal = srcDoc.DocumentLines.Sum(l => l.LineTotal.Value);
            DateTime docDate = DateTime.Parse(srcDoc.DocDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));
            //  apply promo schema
            List<SPromoSchema> schemas = this.CheckSchemaHeader(srcDoc.UCompanyCode, srcDoc.StoreId, srcDoc.CardCode, srcDoc.CardGroup, docTotal, docDate);
            if (schemas != null && schemas.Count > 0)
            {
                foreach (SPromoSchema schm in schemas)
                {
                    if (schm != null)
                    {
                        //  check đã có promotion chưa
                        if (!string.IsNullOrEmpty(srcDoc.PromotionId))
                        {
                            break;
                        }

                        bool isSchema = false;
                        //  check đã có schema chưa
                        foreach (DocumentLine line in srcDoc.DocumentLines)
                        {
                            if (!string.IsNullOrEmpty(line.UPromoCode) || !string.IsNullOrEmpty(line.USchemaCode))
                            {
                                isSchema = true;
                                break;
                            }
                        }

                        if (isSchema)
                        {
                            break;
                        }

                        SchemaViewModel schema = null;// _mapper.Map<SchemaViewModel>(schm);
                        if (schm.SchemaName != "SchemaDefault")
                        {
                            if (!dicPromoSchemas.ContainsKey(schm.SchemaId))
                            {
                                //schema = this.GetSchema(srcDoc.UCompanyCode, schm.SchemaId, out string msg);
                                schema = cacheService.GetCachedData<SchemaViewModel>(string.Format(PrefixCacheSchema, schm.SchemaId));
                                if (schema == null)
                                {
                                    schema = this.GetSchemaPromo(schm, out _);
                                }

                                if (schema != null)
                                {
                                    dicPromoSchemas.Add(schm.SchemaId, schema);
                                }
                            }
                            else
                            {
                                schema = dicPromoSchemas[schm.SchemaId];
                            }
                        }
                        else
                        {
                            schema = _mapper.Map<SchemaViewModel>(schm);
                            if (schema != null)
                            {
                                schema.PromotionLines = this.CheckPromotionsBySchema(srcDoc.UCompanyCode, srcDoc.StoreId, 0, srcDoc.CardCode, srcDoc.CardGroup, docTotal, docDate, schm.SchemaId);
                            }

                            if (dicPromoSchemas.ContainsKey(schema.SchemaId))
                            {
                                dicPromoSchemas[schema.SchemaId] = schema;
                            }
                            else
                            {
                                dicPromoSchemas.Add(schema.SchemaId, schema);
                            }
                        }

                        if (schema != null)
                        {
                            var resDoc = ApplyPromoSchema(srcDoc, schema);
                            if (resDoc != null)
                            {
                                srcDoc = resDoc;
                            }
                        }
                    }
                }
            }

            //  apply promo one new
            bool isApply = false;
            if (schemas.Count > 0)
            {
                if (!string.IsNullOrEmpty(srcDoc.PromotionId))
                {
                    isApply = true;
                }
                else
                {
                    foreach (DocumentLine docLine in srcDoc.DocumentLines)
                    {
                        if (!string.IsNullOrEmpty(docLine.UPromoCode) || !string.IsNullOrEmpty(docLine.USchemaCode))
                        {
                            isApply = true;
                            break;
                        }
                    }
                }
            }

            if (!isApply)
            {
                List<SPromoHeader> promoHeaders = this.CheckPromotions(srcDoc.UCompanyCode, srcDoc.StoreId, 0, srcDoc.CardCode, srcDoc.CardGroup, docTotal, docDate);
                if (promoHeaders != null && promoHeaders.Count > 0)
                {
                    int index = 0;
                    foreach (SPromoHeader prHeader in promoHeaders)
                    {
                        if (index > 0)
                        {
                            if (!string.IsNullOrEmpty(srcDoc.PromotionId))
                            {
                                isApply = true;
                            }
                            else
                            {
                                foreach (DocumentLine docLine in srcDoc.DocumentLines)
                                {
                                    if (!string.IsNullOrEmpty(docLine.UPromoCode) || !string.IsNullOrEmpty(docLine.USchemaCode))
                                    {
                                        isApply = true;
                                        break;
                                    }
                                }
                            }
                        }

                        index++;
                        if (isApply)
                        {
                            break;
                        }

                        if (!string.IsNullOrEmpty(srcDoc.PromotionCode))
                        {
                            if (srcDoc.PromotionCode == prHeader.PromoId && prHeader.IsVoucher == true)
                            {
                                PromotionViewModel promo = null;
                                string promoId = prHeader.PromoId;
                                if (!dicPromotions.ContainsKey(promoId))
                                {
                                    //promo = this.GetPromotion(srcDoc.UCompanyCode, promoId, out string _);
                                    promo = cacheService.GetCachedData<PromotionViewModel>(string.Format(PrefixCachePromo, prHeader.PromoId));
                                    if (promo == null)
                                    {
                                        promo = this.GetPromotionViewModel(prHeader);
                                    }
                                    if (promo != null)
                                    {
                                        dicPromotions.Add(promoId, promo);
                                    }
                                }
                                else
                                {
                                    promo = dicPromotions[promoId];
                                }

                                var res = ApplyPromo(srcDoc, string.Empty, promo);
                                if (res != null)
                                {
                                    srcDoc = res;
                                    srcDoc.VoucherIsApply = true;
                                }
                            }
                        }
                        else
                        {
                            if (prHeader.IsVoucher != true)
                            {
                                PromotionViewModel promo = null;
                                //string promoId = prHeader.PromoId;
                                if (!dicPromotions.ContainsKey(prHeader.PromoId))
                                {
                                    //promo = this.GetPromotion(srcDoc.UCompanyCode, promoId, out string _);
                                    promo = cacheService.GetCachedData<PromotionViewModel>(string.Format(PrefixCachePromo, prHeader.PromoId));
                                    if (promo == null)
                                    {
                                        promo = this.GetPromotionViewModel(prHeader);
                                    }

                                    if (promo != null)
                                    {
                                        dicPromotions.Add(prHeader.PromoId, promo);
                                    }
                                }
                                else
                                {
                                    promo = dicPromotions[prHeader.PromoId];
                                }

                                var res = ApplyPromo(srcDoc, string.Empty, promo);
                                if (res != null)
                                {
                                    srcDoc = res;
                                }
                            }
                        }
                    }
                }
            }

            srcDoc.PromotionApply = new List<PromotionViewModel>();
            foreach (var line in srcDoc.DocumentLines)
            {
                if (!string.IsNullOrEmpty(line.UPromoCode))
                {
                    string[] spl = line.UPromoCode.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string Xstr in spl)
                    {
                        if (srcDoc.PromotionApply.Where(x => x.PromoId == Xstr).SingleOrDefault() == null)
                        {
                            if (dicPromotions.ContainsKey(Xstr))
                            {
                                srcDoc.PromotionApply.Add(dicPromotions[Xstr]);
                                srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoBuys.Clear();
                                srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoGets.Clear();
                                srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoCustomers.Clear();
                                srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoStores.Clear();
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(srcDoc.PromotionId))
            {
                //if (srcDoc.PromotionApply.Where(x => x.PromoId == srcDoc.PromotionId).SingleOrDefault() == null)
                //{
                //    if (dicPromotions.ContainsKey(srcDoc.PromotionId))
                //    {
                //        srcDoc.PromotionApply.Add(dicPromotions[srcDoc.PromotionId]);
                //    }
                //}
                string[] spl = srcDoc.PromotionId.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string Xstr in spl)
                {
                    if (srcDoc.PromotionApply.Where(x => x.PromoId == Xstr).SingleOrDefault() == null)
                    {
                        if (dicPromotions.ContainsKey(Xstr))
                        {
                            srcDoc.PromotionApply.Add(dicPromotions[Xstr]);
                            srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoBuys.Clear();
                            srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoGets.Clear();
                            srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoCustomers.Clear();
                            srcDoc.PromotionApply[srcDoc.PromotionApply.Count - 1].PromoStores.Clear();
                        }
                    }
                }
            }

            return srcDoc;
        }

        private Document ApplyPromoSchema(Document doc, SchemaViewModel schema)
        {
            foreach (SPromoHeader promoHeader in schema.PromotionLines)
            {
                if (promoHeader.Status != "Y" || promoHeader.IsApply != "Y")
                {
                    continue;
                }

                PromotionViewModel promo = null;
                if (!dicPromotions.ContainsKey(promoHeader.PromoId))
                {
                    promo = cacheService.GetCachedData<PromotionViewModel>(string.Format(PrefixCachePromo, promoHeader.PromoId));
                    if (promo == null)
                    {
                        promo = this.GetPromotionViewModel(promoHeader);
                    }

                    //  bỏ qua promotion nếu đó là voucher
                    if (promo.IsVoucher ?? false)
                    {
                        continue;
                    }

                    if (promo != null)
                    {
                        dicPromotions.Add(promoHeader.PromoId, promo);
                    }
                }
                else
                {
                    promo = dicPromotions[promoHeader.PromoId];
                }

                if (promo != null && promo.Status == "Y" && CheckApplyPromoHeader(doc, promo))
                {
                    doc = ApplyPromo(doc, schema.SchemaId, promo);
                }
            }

            return doc;
        }

        private Document ApplyPromo(Document doc, string schemaId, PromotionViewModel promotion)
        {
            int type = promotion.PromoType.Value;
            Document result = doc;

            switch (type)
            {
                case PromoType.SingeDiscountCode:
                case PromoType.MultiBuyCode:
                    result = Apply_Single_Promotion(doc, schemaId, promotion, doc.PromotionCode);
                    break;

                case PromoType.BuyXGetYCode:
                    result = Apply_BuyXGetY_Promotion(doc, schemaId, promotion, doc.PromotionCode);
                    break;

                case PromoType.ComboCode:
                    result = Apply_Combo_Promotion(doc, schemaId, promotion, doc.PromotionCode);
                    break;

                case PromoType.TotalBillCode:
                    result = Apply_TotalBill_Promotion(doc, schemaId, promotion, doc.PromotionCode);
                    break;

                case PromoType.MixMatchCode:
                    result = Apply_MixMatch_Promotion(doc, schemaId, promotion, doc.PromotionCode);
                    break;

                case PromoType.PrepaidCardCode:
                    result = Apply_PrepaidCard_Promotion(doc, schemaId, promotion, doc.PromotionCode);
                    break;
            }
            return result;
        }

        private Document Apply_Single_Promotion(Document doc, string schemaId, PromotionViewModel promotion, string voucherNum)
        {
            string promoId = promotion.PromoId;
            string schemaName = "";
            string allowChain = "";
            SchemaViewModel schema = null;
            if (dicPromoSchemas.ContainsKey(schemaId))
            {
                schema = dicPromoSchemas[schemaId];
                schemaName = schema != null ? schema.SchemaName : "";
                allowChain = schema.AllowChain;
            }
            //else if (schemaId == SchemaDefault.SchemaId)
            //{
            //    schema = SchemaDefault;
            //    schemaName = schema != null ? schema.SchemaName : "";
            //    allowChain = schema.AllowChain;
            //}

            List<DocumentLine> newLinePromo = new List<DocumentLine>();

            if (promotion.PromoGets != null && promotion.PromoGets.Count > 0)
            {
                List<SPromoGet> promoGets = promotion.PromoGets.Where(x => x.InActive != "Y").OrderByDescending(x => (x.Value1 ?? 0)).ToList();
                //  apply promotion
                foreach (SPromoGet promoGetLine in promoGets)
                {
                    double otQuantity = 0;
                    double otAmount = 0;

                    for (int i = 0; i < doc.DocumentLines.Count; i++)
                    {
                        DocumentLine docLine = doc.DocumentLines[i];

                        if (docLine.UCheckPromo == "Y" || docLine.UCheckPromo == "YY")
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0")
                        {
                            if (string.IsNullOrEmpty(promotion.IsCombine)
                                || promotion.IsCombine == "N"
                                //  nếu promtion đang comobine nhưng schema id hiện tại khác schema id đã tồn tại thì không tiếp tục apply promotion
                                || (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && docLine.USchemaCode != schemaId)
                                //  nếu line đã có promotion, nhưng allowchain = Y thì không tiếp tục apply promotion
                                || allowChain == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && !string.IsNullOrEmpty(schemaId))
                            {
                                continue;
                            }
                        }

                        if (string.IsNullOrEmpty(doc.PromotionId)
                            && (string.IsNullOrEmpty(docLine.UPromoCode) || docLine.UPromoCode == "0")
                            && string.IsNullOrEmpty(docLine.USchemaCode) && (string.IsNullOrEmpty(schemaId) || schema == null))
                        {
                            //  check các dòng khác của đơn có chứa promotion/schema khác promotion/schema id hiện tại, thì không apply
                            bool hasPromo = false;
                            for (int j = i; j < doc.DocumentLines.Count; j++)
                            {
                                DocumentLine line = doc.DocumentLines[j];
                                if (line.ItemCode != docLine.ItemCode && line.LineNum != docLine.LineNum)
                                {
                                    if (!string.IsNullOrEmpty(line.UPromoCode))
                                    {
                                        string[] lst = line.UPromoCode.Replace(promoId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                        if (lst.Length != 0)
                                        {
                                            hasPromo = true;
                                            break;
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(line.USchemaCode))
                                    {
                                        string[] lst = line.USchemaCode.Replace(schemaId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                        if (lst.Length != 0)
                                        {
                                            hasPromo = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (hasPromo)
                            {
                                continue;
                            }
                        }

                        if (promoGetLine.LineType == PromoLineType.ItemCode)
                        {
                            if (docLine.ItemCode == promoGetLine.LineCode && docLine.UoMCode == promoGetLine.LineUom)
                            {
                                if (Apply_Single_Promotion_Content(ref docLine, ref newLinePromo, doc.DocumentLines.Count, doc.DocDate.Value, promoId, promotion.PromoName, promoGetLine, schemaId, schemaName, doc.UCompanyCode, doc.StoreId, voucherNum, doc.CardGroup))
                                {
                                    docLine.UCheckPromo = "Y";
                                }
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.BarCode)
                        {
                            if (docLine.BarCode == promoGetLine.LineCode)
                            {
                                if (Apply_Single_Promotion_Content(ref docLine, ref newLinePromo, doc.DocumentLines.Count, doc.DocDate.Value, promoId, promotion.PromoName, promoGetLine, schemaId, schemaName, doc.UCompanyCode, doc.StoreId, voucherNum, doc.CardGroup))
                                {
                                    docLine.UCheckPromo = "Y";
                                }
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.ItemGroup)
                        {
                            if (docLine.ItemGroup == promoGetLine.LineCode && (string.IsNullOrEmpty(promoGetLine.LineUom) || docLine.UoMCode == promoGetLine.LineUom))
                            {
                                if (Apply_Single_Promotion_Content(ref docLine, ref newLinePromo, doc.DocumentLines.Count, doc.DocDate.Value, promoId, promotion.PromoName, promoGetLine, schemaId, schemaName, doc.UCompanyCode, doc.StoreId, voucherNum, doc.CardGroup))
                                {
                                    docLine.UCheckPromo = "Y";
                                }
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.Collection)
                        {
                            if (docLine.UCollection == promoGetLine.LineCode)
                            {
                                if (Apply_Single_Promotion_Content(ref docLine, ref newLinePromo, doc.DocumentLines.Count, doc.DocDate.Value, promoId, promotion.PromoName, promoGetLine, schemaId, schemaName, doc.UCompanyCode, doc.StoreId, voucherNum, doc.CardGroup))
                                {
                                    docLine.UCheckPromo = "Y";
                                }
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.OneTimeGroup)
                        {
                            if (((promoGetLine.MaxQtyDis ?? 0) > 0 && promoGetLine.MaxQtyDis.Value < otQuantity)
                                || ((promoGetLine.MaxAmtDis ?? 0) > 0 && promoGetLine.MaxAmtDis < otAmount))
                            {
                                continue;
                            }

                            var lstGroupItemCode = promoGetLine.Lines.Where(x => x.LineType == PromoLineType.ItemCode && x.LineCode == docLine.ItemCode && x.LineUom == docLine.UoMCode && !x.IsCount);
                            if (lstGroupItemCode.Any())
                            {
                                SPromoOTGroup otGet = lstGroupItemCode.FirstOrDefault();
                                otQuantity += (docLine.Quantity ?? 0);
                                otAmount += (docLine.LineTotal ?? 0);

                                docLine.UCheckPromo = otGet.GroupID;

                                promoGetLine.Lines[promoGetLine.Lines.IndexOf(otGet)].IsCount = true;
                            }
                            else
                            {
                                var lstGroupBarCode = promoGetLine.Lines.Where(x => x.LineType == PromoLineType.BarCode && x.LineCode == docLine.BarCode && !x.IsCount);
                                if (lstGroupBarCode.Any())
                                {
                                    SPromoOTGroup otGet = lstGroupItemCode.FirstOrDefault();
                                    otQuantity += (docLine.Quantity ?? 0);
                                    otAmount += (docLine.LineTotal ?? 0);

                                    docLine.UCheckPromo = otGet.GroupID;

                                    promoGetLine.Lines[promoGetLine.Lines.IndexOf(otGet)].IsCount = true;
                                }
                            }

                            //if (promoGetLine.Lines != null && promoGetLine.Lines.Count > 0)
                            //{
                            //    foreach (SPromoOTGroup item in promoGetLine.Lines)
                            //    {
                            //        if (item.IsCount)
                            //        {
                            //            //  kiểm tra nếu nó đã được đưa vào tính toán thì không tính nữa
                            //            continue;
                            //        }
                            //        if ((item.LineType == PromoLineType.ItemCode && docLine.ItemCode == item.LineCode && docLine.UoMCode == item.LineUom)
                            //                    || (item.LineType == PromoLineType.BarCode && docLine.BarCode == item.LineCode))
                            //        {
                            //            otQuantity += (docLine.Quantity ?? 0);
                            //            otAmount += (docLine.LineTotal ?? 0);

                            //            docLine.UCheckPromo = item.GroupID;
                            //            item.IsCount = true;
                            //            break;
                            //        }
                            //    }
                            //}
                        }

                        doc.DocumentLines[i] = docLine;
                    }

                    List<DocumentLine> lstNewLine = new List<DocumentLine>();
                    if (promoGetLine.LineType == PromoLineType.OneTimeGroup)
                    {
                        bool checkApply = false;
                        int MultiRate = 1;
                        if (promoGetLine.ConditionType == PromoCondition.Quantity)
                        {
                            if (promoGetLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoGetLine.Value1 <= otQuantity)
                                {
                                    //MultiRate = Convert.ToInt32(otQuantity / promoGetLine.Value1);
                                    MultiRate = (int)Math.Round((otQuantity / (promoGetLine.Value1 ?? 1)), RoundLengthQty, MidpointRounding.AwayFromZero);
                                    checkApply = true;
                                }
                            }
                            else if (promoGetLine.Condition1 == PromoCondition.FROM && promoGetLine.Condition2 == PromoCondition.TO)
                            {
                                if (otQuantity >= promoGetLine.Value1
                                        && (promoGetLine.Value2 == null || promoGetLine.Value2.Value == 0 || otQuantity <= promoGetLine.Value2.Value))
                                {
                                    checkApply = true;
                                }
                            }
                        }
                        else if (promoGetLine.ConditionType == PromoCondition.Amount)
                        {
                            if (promoGetLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoGetLine.Value1 <= otAmount)
                                {
                                    MultiRate = (int)Math.Round((otAmount / (promoGetLine.Value1 ?? 1)), RoundLengthQty, MidpointRounding.AwayFromZero);
                                    checkApply = true;
                                }
                            }
                            else if (promoGetLine.Condition1 == PromoCondition.FROM && promoGetLine.Condition2 == PromoCondition.TO)
                            {
                                if (otAmount >= promoGetLine.Value1
                                        && (promoGetLine.Value2 == null || promoGetLine.Value2.Value == 0 || otAmount <= promoGetLine.Value2.Value))
                                {
                                    checkApply = true;
                                }
                            }
                        }
                        if (checkApply)
                        {
                            double maxQtyDis = promoGetLine.MaxQtyDis ?? otQuantity;
                            double maxAmtDis = promoGetLine.MaxAmtDis ?? otAmount;

                            if (maxQtyDis == 0)
                            {
                                maxQtyDis = otQuantity;
                            }

                            if (maxAmtDis == 0)
                            {
                                maxAmtDis = otAmount;
                            }

                            List<DocumentLine> lstLineOrderBy = doc.DocumentLines.OrderBy(l => l.Quantity.HasValue ? l.Quantity.Value : 0).Where(l => l.UCheckPromo == promoGetLine.LineCode).ToList();
                            foreach (DocumentLine doclineCheck in lstLineOrderBy)
                            {
                                if (otAmount <= 0 || maxQtyDis <= 0 || maxAmtDis <= 0)
                                {
                                    break;
                                }
                                double maxQty = Math.Min(maxQtyDis, doclineCheck.Quantity.Value);
                                DocumentLine line = ApplyPromoForMixAndMatch(promoGetLine, doclineCheck, doc.DocumentLines.Count + lstNewLine.Count, promotion.PromoId, promotion.PromoName, schemaId, schemaName, voucherNum, maxQty, maxAmtDis, MultiRate, out DocumentLine newLine);

                                otAmount -= (doclineCheck.LineTotal ?? 0);
                                maxQtyDis -= (line.Quantity ?? 0);
                                maxAmtDis -= (line.UnitPrice ?? 0) * (line.Quantity ?? 0) * (line.DiscountPercent ?? 0) / 100;

                                for (int i = 0; i < doc.DocumentLines.Count; i++)
                                {
                                    DocumentLine docLine = doc.DocumentLines[i];
                                    if (docLine.LineNum == line.LineNum && docLine.ItemCode == line.ItemCode)
                                    {
                                        doc.DocumentLines[i] = line;
                                        break;
                                    }
                                }
                                if (newLine != null)
                                {
                                    lstNewLine.Add(newLine);
                                }
                            }
                        }
                    }

                    if (lstNewLine.Count > 0)
                    {
                        doc.DocumentLines.AddRange(lstNewLine);
                    }
                }
            }

            doc.DocumentLines.AddRange(newLinePromo);

            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            return doc;
        }

        private Document Apply_PrepaidCard_Promotion(Document doc, string schemaId, PromotionViewModel promotion, string voucherNum)
        {
            var lstBuyLine = promotion.PromoBuys.Where(x => x.InActive != "Y");// My.SapFactory.LoadPromotionBuyLine(promoId);
            var lstGetLine = promotion.PromoGets.Where(x => x.InActive != "Y");// My.SapFactory.LoadPromotionGetLine(promoId);

            //int MultiRate = 1;
            // Check Condition Buy Line
            ArrayList arlcheckAll = new ArrayList();
            //ArrayList arlMultiRate = new ArrayList();
            foreach (var promoBuyLine in lstBuyLine)
            {
                bool checkGrid = false;

                foreach (var docLine in doc.DocumentLines)
                {
                    if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0" && (string.IsNullOrEmpty(promotion.IsCombine) || promotion.IsCombine == "N" || (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && docLine.USchemaCode != schemaId)))
                    {
                        continue;
                    }

                    if (docLine.UnitPrice == 0)
                        continue;

                    if (promoBuyLine.LineType == PromoLineType.ItemCode)
                    {
                        if (docLine.ItemCode == promoBuyLine.LineCode && docLine.UoMCode == promoBuyLine.LineUom)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 == docLine.Quantity)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.Quantity.Value) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 == docLine.LineTotal)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.LineTotal) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    { checkGrid = true; }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.BarCode)
                    {
                        if (docLine.BarCode == promoBuyLine.LineCode)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 == docLine.Quantity)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.Quantity.Value) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    { checkGrid = true; }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 == docLine.LineTotal)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.LineTotal) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    { checkGrid = true; }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.ItemGroup)
                    {
                        if (docLine.ItemGroup == promoBuyLine.LineCode)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 == docLine.Quantity)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.Quantity.Value) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 == docLine.LineTotal)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.LineTotal) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    { checkGrid = true; }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.Collection)
                    {
                        if (docLine.UCollection == promoBuyLine.LineCode)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.Quantity)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.Quantity.Value) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    { checkGrid = true; }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.LineTotal)
                                    {
                                        //arlMultiRate.Add(Convert.ToInt32(docLine.LineTotal) / Convert.ToInt32(promoBuyLine.Value1));
                                        checkGrid = true;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    { checkGrid = true; }
                                }
                            }
                        }
                    }

                    //dgr.EndEdit();
                }

                arlcheckAll.Add(checkGrid);
            }

            bool checkResult = false;
            foreach (bool c in arlcheckAll)
            {
                if (c)
                {
                    checkResult = c;
                }
                else
                {
                    checkResult = c;
                    break;
                }
            }

            //if (arlMultiRate.Count > 0)
            //{
            //    arlMultiRate.Sort();
            //    if (MultiRate < Convert.ToInt32(arlMultiRate[0]))
            //        MultiRate = Convert.ToInt32(arlMultiRate[0]);
            //    else
            //        MultiRate = 1;
            //}

            if (checkResult)
            {
                string schemaName = "";
                if (dicPromoSchemas.ContainsKey(schemaId))
                {
                    SchemaViewModel schema = dicPromoSchemas[schemaId];
                    schemaName = schema != null ? schema.SchemaName : "";
                }

                //  apply promo to SO
                foreach (var promoGetLine in lstGetLine)
                {
                    foreach (DocumentLine docLine in doc.DocumentLines)
                    {
                        if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0" && (string.IsNullOrEmpty(promotion.IsCombine) || promotion.IsCombine == "N" || (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && docLine.USchemaCode != schemaId)))
                        {
                            continue;
                        }
                        if (promoGetLine.LineType == PromoLineType.ItemCode)
                        {
                            if (promoGetLine.LineCode == docLine.ItemCode && promoGetLine.LineUom == docLine.UoMCode)
                            {
                                if (promoGetLine.ValueType == PromoValueType.BonusPercent)
                                {
                                    if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                    {
                                        docLine.DiscountPercent = promoGetLine.GetValue ?? 0;
                                    }
                                    else
                                    {
                                        double firstPrnct = docLine.DiscountPercent ?? 0;
                                        double newPrnct = promoGetLine.GetValue ?? 0;
                                        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                        docLine.DiscountPercent = lastPrcnt;
                                    }
                                    docLine.PromoType = PromoValueType.BonusPercent;

                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                }
                                if (promoGetLine.ValueType == PromoValueType.BonusAmount)
                                {
                                    if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                    {
                                        docLine.DiscountPercent = 100 * promoGetLine.GetValue.Value / docLine.LineTotal;
                                    }
                                    else
                                    {
                                        double firstPrnct = docLine.DiscountPercent ?? 0;
                                        double newPrnct = promoGetLine.GetValue ?? 0;
                                        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                        docLine.DiscountPercent = lastPrcnt;
                                    }
                                    docLine.PromoType = PromoValueType.BonusAmount;

                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                }
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.Collection)
                        {
                            if (docLine.UCollection == promoGetLine.LineCode)
                            {
                                if (promoGetLine.ValueType == PromoValueType.BonusPercent)
                                {
                                    if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                    {
                                        docLine.DiscountPercent = promoGetLine.GetValue ?? 0;
                                    }
                                    else
                                    {
                                        double firstPrnct = docLine.DiscountPercent ?? 0;
                                        double newPrnct = promoGetLine.GetValue ?? 0;
                                        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                        docLine.DiscountPercent = lastPrcnt;
                                    }
                                    docLine.PromoType = PromoValueType.BonusPercent;

                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                }
                                if (promoGetLine.ValueType == PromoValueType.BonusAmount)
                                {
                                    if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                    {
                                        docLine.DiscountPercent = 100 * promoGetLine.GetValue.Value / docLine.LineTotal;
                                    }
                                    else
                                    {
                                        double firstPrnct = docLine.DiscountPercent ?? 0;
                                        double newPrnct = promoGetLine.GetValue ?? 0;
                                        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                        docLine.DiscountPercent = lastPrcnt;
                                    }
                                    docLine.PromoType = PromoValueType.BonusAmount;

                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                }
                            }
                        }
                    }
                }
                CardSet_ReCalcGrid(ref doc);
                CardSet_SumTotalAmt(ref doc);
            }


            //fff
            return doc;
        }

        private void CardSet_ReCalcGrid(ref Document doc)
        {
            foreach (DocumentLine docLine in doc.DocumentLines)
            {
                var taxRate = docLine.TaxPercentagePerRow ?? 0;
                var ItemPrice = docLine.UnitPrice ?? 0;
                var ItemQty = docLine.Quantity ?? 0;
                var ItemDisRate = docLine.DiscountPercent ?? 0;
                var total = ItemPrice * ItemQty;
                var totalWithDiscount = total;// - (total * ItemDisRate / 100);
                //docLine.DiscountPercent = ItemDisRate;
                var totalWithTax = totalWithDiscount + totalWithDiscount * taxRate / 100;

                docLine.LineTotal = totalWithTax;
            }
        }

        private void CardSet_SumTotalAmt(ref Document doc)
        {
            //try
            //{
            var TotalQty = 0.0d;
            var TotalLine = 0.0d;
            var TotalLineDis = 0.0d;
            double totalLineTax = 0.0d;
            double totalPayable = 0.0d;
            //double TotalDepositAmnt = 0.0d;
            double TotalReturnAmnt = 0.0d;
            //double TotalAmtFur = 0.0d;
            //double TotalSumReceipt = 0.0d;
            //double totalWeight = 0;
            if (doc.DocumentLines.Count > 0)
            {
                foreach (DocumentLine docLine in doc.DocumentLines)
                {
                    var ItemPrice = docLine.UnitPrice ?? 0;
                    var ItemQty = docLine.Quantity ?? 0;
                    var ItemDisRate = docLine.DiscountPercent ?? 0;
                    var taxRate = docLine.TaxPercentagePerRow ?? 0;
                    var LineTotal = docLine.LineTotal ?? 0;

                    var total = ItemPrice * ItemQty;
                    var discountAmt = total * ItemDisRate / 100;
                    var totalWithDiscount = total;// - discountAmt;
                    var taxAmt = totalWithDiscount * taxRate / 100;
                    var totalWithTax = totalWithDiscount + taxAmt;
                    //if (drv["cDeliveryType"] != null && drv["cDeliveryType"].ToString() == "1" && total < 0)
                    //    TotalAmtFur += totalWithTax;
                    //if (HeaderDiscPrct > 0)
                    //{
                    //    var discountAmt2 = totalWithDiscount * HeaderDiscPrct / 100;
                    //    totalWithDiscount = totalWithDiscount - discountAmt2;
                    //    var taxAmt2 = totalWithDiscount * taxRate / 100;
                    //    totalWithTax = totalWithDiscount + taxAmt2;

                    //    discountAmt += discountAmt2;
                    //    taxAmt = taxAmt2;
                    //}

                    totalLineTax += taxAmt;
                    TotalLine += total;
                    TotalLineDis += discountAmt;
                    totalPayable += totalWithTax;
                    //TotalSumReceipt += LineTotal;
                    //TotalDepositAmnt += depositAmnt;
                    //totalWeight += (cbm * ItemQty);
                    if (ItemQty > 0)
                        TotalQty += ItemQty;
                    if (totalWithTax < 0)
                        TotalReturnAmnt += totalWithTax;
                    docLine.LineTotal = totalWithTax;
                    docLine.UDisPrcnt = docLine.DiscountPercent;
                    docLine.UDisAmt = discountAmt;
                    //docLine.UPriceAfDis = Math.Round(ItemPrice * (100 - ItemDisRate) / 100, 0);
                    docLine.UTotalAfDis = totalWithDiscount;
                    docLine.VatPerPriceAfDis = (docLine.UPriceAfDis ?? 0) * (docLine.TaxPercentagePerRow ?? 0) / 100;
                    //docLine.PriceAfDisAndVat = docLine.VatPerPriceAfDis + (docLine.UPriceAfDis ?? 0);
                }
            }
            //TotalLineDis = Math.Round(TotalLineDis / 100, RoundLength) * 100;
            //TotalLine = Math.Round(TotalLine / 100, RoundLength) * 100;
            //totalPayable = Math.Round(totalPayable, RoundLength);
            //totalLineTax = Math.Round(totalLineTax / 100, RoundLength) * 100;

            txtTotalPayable = totalPayable;// LoginUser.GnlSets.FormatCurrency);
        }

        private Document Apply_BuyXGetY_Promotion(Document doc, string schemaId, PromotionViewModel promotion, string voucherNum)
        {
            var lstBuyLine = promotion.PromoBuys.Where(x => x.InActive != "Y");
            var lstGetLine = promotion.PromoGets.Where(x => x.InActive != "Y");

            string schemaName = "";
            string allowChain = "";
            SchemaViewModel schema = null;
            if (dicPromoSchemas.ContainsKey(schemaId))
            {
                schema = dicPromoSchemas[schemaId];
                schemaName = schema != null ? schema.SchemaName : "";
                allowChain = schema.AllowChain;
            }

            int MultiRate = 1;
            // Check Condition Buy Line
            ArrayList arlcheckAll = new ArrayList();
            ArrayList arlMultiRate = new ArrayList();
            foreach (var promoBuyLine in lstBuyLine)
            {
                bool checkGrid = false;
                double otQuantity = 0;
                double otAmount = 0;

                double groupQuantity = 0;
                double groupAmount = 0;

                double collectQuantity = 0;
                double collectAmount = 0;

                int index = -1;
                foreach (var docLine in doc.DocumentLines)
                {
                    index++;
                    if (docLine.UCheckPromo == "Y")
                    {
                        //  nếu item nào đã được đánh dấu apply promotion thì không check nữa
                        continue;
                    }

                    if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0")
                    {
                        if (string.IsNullOrEmpty(promotion.IsCombine)
                            || promotion.IsCombine == "N"
                            //  nếu promtion đang comobine nhưng schema id hiện tại khác schema id đã tồn tại thì không tiếp tục apply promotion
                            || (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && docLine.USchemaCode != schemaId)
                            //  nếu line đã có promotion, nhưng allowchain = Y thì không tiếp tục apply promotion
                            || allowChain == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && !string.IsNullOrEmpty(schemaId))
                        {
                            continue;
                        }
                    }

                    if (string.IsNullOrEmpty(doc.PromotionId)
                    && (string.IsNullOrEmpty(docLine.UPromoCode) || docLine.UPromoCode == "0")
                    && string.IsNullOrEmpty(docLine.USchemaCode)
                    && (string.IsNullOrEmpty(schemaId) || schema == null))
                    {
                        bool hasPromo = false;
                        for (int i = index; i < doc.DocumentLines.Count; i++)
                        {
                            DocumentLine line = doc.DocumentLines[i];
                            if (line.ItemCode != docLine.ItemCode && line.LineNum != docLine.LineNum)
                            {
                                if (!string.IsNullOrEmpty(line.UPromoCode))
                                {
                                    string[] lst = line.UPromoCode.Replace(promotion.PromoId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                    if (lst.Length != 0)
                                    {
                                        hasPromo = true;
                                        break;
                                    }
                                }
                                else if (!string.IsNullOrEmpty(line.USchemaCode))
                                {
                                    string[] lst = line.USchemaCode.Replace(schemaId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                    if (lst.Length != 0)
                                    {
                                        hasPromo = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (hasPromo)
                        {
                            continue;
                        }
                    }

                    if (docLine.UnitPrice == 0)
                        continue;

                    if (promoBuyLine.LineType == PromoLineType.ItemCode)
                    {
                        if (docLine.ItemCode == promoBuyLine.LineCode && docLine.UoMCode == promoBuyLine.LineUom)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.Quantity)
                                    {
                                        arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.LineTotal)
                                    {
                                        arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    {
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.BarCode)
                    {
                        if (docLine.BarCode == promoBuyLine.LineCode)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.Quantity)
                                    {
                                        arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.LineTotal)
                                    {
                                        arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    {
                                        docLine.UCheckPromo = "Y";
                                        checkGrid = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.ItemGroup)
                    {
                        if (docLine.ItemGroup == promoBuyLine.LineCode && (string.IsNullOrEmpty(promoBuyLine.LineUom) || docLine.UoMCode == promoBuyLine.LineUom))
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                groupQuantity += docLine.Quantity ?? 0;
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                groupAmount += docLine.LineTotal ?? 0;
                            }

                            docLine.UCheckPromo = "Y";
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.Collection)
                    {
                        if (docLine.UCollection == promoBuyLine.LineCode && (string.IsNullOrEmpty(promoBuyLine.LineUom) || docLine.UoMCode == promoBuyLine.LineUom))
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                collectQuantity += docLine.Quantity ?? 0;
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                collectAmount += docLine.LineTotal ?? 0;
                            }

                            docLine.UCheckPromo = "Y";
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.OneTimeGroup)
                    {
                        if (promoBuyLine.Lines != null && promoBuyLine.Lines.Count > 0)
                        {
                            var lstGroupItemCode = promoBuyLine.Lines.Where(x => x.LineType == PromoLineType.ItemCode && x.LineCode == docLine.ItemCode && x.LineUom == docLine.UoMCode && !x.IsCount);
                            if (lstGroupItemCode.Any())
                            {
                                otQuantity += (docLine.Quantity ?? 0);
                                otAmount += (docLine.LineTotal ?? 0);

                                promoBuyLine.Lines[promoBuyLine.Lines.IndexOf(lstGroupItemCode.FirstOrDefault())].IsCount = true;
                            }
                            else
                            {
                                var lstGroupBarCode = promoBuyLine.Lines.Where(x => x.LineType == PromoLineType.BarCode && x.LineCode == docLine.BarCode && !x.IsCount);
                                if (lstGroupBarCode.Any())
                                {
                                    otQuantity += (docLine.Quantity ?? 0);
                                    otAmount += (docLine.LineTotal ?? 0);

                                    promoBuyLine.Lines[promoBuyLine.Lines.IndexOf(lstGroupBarCode.FirstOrDefault())].IsCount = true;

                                }
                            }

                            //foreach (SPromoOTGroup item in promoBuyLine.Lines)
                            //{
                            //    if (item.IsCount)
                            //    {
                            //        //  kiểm tra nếu nó đã được đưa vào tính toán thì không tính nữa
                            //        continue;
                            //    }

                            //    if ((item.LineType == PromoLineType.ItemCode && docLine.ItemCode == item.LineCode && docLine.UoMCode == item.LineUom)
                            //                || (item.LineType == PromoLineType.BarCode && docLine.BarCode == item.LineCode))
                            //    {
                            //        if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            //        {
                            //            otQuantity += docLine.Quantity ?? 0;
                            //        }
                            //        else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            //        {
                            //            otAmount += docLine.LineTotal ?? 0;
                            //        }

                            //        item.IsCount = true;
                            //        docLine.UCheckPromo = "Y";
                            //        break;
                            //    }
                            //}
                        }
                    }
                }

                if (promoBuyLine.LineType == PromoLineType.ItemGroup)
                {
                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= groupQuantity)
                            {
                                arlMultiRate.Add((int)(groupQuantity / promoBuyLine.Value1));
                                checkGrid = true;
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (groupQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || groupQuantity <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                            }
                        }
                    }
                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= groupAmount)
                            {
                                arlMultiRate.Add((int)(groupAmount / promoBuyLine.Value1));
                                checkGrid = true;
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (groupAmount >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || groupAmount <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                            }
                        }
                    }
                }
                else if (promoBuyLine.LineType == PromoLineType.Collection)
                {
                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= collectQuantity)
                            {
                                arlMultiRate.Add((int)(collectQuantity / promoBuyLine.Value1));
                                checkGrid = true;
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (collectQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || collectQuantity <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                            }
                        }
                    }
                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= collectAmount)
                            {
                                arlMultiRate.Add((int)(collectAmount / promoBuyLine.Value1));
                                checkGrid = true;
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (collectAmount >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || collectAmount <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                            }
                        }
                    }
                }
                else if (promoBuyLine.LineType == PromoLineType.OneTimeGroup)
                {
                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= otQuantity)
                            {
                                arlMultiRate.Add((int)(otQuantity / promoBuyLine.Value1));
                                checkGrid = true;
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (otQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || otQuantity <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                            }
                        }
                    }
                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= otAmount)
                            {
                                arlMultiRate.Add((int)(otAmount / promoBuyLine.Value1));
                                checkGrid = true;
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (otAmount >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || otAmount <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                            }
                        }
                    }
                }

                arlcheckAll.Add(checkGrid);
            }

            bool checkResult = false;
            foreach (bool c in arlcheckAll)
            {
                if (c)
                {
                    checkResult = c;
                }
                else
                {
                    checkResult = c;
                    break;
                }
            }

            if (arlMultiRate.Count > 0)
            {
                arlMultiRate.Sort();
                if (MultiRate < (int)(arlMultiRate[0]))
                    MultiRate = (int)(arlMultiRate[0]);
                else
                    MultiRate = 1;
            }

            if (checkResult)
            {
                // Apply Set Line
                int RowCount = doc.DocumentLines.Count;
                double quantity = MultiRate;
                List<DocumentLine> newLines = new List<DocumentLine>();
                foreach (var promoGetLine in lstGetLine)
                {
                    DocumentLine newLine = null;
                    if (promoGetLine.LineType == PromoLineType.ItemCode)
                    {
                        if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(MultiRate, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, quantity, 0, 0, promoGetLine.ValueType, promoGetLine.GetValue.Value, doc.UCompanyCode, doc.StoreId, doc.CardGroup);

                            if (newLine != null)
                            {
                                newLine.PromoType = PromoValueType.DiscountAmount;
                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }
                            }
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                        {
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(MultiRate, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, quantity, promoGetLine.GetValue.Value, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            if (newLine != null)
                            {
                                newLine.PromoType = PromoValueType.FixedPrice;
                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }
                            }
                        }
                        else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(MultiRate, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, quantity, 0, promoGetLine.GetValue.Value, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);

                            if (newLine != null)
                            {
                                newLine.PromoType = PromoValueType.DiscountPercent;
                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }

                                double lineTotal = (newLine.UnitPrice ?? 0) * (newLine.Quantity ?? 0);
                                double maxDiscount = promoGetLine.MaxAmtDis ?? 0;
                                double maxPercent = maxDiscount / lineTotal * 100;
                                if (maxPercent <= 0 || lineTotal == 0)
                                {
                                    maxPercent = promoGetLine.GetValue ?? 0;
                                }

                                newLine.DiscountPercent = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                            }
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            double fixQty = promoGetLine.GetValue.Value * MultiRate;
                            quantity = fixQty;
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(fixQty, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, quantity, 0, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);

                            if (newLine != null)
                            {
                                newLine.DiscountPercent = 100;
                                newLine.PromoType = PromoValueType.FixedQuantity;

                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }
                            }
                        }
                    }
                    else if (promoGetLine.LineType == PromoLineType.BarCode)
                    {
                        if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(MultiRate, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, quantity, 0, 0, promoGetLine.ValueType, promoGetLine.GetValue.Value, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            if (newLine != null)
                            {
                                newLine.PromoType = PromoValueType.DiscountAmount;
                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }
                            }
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                        {
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(MultiRate, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, quantity, promoGetLine.GetValue.Value, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            if (newLine != null)
                            {
                                newLine.PromoType = PromoValueType.FixedPrice;
                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }
                            }
                        }
                        else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(MultiRate, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, quantity, 0, promoGetLine.GetValue.Value, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            if (newLine != null)
                            {
                                newLine.PromoType = PromoValueType.DiscountPercent;
                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }

                                double lineTotal = (newLine.UnitPrice ?? 0) * (newLine.Quantity ?? 0);
                                double maxDiscount = promoGetLine.MaxAmtDis ?? 0;
                                double maxPercent = maxDiscount / lineTotal * 100;
                                if (maxPercent <= 0 || lineTotal == 0)
                                {
                                    maxPercent = promoGetLine.GetValue ?? 0;
                                }

                                newLine.DiscountPercent = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                            }
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            double fixQty = promoGetLine.GetValue.Value * MultiRate;
                            quantity = fixQty;
                            if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                            {
                                quantity = Math.Min(MultiRate, promoGetLine.MaxQtyDis.Value);
                            }
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, quantity, 0, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            if (newLine != null)
                            {
                                newLine.DiscountPercent = 100;
                                newLine.PromoType = PromoValueType.FixedQuantity;
                                newLine.UPromoCode += promotion.PromoId + ", ";
                                newLine.UPromoName = promotion.PromoName;
                                newLine.USchemaCode = schemaId;
                                newLine.USchemaName = schemaName;
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    newLine.RefTransId = voucherNum;
                                    newLine.ApplyType = "CRM";
                                }
                            }
                        }
                    }

                    if (newLine != null)
                    {
                        newLines.Add(newLine);
                    }
                }

                if (newLines.Count > 0)
                {
                    foreach (DocumentLine docLine in doc.DocumentLines)
                    {
                        if (docLine.UCheckPromo != "Y")
                        {
                            //  nếu item nào không được đánh dấu apply promotion thì không check nữa
                            continue;
                        }
                        //Phú đã xin phép Minh
                        //Sửa PromoType = Discount Percent 0% khi Promotion Buy dc áp dụng
                        foreach (var promoBuyLine in lstBuyLine)
                        {
                            if (promoBuyLine.LineType == PromoLineType.ItemCode)
                            {
                                if (docLine.ItemCode == promoBuyLine.LineCode && docLine.UoMCode == promoBuyLine.LineUom)
                                {
                                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                    {
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.Value1 <= docLine.Quantity)
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }
                                            }
                                        }
                                        if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                        {
                                            if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }

                                            }
                                        }
                                    }
                                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                    {
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.Value1 <= docLine.LineTotal)
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }

                                            }
                                        }
                                        if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                        {
                                            if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            else if (promoBuyLine.LineType == PromoLineType.BarCode)
                            {
                                if (docLine.BarCode == promoBuyLine.LineCode)
                                {
                                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                    {
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.Value1 <= docLine.Quantity)
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }

                                            }
                                        }
                                        if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                        {
                                            if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }

                                            }
                                        }
                                    }
                                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                    {
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.Value1 <= docLine.LineTotal)
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }

                                            }
                                        }
                                        if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                        {
                                            if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                            {
                                                docLine.UPromoCode += promotion.PromoId + ", ";
                                                docLine.UPromoName = promotion.PromoName;
                                                if (string.IsNullOrEmpty(docLine.PromoType))
                                                {
                                                    docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                    docLine.DiscountPercent = 0;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            else if (promoBuyLine.LineType == PromoLineType.ItemGroup)
                            {
                                if (docLine.ItemGroup == promoBuyLine.LineCode && (string.IsNullOrEmpty(promoBuyLine.LineUom) || docLine.UoMCode == promoBuyLine.LineUom))
                                {
                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (string.IsNullOrEmpty(docLine.PromoType))
                                    {
                                        docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                        docLine.DiscountPercent = 0;
                                    }
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                            }
                            else if (promoBuyLine.LineType == PromoLineType.Collection)
                            {
                                if (docLine.UCollection == promoBuyLine.LineCode && (string.IsNullOrEmpty(promoBuyLine.LineUom) || docLine.UoMCode == promoBuyLine.LineUom))
                                {
                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (string.IsNullOrEmpty(docLine.PromoType))
                                    {
                                        docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                        docLine.DiscountPercent = 0;
                                    }
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                            }
                            else if (promoBuyLine.LineType == PromoLineType.OneTimeGroup)
                            {
                                if (promoBuyLine.Lines != null && promoBuyLine.Lines.Count > 0)
                                {
                                    foreach (SPromoOTGroup item in promoBuyLine.Lines)
                                    {
                                        if (!item.IsCount)
                                        {
                                            //  kiểm tra nếu nó đã được đưa vào tính toán thì mới set promotion
                                            continue;
                                        }
                                        if ((item.LineType == PromoLineType.ItemCode && docLine.ItemCode == item.LineCode && docLine.UoMCode == item.LineUom)
                                                || (item.LineType == PromoLineType.BarCode && docLine.BarCode == item.LineCode))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                            if (string.IsNullOrEmpty(docLine.PromoType))
                                            {
                                                docLine.PromoType = PromoValueType.DiscountPercent;//"Discount Percent";
                                                docLine.DiscountPercent = 0;
                                            }
                                            if (!string.IsNullOrEmpty(schemaId))
                                            {
                                                docLine.USchemaCode = schemaId;
                                                docLine.USchemaName = schemaName;
                                            }
                                            if (!string.IsNullOrEmpty(voucherNum))
                                            {
                                                docLine.RefTransId = voucherNum;
                                                docLine.ApplyType = "CRM";
                                            }
                                            item.IsCount = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    doc.DocumentLines.AddRange(newLines);
                }
            }

            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            return doc;
        }

        private Document Apply_Combo_Promotion(Document doc, string schemaId, PromotionViewModel promotion, string voucherNum)
        {
            string schemaName = "";
            string allowChain = "";
            SchemaViewModel schema = null;
            if (dicPromoSchemas.ContainsKey(schemaId))
            {
                schema = dicPromoSchemas[schemaId];
                schemaName = schema != null ? schema.SchemaName : "";
                allowChain = schema.AllowChain;
            }

            int MultiRate = 1;
            // Check Condition Buy Line
            ArrayList arlcheckAll = new ArrayList();
            ArrayList arlMultiRate = new ArrayList();
            //ArrayList arlComboSum = new ArrayList();
            Dictionary<string, double> dicComboSum = new Dictionary<string, double>();
            //int otGroupIndex = 1;
            foreach (var promoBuyLine in promotion.PromoBuys)
            {
                if (promoBuyLine.InActive == "Y")
                {
                    continue;
                }
                bool checkGrid = false;
                double otQuantity = 0;
                double otAmount = 0;

                int index = -1;
                foreach (DocumentLine docLine in doc.DocumentLines)
                {
                    index++;
                    if (docLine.UCheckPromo == "Y" | docLine.UCheckPromo.StartsWith("Y"))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0" && (string.IsNullOrEmpty(promotion.IsCombine) || promotion.IsCombine == "N" || (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && docLine.USchemaCode != schemaId)))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0" && !string.IsNullOrEmpty(docLine.USchemaCode) && !string.IsNullOrEmpty(schemaId))
                    {
                        if (allowChain == "Y")
                        {
                            continue;
                        }
                    }

                    if (string.IsNullOrEmpty(doc.PromotionId)
                    && (string.IsNullOrEmpty(docLine.UPromoCode) || docLine.UPromoCode == "0")
                    && string.IsNullOrEmpty(docLine.USchemaCode)
                    && (string.IsNullOrEmpty(schemaId) || schema == null))
                    {
                        bool hasPromo = false;
                        for (int i = index; i < doc.DocumentLines.Count; i++)
                        {
                            DocumentLine line = doc.DocumentLines[i];
                            if (line.ItemCode != docLine.ItemCode && line.LineNum != docLine.LineNum)
                            {
                                if (!string.IsNullOrEmpty(line.UPromoCode))
                                {
                                    string[] lst = line.UPromoCode.Replace(promotion.PromoId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                    if (lst.Length != 0)
                                    {
                                        hasPromo = true;
                                        break;
                                    }
                                }
                                else if (!string.IsNullOrEmpty(line.USchemaCode))
                                {
                                    string[] lst = line.USchemaCode.Replace(schemaId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                    if (lst.Length != 0)
                                    {
                                        hasPromo = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (hasPromo)
                        {
                            continue;
                        }
                    }

                    //  checkcheck
                    if (docLine.UnitPrice == 0)
                        continue;

                    if (promoBuyLine.LineType == PromoLineType.ItemCode)
                    {
                        if (docLine.ItemCode == promoBuyLine.LineCode && docLine.UoMCode == promoBuyLine.LineUom)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.Quantity)
                                    {
                                        arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y01" + promoBuyLine.KeyId;
                                        //comboLineTotal += docLine.LineTotal.Value * ((int)arlMultiRate[arlMultiRate.Count - 1] * promoBuyLine.Value1.Value / docLine.Quantity.Value);
                                        //arlComboSum.Add(docLine.LineTotal.Value / docLine.Quantity.Value * promoBuyLine.Value1);
                                        //dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value / docLine.Quantity.Value * promoBuyLine.Value1.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y02" + promoBuyLine.KeyId;
                                        //comboLineTotal += docLine.LineTotal.Value;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.LineTotal)
                                    {
                                        arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y03" + promoBuyLine.KeyId;
                                        //comboLineTotal += (docLine.LineTotal.Value / (int)arlMultiRate[arlMultiRate.Count - 1]);
                                        //arlComboSum.Add(promoBuyLine.Value1.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, promoBuyLine.Value1.Value);
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y04" + promoBuyLine.KeyId;
                                        //comboLineTotal += docLine.LineTotal.Value;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.BarCode)
                    {
                        if (docLine.BarCode == promoBuyLine.LineCode)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.Quantity)
                                    {
                                        arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y05" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value / docLine.Quantity.Value * promoBuyLine.Value1);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y06" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.LineTotal)
                                    {
                                        arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y07" + promoBuyLine.KeyId;
                                        ////comboLineTotal += (docLine.LineTotal.Value / (int)arlMultiRate[arlMultiRate.Count - 1]);
                                        //arlComboSum.Add(promoBuyLine.Value1.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, promoBuyLine.Value1.Value);
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y08" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.ItemGroup)
                    {
                        if (docLine.ItemGroup == promoBuyLine.LineCode)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.Quantity)
                                    {
                                        arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y09" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value / docLine.Quantity.Value * promoBuyLine.Value1);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y10" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.LineTotal)
                                    {
                                        arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y11" + promoBuyLine.KeyId;
                                        ////comboLineTotal += (docLine.LineTotal.Value / (int)arlMultiRate[arlMultiRate.Count - 1]);
                                        //arlComboSum.Add(promoBuyLine.Value1.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, promoBuyLine.Value1.Value);
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y12" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.Collection)
                    {
                        if (docLine.UCollection == promoBuyLine.LineCode)
                        {
                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.Quantity)
                                    {
                                        arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y13" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value / docLine.Quantity.Value * promoBuyLine.Value1);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                                else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y14" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                            {
                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                {
                                    if (promoBuyLine.Value1 <= docLine.LineTotal)
                                    {
                                        arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y15" + promoBuyLine.KeyId;
                                        ////comboLineTotal += (docLine.LineTotal.Value / (int)arlMultiRate[arlMultiRate.Count - 1]);
                                        //arlComboSum.Add(promoBuyLine.Value1.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, promoBuyLine.Value1.Value);
                                        break;
                                    }
                                }
                                if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                {
                                    if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                    {
                                        checkGrid = true;
                                        docLine.UCheckPromo = "Y16" + promoBuyLine.KeyId;
                                        //arlComboSum.Add(docLine.LineTotal.Value);
                                        dicComboSum.Add(docLine.UCheckPromo, docLine.LineTotal.Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.OneTimeGroup)
                    {
                        if (promoBuyLine.Lines != null && promoBuyLine.Lines.Count > 0)
                        {
                            foreach (SPromoOTGroup item in promoBuyLine.Lines)
                            {
                                if (item.IsCount)
                                {
                                    //  kiểm tra nếu nó đã được đưa vào tính toán thì không tính nữa
                                    continue;
                                }
                                if ((item.LineType == PromoLineType.ItemCode && docLine.ItemCode == item.LineCode && docLine.UoMCode == item.LineUom)
                                            || (item.LineType == PromoLineType.BarCode && docLine.BarCode == item.LineCode))
                                {
                                    //if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                    //{
                                    //}
                                    //else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                    //{
                                    //}
                                    otQuantity += (docLine.Quantity ?? 0);
                                    otAmount += (docLine.LineTotal ?? 0);

                                    //docLine.UCheckPromo = "Y17-" + otGroupIndex.ToString();
                                    docLine.UCheckPromo = "Y17-" + promoBuyLine.KeyId;
                                    item.IsCount = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (promoBuyLine.LineType == PromoLineType.OneTimeGroup)
                {
                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= otQuantity)
                            {
                                arlMultiRate.Add((int)(otQuantity / promoBuyLine.Value1));
                                checkGrid = true;
                                ////comboLineTotal += (otQuantity / (int)arlMultiRate[arlMultiRate.Count - 1]);
                                //arlComboSum.Add(otAmount / otQuantity * promoBuyLine.Value1.Value);
                                //dicComboSum.Add("Y17-" + otGroupIndex.ToString(), otAmount / otQuantity * promoBuyLine.Value1.Value);
                                //dicComboSum.Add("Y17-" + otGroupIndex.ToString(), otAmount);
                                dicComboSum.Add("Y17-" + promoBuyLine.KeyId, otAmount);
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (otQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || otQuantity <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                                ////comboLineTotal += otAmount;
                                //arlComboSum.Add(otAmount);
                                //dicComboSum.Add("Y17-" + otGroupIndex.ToString(), otAmount);
                                dicComboSum.Add("Y17-" + promoBuyLine.KeyId, otAmount);
                            }
                        }
                    }
                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                    {
                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                        {
                            if (promoBuyLine.Value1 <= otAmount)
                            {
                                arlMultiRate.Add((int)(otAmount / promoBuyLine.Value1));
                                checkGrid = true;
                                ////comboLineTotal += (otQuantity / (int)arlMultiRate[arlMultiRate.Count - 1]);
                                //arlComboSum.Add(promoBuyLine.Value1.Value);
                                //dicComboSum.Add("Y17-" + otGroupIndex.ToString(), promoBuyLine.Value1.Value);
                                dicComboSum.Add("Y17-" + promoBuyLine.KeyId, promoBuyLine.Value1.Value);
                            }
                        }
                        else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                        {
                            if (otQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || otQuantity <= promoBuyLine.Value2.Value))
                            {
                                checkGrid = true;
                                //comboLineTotal += otAmount;
                                //arlComboSum.Add(otAmount);
                                //dicComboSum.Add("Y17-" + otGroupIndex.ToString(), otAmount);
                                dicComboSum.Add("Y17-" + promoBuyLine.KeyId, otAmount);
                            }
                        }
                    }

                    //otGroupIndex++;
                }

                arlcheckAll.Add(checkGrid);
            }

            bool checkResult = false;
            foreach (bool c in arlcheckAll)
            {
                if (c)
                { checkResult = c; }
                else
                {
                    checkResult = c;
                    break;
                }
            }

            if (arlMultiRate.Count > 0)
            {
                arlMultiRate.Sort();
                if (MultiRate < (int)(arlMultiRate[0]))
                    MultiRate = (int)(arlMultiRate[0]);
                else
                    MultiRate = 1;
            }

            if (checkResult)
            {
                string GetType = promotion.TotalGetType;
                double GetValue = promotion.TotalGetValue ?? 0;

                double comboLineTotal = 0.0;
                //foreach (DocumentLine docLine in doc.DocumentLines)
                //{
                //    if (docLine.UCheckPromo == "Y")
                //    {
                //        comboLineTotal += docLine.LineTotal ?? 0;
                //    }
                //}
                //foreach (var arlSum in arlComboSum)
                //{
                //    comboLineTotal += (double)arlSum * MultiRate;
                //}

                List<string> lstKey = dicComboSum.Keys.ToList();
                foreach (var item in lstKey)
                {
                    comboLineTotal += dicComboSum[item];
                }

                //comboLineTotal *= MultiRate;

                if (GetType == PromoValueType.DiscountAmount || GetType == PromoValueType.FixedPrice)
                {
                    if ((promotion.MaxTotalGetValue ?? 0) > 0)
                    {
                        MultiRate = Math.Min(MultiRate, int.Parse(promotion.MaxTotalGetValue.Value.ToString()));
                    }
                    GetValue *= MultiRate;

                    GetValue = Math.Min(GetValue, comboLineTotal);
                }
                else if (GetType == PromoValueType.DiscountPercent)
                {
                    double MaxGetValue = promotion.MaxTotalGetValue ?? 0;
                    if (comboLineTotal != 0)
                    {
                        if (MaxGetValue > 0)
                        {
                            double valueMax = MaxGetValue / comboLineTotal * 100;
                            GetValue = Math.Min(valueMax, GetValue);
                        }
                    }
                    else
                    {
                        GetValue = 0;
                    }
                }
                //else if (GetType == PromoValueType.FixedPrice)
                //{
                //    if ((promotion.MaxTotalGetValue ?? 0) > 0)
                //    {
                //        MultiRate = Math.Min(MultiRate, int.Parse(promotion.MaxTotalGetValue.Value.ToString()));
                //    }
                //    GetValue *= MultiRate;
                //}

                List<DocumentLine> newLines = new List<DocumentLine>();
                foreach (var promoBuyLine in promotion.PromoBuys)
                {
                    if (promoBuyLine.InActive == "Y")
                    {
                        continue;
                    }

                    double openValue = Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero);

                    foreach (DocumentLine docLine in doc.DocumentLines)
                    {
                        if (openValue <= 0)
                        {
                            break;
                        }

                        if (string.IsNullOrEmpty(docLine.PromoType) && (docLine.UCheckPromo == "Y" | docLine.UCheckPromo.StartsWith("Y")))
                        {
                            if (promoBuyLine.LineType == PromoLineType.ItemCode && docLine.UCheckPromo.EndsWith(promoBuyLine.KeyId))
                            {
                                if (docLine.ItemCode == promoBuyLine.LineCode && docLine.UoMCode == promoBuyLine.LineUom)
                                {
                                    if (GetType == PromoValueType.DiscountAmount)
                                    {
                                        double indexTotal = dicComboSum[docLine.UCheckPromo];
                                        double percentTotal = indexTotal / comboLineTotal * 100;
                                        //double percentTotal = (docLine.LineTotal ?? 0) / comboLineTotal * 100;
                                        double newValue = percentTotal * GetValue / 100;

                                        double newPrct = newValue / (docLine.LineTotal ?? 1) * 100;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = newPrct;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = newPrct;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.DiscountAmount;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                    else if (GetType == PromoValueType.DiscountPercent)
                                    {
                                        double maxQty = docLine.Quantity ?? 0;
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                            {
                                                //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                maxQty = Math.Min(maxQty, openValue);
                                            }
                                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                            {
                                                //...
                                            }
                                        }

                                        if ((docLine.Quantity ?? 0) > maxQty)
                                        {
                                            DocumentLine newLine = docLine.Clone();
                                            //newLine.Quantity -= maxQty;
                                            newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                            newLine.UCheckPromo = "YY";
                                            newLines.Add(newLine);

                                            docLine.Quantity = maxQty;
                                            docLine.LineTotal = GetLineTotal(docLine);
                                        }

                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = GetValue;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = GetValue;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.DiscountPercent;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                    else if (GetType == PromoValueType.FixedPrice)
                                    {
                                        double maxQty = docLine.Quantity ?? 0;
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                            {
                                                //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                maxQty = Math.Min(maxQty, openValue);
                                            }
                                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                            {
                                                //...
                                            }
                                        }

                                        if ((docLine.Quantity ?? 0) > maxQty)
                                        {
                                            DocumentLine newLine = docLine.Clone();
                                            //newLine.Quantity -= maxQty;
                                            newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                            newLine.UCheckPromo = "YY";
                                            newLines.Add(newLine);

                                            docLine.Quantity = maxQty;
                                            docLine.LineTotal = GetLineTotal(docLine);

                                            double diff = dicComboSum[docLine.UCheckPromo] - docLine.LineTotal.Value;
                                            comboLineTotal -= diff;
                                            dicComboSum[docLine.UCheckPromo] = docLine.LineTotal.Value;
                                        }

                                        double indexTotal = dicComboSum[docLine.UCheckPromo];
                                        double percentTotal = indexTotal / comboLineTotal * 100;
                                        //double percentTotal = (docLine.LineTotal ?? 0) / comboLineTotal * 100;
                                        double newValue = percentTotal * GetValue / 100;

                                        double newPrct = Math.Max(0, (docLine.LineTotal ?? 0) - newValue) / (docLine.LineTotal ?? 1) * 100;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = newPrct;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = newPrct;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.FixedPrice;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                }
                            }
                            else if (promoBuyLine.LineType == PromoLineType.BarCode && docLine.UCheckPromo.EndsWith(promoBuyLine.KeyId))
                            {
                                if (docLine.BarCode == promoBuyLine.LineCode)
                                {
                                    if (GetType == PromoValueType.DiscountAmount)
                                    {
                                        double indexTotal = dicComboSum[docLine.UCheckPromo];
                                        double percentTotal = indexTotal / comboLineTotal * 100;
                                        double newValue = percentTotal * GetValue / 100;

                                        double newPrct = newValue / (docLine.LineTotal ?? 1) * 100;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = newPrct;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = newPrct;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.DiscountAmount;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                    else if (GetType == PromoValueType.DiscountPercent)
                                    {
                                        double maxQty = docLine.Quantity ?? 0;
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                            {
                                                //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                maxQty = Math.Min(maxQty, openValue);
                                            }
                                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                            {
                                                //...
                                            }
                                        }

                                        if ((docLine.Quantity ?? 0) > maxQty)
                                        {
                                            DocumentLine newLine = docLine.Clone();
                                            //newLine.Quantity -= maxQty;
                                            newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                            newLine.UCheckPromo = "YY";
                                            newLines.Add(newLine);

                                            docLine.Quantity = maxQty;
                                            docLine.LineTotal = GetLineTotal(docLine);
                                        }

                                        //docLine.DiscountPercent = GetValue;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = GetValue;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = GetValue;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.DiscountPercent;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                    else if (GetType == PromoValueType.FixedPrice)
                                    {
                                        double maxQty = docLine.Quantity ?? 0;
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                            {
                                                //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                maxQty = Math.Min(maxQty, openValue);
                                            }
                                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                            {
                                                //...
                                            }
                                        }

                                        if ((docLine.Quantity ?? 0) > maxQty)
                                        {
                                            DocumentLine newLine = docLine.Clone();
                                            //newLine.Quantity -= maxQty;
                                            newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                            newLine.UCheckPromo = "YY";
                                            newLines.Add(newLine);

                                            docLine.Quantity = maxQty;
                                            docLine.LineTotal = GetLineTotal(docLine);

                                            double diff = dicComboSum[docLine.UCheckPromo] - docLine.LineTotal.Value;
                                            comboLineTotal -= diff;
                                            dicComboSum[docLine.UCheckPromo] = docLine.LineTotal.Value;
                                        }

                                        double indexTotal = dicComboSum[docLine.UCheckPromo];
                                        double percentTotal = indexTotal / comboLineTotal * 100;
                                        //double percentTotal = (docLine.LineTotal ?? 0) / comboLineTotal * 100;
                                        double newValue = percentTotal * GetValue / 100;

                                        double newPrct = Math.Max(0, (docLine.LineTotal ?? 0) - newValue) / (docLine.LineTotal ?? 1) * 100;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = newPrct;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = newPrct;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.FixedPrice;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                }
                            }
                            else if (promoBuyLine.LineType == PromoLineType.Collection && docLine.UCheckPromo.EndsWith(promoBuyLine.KeyId))
                            {
                                if (docLine.UCollection == promoBuyLine.LineCode)
                                {
                                    if (GetType == PromoValueType.DiscountAmount)
                                    {
                                        double indexTotal = dicComboSum[docLine.UCheckPromo];
                                        double percentTotal = indexTotal / comboLineTotal * 100;
                                        double newValue = percentTotal * GetValue / 100;

                                        double newPrct = newValue / (docLine.LineTotal ?? 1) * 100;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = newPrct;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = newPrct;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.DiscountAmount;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                    else if (GetType == PromoValueType.DiscountPercent)
                                    {
                                        double maxQty = docLine.Quantity ?? 0;
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                            {
                                                //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                maxQty = Math.Min(maxQty, openValue);
                                            }
                                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                            {
                                                //...
                                            }
                                        }

                                        if ((docLine.Quantity ?? 0) > maxQty)
                                        {
                                            DocumentLine newLine = docLine.Clone();
                                            //newLine.Quantity -= maxQty;
                                            newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                            newLine.UCheckPromo = "YY";
                                            newLines.Add(newLine);

                                            docLine.Quantity = maxQty;
                                            docLine.LineTotal = GetLineTotal(docLine);
                                        }

                                        //docLine.DiscountPercent = GetValue;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = GetValue;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = GetValue;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.DiscountPercent;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                    else if (GetType == PromoValueType.FixedPrice)
                                    {
                                        double maxQty = docLine.Quantity ?? 0;
                                        if (promoBuyLine.Condition1 == PromoCondition.CE)
                                        {
                                            if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                            {
                                                //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                maxQty = Math.Min(maxQty, openValue);
                                            }
                                            else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                            {
                                                //...
                                            }
                                        }

                                        if ((docLine.Quantity ?? 0) > maxQty)
                                        {
                                            DocumentLine newLine = docLine.Clone();
                                            //newLine.Quantity -= maxQty;
                                            newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                            newLine.UCheckPromo = "YY";
                                            newLines.Add(newLine);

                                            docLine.Quantity = maxQty;
                                            docLine.LineTotal = GetLineTotal(docLine);

                                            double diff = dicComboSum[docLine.UCheckPromo] - docLine.LineTotal.Value;
                                            comboLineTotal -= diff;
                                            dicComboSum[docLine.UCheckPromo] = docLine.LineTotal.Value;
                                        }

                                        double indexTotal = dicComboSum[docLine.UCheckPromo];
                                        double percentTotal = indexTotal / comboLineTotal * 100;
                                        //double percentTotal = (docLine.LineTotal ?? 0) / comboLineTotal * 100;
                                        double newValue = percentTotal * GetValue / 100;

                                        double newPrct = Math.Max(0, (docLine.LineTotal ?? 0) - newValue) / (docLine.LineTotal ?? 1) * 100;
                                        if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                        {
                                            docLine.DiscountPercent = newPrct;
                                        }
                                        else
                                        {
                                            double firstPrnct = docLine.DiscountPercent ?? 0;
                                            double newPrnct = newPrct;
                                            double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                            docLine.DiscountPercent = lastPrcnt;
                                        }

                                        docLine.PromoType = PromoValueType.FixedPrice;

                                        if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                        {
                                            docLine.UPromoCode += promotion.PromoId + ", ";
                                            docLine.UPromoName = promotion.PromoName;
                                        }
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                }
                            }
                            else if (promoBuyLine.LineType == PromoLineType.OneTimeGroup && docLine.UCheckPromo.EndsWith(promoBuyLine.KeyId))
                            {
                                if (promoBuyLine.Lines != null && promoBuyLine.Lines.Count > 0)
                                {
                                    foreach (SPromoOTGroup item in promoBuyLine.Lines)
                                    {
                                        if (!item.IsCount)
                                        {
                                            continue;
                                        }

                                        if ((item.LineType == PromoLineType.ItemCode && docLine.ItemCode == item.LineCode && docLine.UoMCode == item.LineUom)
                                            || (item.LineType == PromoLineType.BarCode && docLine.BarCode == item.LineCode))
                                        {
                                            if (GetType == PromoValueType.DiscountAmount)
                                            {
                                                double indexTotal = dicComboSum[docLine.UCheckPromo];
                                                double percentTotal = indexTotal / comboLineTotal * 100;
                                                double newValue = percentTotal * GetValue / 100;

                                                if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                                {
                                                    openValue -= docLine.Quantity.Value;
                                                    newValue *= (docLine.Quantity.Value / (MultiRate * promoBuyLine.Value1.Value));
                                                }
                                                else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                                {
                                                    openValue -= docLine.LineTotal.Value;
                                                    newValue *= (docLine.LineTotal.Value / (MultiRate * promoBuyLine.Value1.Value));
                                                }

                                                double newPrct = newValue / indexTotal * 100;
                                                //double newPrct = newValue / (docLine.LineTotal ?? 1) * 100;

                                                if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                                {
                                                    docLine.DiscountPercent = Math.Min(newPrct, 100);
                                                }
                                                else
                                                {
                                                    double firstPrnct = docLine.DiscountPercent ?? 0;
                                                    double newPrnct = newPrct;
                                                    double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                                    docLine.DiscountPercent = Math.Min(lastPrcnt, 100);
                                                }

                                                docLine.PromoType = PromoValueType.DiscountAmount;

                                                if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                                {
                                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                                    docLine.UPromoName = promotion.PromoName;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }
                                            }
                                            else if (GetType == PromoValueType.DiscountPercent)
                                            {
                                                double maxQty = docLine.Quantity ?? 0;
                                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                                {
                                                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                                    {
                                                        //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                        maxQty = Math.Min(maxQty, openValue);
                                                    }
                                                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                                    {
                                                        //...
                                                    }
                                                }

                                                if ((docLine.Quantity ?? 0) > maxQty)
                                                {
                                                    DocumentLine newLine = docLine.Clone();
                                                    //newLine.Quantity -= maxQty;
                                                    newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                                    newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                                    newLine.UCheckPromo = "YY";
                                                    newLines.Add(newLine);

                                                    docLine.Quantity = maxQty;
                                                    docLine.LineTotal = GetLineTotal(docLine);
                                                }

                                                if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                                {
                                                    openValue -= docLine.Quantity.Value;
                                                }
                                                else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                                {
                                                    openValue -= docLine.LineTotal.Value;
                                                }

                                                if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                                {
                                                    docLine.DiscountPercent = GetValue;
                                                }
                                                else
                                                {
                                                    double firstPrnct = docLine.DiscountPercent ?? 0;
                                                    double newPrnct = GetValue;
                                                    double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                                    docLine.DiscountPercent = lastPrcnt;
                                                }

                                                docLine.PromoType = PromoValueType.DiscountPercent;

                                                if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                                {
                                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                                    docLine.UPromoName = promotion.PromoName;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }
                                            }
                                            else if (GetType == PromoValueType.FixedPrice)
                                            {
                                                double maxQty = docLine.Quantity ?? 0;
                                                if (promoBuyLine.Condition1 == PromoCondition.CE)
                                                {
                                                    if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                                    {
                                                        //maxQty = Math.Min(docLine.Quantity ?? 0, Math.Round(MultiRate * promoBuyLine.Value1.Value, RoundLengthQty, MidpointRounding.AwayFromZero));
                                                        maxQty = Math.Min(maxQty, openValue);
                                                    }
                                                    else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                                    {
                                                        //...
                                                    }
                                                }

                                                if ((docLine.Quantity ?? 0) > maxQty)
                                                {
                                                    DocumentLine newLine = docLine.Clone();
                                                    //newLine.Quantity -= maxQty;
                                                    newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                                    newLine.LineNum = doc.DocumentLines.Count + newLines.Count;
                                                    newLine.UCheckPromo = "YY";
                                                    newLines.Add(newLine);

                                                    docLine.Quantity = maxQty;
                                                    docLine.LineTotal = GetLineTotal(docLine);

                                                    double diff = dicComboSum[docLine.UCheckPromo] - docLine.LineTotal.Value;
                                                    comboLineTotal -= diff;
                                                    dicComboSum[docLine.UCheckPromo] = docLine.LineTotal.Value;
                                                }

                                                double indexTotal = dicComboSum[docLine.UCheckPromo];
                                                double percentTotal = indexTotal / comboLineTotal * 100;
                                                //double percentTotal = (docLine.LineTotal ?? 0) / comboLineTotal * 100;
                                                double newValue = percentTotal * GetValue / 100;

                                                if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                                {
                                                    openValue -= docLine.Quantity.Value;
                                                    newValue *= (docLine.Quantity.Value / (MultiRate * promoBuyLine.Value1.Value));
                                                }
                                                else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                                {
                                                    openValue -= docLine.LineTotal.Value;
                                                    newValue *= (docLine.LineTotal.Value / (MultiRate * promoBuyLine.Value1.Value));
                                                }

                                                double newPrct = Math.Max(0, (docLine.LineTotal ?? 0) - newValue) / (docLine.LineTotal ?? 1) * 100;
                                                if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                                {
                                                    docLine.DiscountPercent = newPrct;
                                                }
                                                else
                                                {
                                                    double firstPrnct = docLine.DiscountPercent ?? 0;
                                                    double newPrnct = newPrct;
                                                    double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                                    docLine.DiscountPercent = lastPrcnt;
                                                }


                                                docLine.PromoType = PromoValueType.FixedPrice;

                                                if (string.IsNullOrEmpty(docLine.UPromoCode) || !docLine.UPromoCode.Contains(promotion.PromoId))
                                                {
                                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                                    docLine.UPromoName = promotion.PromoName;
                                                }
                                                if (!string.IsNullOrEmpty(schemaId))
                                                {
                                                    docLine.USchemaCode = schemaId;
                                                    docLine.USchemaName = schemaName;
                                                }
                                                if (!string.IsNullOrEmpty(voucherNum))
                                                {
                                                    docLine.RefTransId = voucherNum;
                                                    docLine.ApplyType = "CRM";
                                                }
                                            }

                                            item.IsCount = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (newLines.Count > 0)
                {
                    doc.DocumentLines.AddRange(newLines);
                }
            }

            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            return doc;
        }

        private Document Apply_TotalBill_Promotion(Document doc, string schemaId, PromotionViewModel promotion, string voucherNum)
        {
            if (doc.DocumentLines.Count == 0)
            {
                return doc;
            }

            string schemaName = "";
            string allowChain = string.Empty;
            if (dicPromoSchemas.ContainsKey(schemaId))
            {
                SchemaViewModel schema = dicPromoSchemas[schemaId];
                schemaName = schema != null ? schema.SchemaName : "";
                allowChain = schema.AllowChain;
            }

            //if (!string.IsNullOrEmpty(doc.PromotionId) && doc.DiscountPercent != null && doc.DiscountPercent.Value != 0
            //    && (string.IsNullOrEmpty(promotion.IsCombine) || promotion.IsCombine == "N" || allowChain == "Y") && string.IsNullOrEmpty(schemaId))
            //{
            //    return doc;
            //}
            //else //if (string.IsNullOrEmpty(doc.PromotionId) || doc.DiscountPercent == null || doc.DiscountPercent.Value == 0)
            //{
            //    if (string.IsNullOrEmpty(allowChain) || allowChain == "Y")
            //    {
            //        //  check promotion line
            //        foreach (DocumentLine line in doc.DocumentLines)
            //        {
            //            if (!string.IsNullOrEmpty(line.UPromoCode) || !string.IsNullOrEmpty(line.USchemaCode))
            //            {
            //                return doc;
            //            }
            //        }
            //    }
            //}

            // Apply Get Header
            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);
            double totalPayable = txtTotalPayable;
            //if (Convert.ToDouble(txtTotalPayable) != 0 && promotion.TotalBuyFrom <= Convert.ToDouble(txtTotalPayable)
            //    && (promotion.TotalBuyTo == null || promotion.TotalBuyTo >= Convert.ToDouble(txtTotalPayable)))
            if (totalPayable != 0 && promotion.TotalBuyFrom <= totalPayable && (promotion.TotalBuyTo == null || promotion.TotalBuyTo >= totalPayable))
            {
                int multiRate = 1;
                if ((promotion.TotalBuyTo ?? 0) == 0)
                {
                    multiRate = (int)Math.Round((totalPayable / (promotion.TotalBuyFrom ?? 1)), RoundLengthQty, MidpointRounding.AwayFromZero);
                }

                double maxQtyByBill = promotion.MaxQtyByReceipt ?? 99999999999;
                if ((promotion.MaxQtyByStore ?? 0) != 0)
                {
                    //  get remaining qty
                    double remainQty = GetRemainingPromoQuantiy(doc.UCompanyCode, doc.StoreId, promotion.PromoId);
                    if ((promotion.MaxQtyByReceipt ?? 0) != 0)
                    {
                        maxQtyByBill = Math.Min(promotion.MaxQtyByReceipt.Value, remainQty);
                    }
                    else
                    {
                        maxQtyByBill = remainQty;
                    }
                }

                //1 - Discount Amount
                //2 - Discount Percent
                //3 - Fixed Amount

                foreach (var promoGetLine in promotion.PromoGets)
                {
                    if (promoGetLine.InActive == "Y")
                    {
                        continue;
                    }

                    DocumentLine newLine = null;
                    int RowCount = doc.DocumentLines.Count;

                    double maxQtyLine = maxQtyByBill;
                    if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                    {
                        maxQtyLine = promoGetLine.MaxQtyDis ?? 0;
                    }

                    if (promoGetLine.LineType == PromoLineType.ItemCode)
                    {
                        if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            double newQty = Math.Min((double)multiRate, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, newQty, 0, 0, promoGetLine.ValueType, promoGetLine.GetValue.Value, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                        {
                            double newQty = Math.Min((double)multiRate, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, newQty, promoGetLine.GetValue.Value, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                        else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            double newQty = Math.Min((double)multiRate, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, newQty, 0, promoGetLine.GetValue.Value, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            double newQty = Math.Min((double)multiRate * promoGetLine.GetValue.Value, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, newQty, 0, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                    }
                    else if (promoGetLine.LineType == PromoLineType.BarCode)
                    {
                        if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            double newQty = Math.Min((double)multiRate, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, newQty, 0, 0, promoGetLine.ValueType, promoGetLine.GetValue.Value, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                        {
                            double newQty = Math.Min((double)multiRate, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, newQty, promoGetLine.GetValue.Value, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                        else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            double newQty = Math.Min((double)multiRate, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, newQty, 0, promoGetLine.GetValue.Value, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                        else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            double newQty = Math.Min((double)multiRate * promoGetLine.GetValue.Value, Math.Min(maxQtyLine, maxQtyByBill));
                            newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, promoGetLine.GetValue.Value, 0, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                            maxQtyByBill -= newQty;
                        }
                    }

                    if (newLine != null)
                    {
                        newLine.UPromoCode += promotion.PromoId + ", ";
                        newLine.UPromoName = promotion.PromoName;
                        newLine.USchemaCode = schemaId;
                        newLine.USchemaName = schemaName;
                        doc.DocumentLines.Add(newLine);
                    }

                    if (maxQtyByBill <= 0)
                    {
                        break;
                    }
                }

                //// Apply Set Line
                //for (int j = 0; j < multiRate; j++)
                //{
                //    foreach (var promoGetLine in promotion.PromoGets)
                //    {
                //        DocumentLine newLine = null;
                //        int RowCount = doc.DocumentLines.Count;
                //        if (promoGetLine.LineType == PromoLineType.ItemCode)
                //        {
                //            bool isUpdate = false;
                //            for (int i = 0; i < doc.DocumentLines.Count; i++)
                //            {
                //                if (doc.DocumentLines[i].UIsPromo == "1")
                //                {
                //                    if (doc.DocumentLines[i].ItemCode == promoGetLine.LineCode && doc.DocumentLines[i].UoMCode == promoGetLine.LineUom)
                //                    {
                //                        if (promoGetLine.ValueType == PromoValueType.DiscountAmount || promoGetLine.ValueType == PromoValueType.FixedPrice || promoGetLine.ValueType == PromoValueType.DiscountPercent)
                //                        {
                //                            doc.DocumentLines[i].Quantity += Math.Min(1, maxQtyByBill);
                //                            maxQtyByBill -= 1;
                //                        }
                //                        else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                //                        {
                //                            doc.DocumentLines[i].Quantity += Math.Min(promoGetLine.GetValue.Value, maxQtyByBill);
                //                            maxQtyByBill -= promoGetLine.GetValue.Value;
                //                        }
                //                        isUpdate = true;
                //                        break;
                //                    }
                //                }
                //            }

                //            if (maxQtyByBill <= 0)
                //            {
                //                break;
                //            }

                //            if (!isUpdate)
                //            {
                //                if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, 1, 0, 0, promoGetLine.ValueType, promoGetLine.GetValue.Value, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= 1;
                //                }
                //                else if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, 1, promoGetLine.GetValue.Value, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= 1;
                //                }
                //                else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, 1, 0, promoGetLine.GetValue.Value, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= 1;
                //                }
                //                else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, false, string.Empty, promoGetLine.LineCode, promoGetLine.LineUom, promoGetLine.GetValue.Value, 0, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= promoGetLine.GetValue.Value;
                //                }
                //            }
                //        }
                //        else if (promoGetLine.LineType == PromoLineType.BarCode)
                //        {
                //            bool isUpdate = false;
                //            for (int i = 0; i < doc.DocumentLines.Count; i++)
                //            {
                //                if (doc.DocumentLines[i].UIsPromo == "1")
                //                {
                //                    if (doc.DocumentLines[i].BarCode == promoGetLine.LineCode && doc.DocumentLines[i].UoMCode == promoGetLine.LineUom)
                //                    {
                //                        if (promoGetLine.ValueType == PromoValueType.DiscountAmount || promoGetLine.ValueType == PromoValueType.FixedPrice || promoGetLine.ValueType == PromoValueType.DiscountPercent)
                //                        {
                //                            doc.DocumentLines[i].Quantity += Math.Min(1, maxQtyByBill);
                //                            maxQtyByBill -= 1;
                //                        }
                //                        else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                //                        {
                //                            doc.DocumentLines[i].Quantity += Math.Min(promoGetLine.GetValue.Value, maxQtyByBill);
                //                            maxQtyByBill -= promoGetLine.GetValue.Value;
                //                        }
                //                        isUpdate = true;
                //                        break;
                //                    }
                //                }
                //            }

                //            if (maxQtyByBill <= 0)
                //            {
                //                break;
                //            }

                //            if (!isUpdate)
                //            {
                //                if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, 1, 0, 0, promoGetLine.ValueType, promoGetLine.GetValue.Value, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= 1;
                //                }
                //                else if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, 1, promoGetLine.GetValue.Value, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= 1;
                //                }
                //                else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, 1, 0, promoGetLine.GetValue.Value, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= 1;
                //                }
                //                else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                //                {
                //                    newLine = Add_PromoLineToGrid(RowCount, true, promoGetLine.LineCode, string.Empty, promoGetLine.LineUom, promoGetLine.GetValue.Value, 0, 0, promoGetLine.ValueType, 0, doc.UCompanyCode, doc.StoreId, doc.CardGroup);
                //                    maxQtyByBill -= promoGetLine.GetValue.Value;
                //                }
                //            }
                //        }

                //        if (newLine != null)
                //        {
                //            newLine.UPromoCode += promotion.PromoId + ", ";
                //            newLine.UPromoName = promotion.PromoName;
                //            newLine.USchemaCode = schemaId;
                //            newLine.USchemaName = schemaName;
                //            doc.DocumentLines.Add(newLine);
                //        }

                //        if (maxQtyByBill <= 0)
                //        {
                //            break;
                //        }
                //    }

                //    if (maxQtyByBill <= 0)
                //    {
                //        break;
                //    }
                //}

                Set_ReCalcGrid(ref doc);
                Set_SumTotalAmt(ref doc);

                string MaxGetValue = promotion.MaxTotalGetValue.ToString() == "" || Convert.ToDouble(promotion.MaxTotalGetValue.ToString()) <= 0 ? null : promotion.MaxTotalGetValue.ToString();
                double newPercent = 0;
                if (promotion.TotalGetType == PromoValueType.DiscountAmount)
                {
                    double newDisAmt = promotion.TotalGetValue.Value * multiRate;
                    if (MaxGetValue != null && totalPayable > Convert.ToDouble(MaxGetValue))
                    {
                        newPercent = Math.Min(newDisAmt, Convert.ToDouble(MaxGetValue)) / totalPayable * 100;
                    }
                    else
                    {
                        newPercent = newDisAmt / totalPayable * 100;
                    }

                    //if (MaxGetValue != null && totalPayable > Convert.ToDouble(MaxGetValue))
                    //{
                    //    newPercent = promotion.TotalGetValue.Value / Convert.ToDouble(MaxGetValue) * 100;
                    //}
                    //else
                    //{
                    //    newPercent = promotion.TotalGetValue.Value / totalPayable * 100;
                    //}
                }
                else if (promotion.TotalGetType == PromoValueType.DiscountPercent)
                {
                    double tmpValue = totalPayable * promotion.TotalGetValue.Value / 100;
                    if (MaxGetValue != null && tmpValue > Convert.ToDouble(MaxGetValue))
                    {
                        newPercent = Convert.ToDouble(MaxGetValue) * 100 / totalPayable;
                    }
                    else
                    {
                        newPercent = promotion.TotalGetValue ?? 0;
                    }
                }
                else if (promotion.TotalGetType == PromoValueType.FixedPrice)
                {
                    newPercent = (totalPayable - promotion.TotalGetValue.Value) / totalPayable * 100;
                }

                newPercent = Math.Min(newPercent, 100);

                if (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(schemaId) && !string.IsNullOrEmpty(allowChain) && allowChain != "Y")
                {
                    double firstPrnct = doc.DiscountPercent ?? 0;
                    double lastPrcnt = (firstPrnct + newPercent) - (firstPrnct * newPercent) / 100;
                    doc.DiscountPercent = lastPrcnt;
                    if (doc.DiscountPercent > 0)
                    {
                        doc.PromotionId += promotion.PromoId + ",";
                    }
                }
                else if (string.IsNullOrEmpty(schemaId) || (doc.DiscountPercent ?? 0) == 0)
                {
                    doc.DiscountPercent = newPercent;
                    if (doc.DiscountPercent > 0)
                    {
                        doc.PromotionId += promotion.PromoId + ",";
                    }
                }


                //if (promotion.TotalGetType == PromoValueType.DiscountAmount)
                //{
                //    if (MaxGetValue != null && Convert.ToDouble(txtTotalPayable) > Convert.ToDouble(MaxGetValue))
                //    {
                //        doc.DiscountPercent = promotion.TotalGetValue.Value / Convert.ToDouble(MaxGetValue) * 100;
                //    }
                //    else
                //    {
                //        doc.DiscountPercent = promotion.TotalGetValue.Value / Convert.ToDouble(txtTotalPayable) * 100;
                //    }
                //}
                //else if (promotion.TotalGetType == PromoValueType.DiscountPercent)
                //{
                //    if (doc.DiscountPercent == null || doc.DiscountPercent.Value == 0)
                //    {
                //        double tmpValue = Convert.ToDouble(txtTotalPayable) * promotion.TotalGetValue.Value / 100;
                //        if (MaxGetValue != null && tmpValue > Convert.ToDouble(MaxGetValue))
                //        {
                //            double newPercent = Convert.ToDouble(MaxGetValue) * 100 / Convert.ToDouble(txtTotalPayable);
                //            doc.DiscountPercent = newPercent;
                //        }
                //        else
                //        {
                //            doc.DiscountPercent = promotion.TotalGetValue.Value;
                //        }
                //    }
                //    else if (promotion.IsCombine == "Y")
                //    {
                //        double firstPrnct = doc.DiscountPercent.Value;
                //        double newPrnct = promotion.TotalGetValue.Value;
                //        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                //        doc.DiscountPercent = lastPrcnt;
                //    }
                //}
                //else if (promotion.TotalGetType == PromoValueType.FixedPrice)
                //{
                //    doc.DiscountPercent = (Convert.ToDouble(txtTotalPayable) - promotion.TotalGetValue.Value) / Convert.ToDouble(txtTotalPayable) * 100;
                //}

                //if (doc.DiscountPercent > 0)
                //{
                //    doc.PromotionId = promotion.PromoId;
                //}
            }

            return doc;
        }

        private bool CheckTotalValue(PromotionViewModel promotion, double totalPayable)
        {
            return promotion.TotalBuyFrom == null || promotion.TotalBuyFrom.Value == 0
                || (promotion.TotalBuyFrom != null && promotion.TotalBuyFrom.Value > 0 && totalPayable != 0 && promotion.TotalBuyFrom <= totalPayable && (promotion.TotalBuyTo == null || promotion.TotalBuyTo.Value == 0 || promotion.TotalBuyTo >= totalPayable));
        }

        private Document Apply_MixMatch_Promotion(Document doc, string schemaId, PromotionViewModel promotion, string voucherNum)
        {
            var lstBuyLine = promotion.PromoBuys.Where(x => x.InActive != "Y");
            var lstGetLine = promotion.PromoGets.Where(x => x.InActive != "Y");

            string schemaName = "";
            string allowChain = "";
            SchemaViewModel schema = null;
            if (dicPromoSchemas.ContainsKey(schemaId))
            {
                schema = dicPromoSchemas[schemaId];
                schemaName = schema != null ? schema.SchemaName : "";
                allowChain = schema.AllowChain;
            }

            int MultiRate = 1;

            // Check Condition Buy Line
            ArrayList arlcheckAll = new ArrayList();
            ArrayList arlMultiRate = new ArrayList();

            double totalPayable = txtTotalPayable;
            if (promotion.TotalBuyFrom == null || promotion.TotalBuyFrom.Value == 0
                || (promotion.TotalBuyFrom != null && promotion.TotalBuyFrom.Value > 0 && totalPayable != 0 && promotion.TotalBuyFrom <= totalPayable && (promotion.TotalBuyTo == null || promotion.TotalBuyTo.Value == 0 || promotion.TotalBuyTo >= totalPayable)))
            {
                //  add true cho trường hợp buy không có
                arlcheckAll.Add(true);

                foreach (var promoBuyLine in lstBuyLine)
                {
                    bool checkGrid = false;
                    double otQuantity = 0;
                    double otAmount = 0;

                    double groupQuantity = 0;
                    double groupAmount = 0;

                    double collectQuantity = 0;
                    double collectAmount = 0;

                    int index = -1;
                    foreach (var docLine in doc.DocumentLines)
                    {
                        index++;
                        if (docLine.UCheckPromo == "Y")
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0")
                        {
                            if (string.IsNullOrEmpty(promotion.IsCombine)
                                || promotion.IsCombine == "N"
                                //  nếu promtion đang comobine nhưng schema id hiện tại khác schema id đã tồn tại thì không tiếp tục apply promotion
                                || (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && docLine.USchemaCode != schemaId)
                                //  nếu line đã có promotion, nhưng allowchain = Y thì không tiếp tục apply promotion
                                || allowChain == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && !string.IsNullOrEmpty(schemaId))
                            {
                                continue;
                            }
                        }

                        if (string.IsNullOrEmpty(doc.PromotionId)
                        && (string.IsNullOrEmpty(docLine.UPromoCode) || docLine.UPromoCode == "0")
                        && string.IsNullOrEmpty(docLine.USchemaCode)
                        && (string.IsNullOrEmpty(schemaId) || schema == null))
                        {
                            bool hasPromo = false;
                            for (int i = index; i < doc.DocumentLines.Count; i++)
                            {
                                DocumentLine line = doc.DocumentLines[i];
                                if (line.ItemCode != docLine.ItemCode && line.LineNum != docLine.LineNum)
                                {
                                    if (!string.IsNullOrEmpty(line.UPromoCode))
                                    {
                                        string[] lst = line.UPromoCode.Replace(promotion.PromoId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                        if (lst.Length != 0)
                                        {
                                            hasPromo = true;
                                            break;
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(line.USchemaCode))
                                    {
                                        string[] lst = line.USchemaCode.Replace(schemaId, "").Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                        if (lst.Length != 0)
                                        {
                                            hasPromo = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (hasPromo)
                            {
                                continue;
                            }
                        }

                        //if (docLine.UnitPrice == 0)
                        //    continue;

                        if (promoBuyLine.LineType == PromoLineType.ItemCode)
                        {
                            if (docLine.ItemCode == promoBuyLine.LineCode && docLine.UoMCode == promoBuyLine.LineUom)
                            {
                                if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                {
                                    if (promoBuyLine.Condition1 == PromoCondition.CE)
                                    {
                                        if (promoBuyLine.Value1 <= docLine.Quantity)
                                        {
                                            arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                    else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                    {
                                        if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                        {
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                }
                                else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                {
                                    if (promoBuyLine.Condition1 == PromoCondition.CE)
                                    {
                                        if (promoBuyLine.Value1 <= docLine.LineTotal)
                                        {
                                            arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                    else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                    {
                                        if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                        {
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (promoBuyLine.LineType == PromoLineType.BarCode)
                        {
                            if (docLine.BarCode == promoBuyLine.LineCode)
                            {
                                if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                {
                                    if (promoBuyLine.Condition1 == PromoCondition.CE)
                                    {
                                        if (promoBuyLine.Value1 <= docLine.Quantity)
                                        {
                                            arlMultiRate.Add((int)(docLine.Quantity.Value / promoBuyLine.Value1));
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                    else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                    {
                                        if (docLine.Quantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.Quantity <= promoBuyLine.Value2))
                                        {
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                }
                                else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                {
                                    if (promoBuyLine.Condition1 == PromoCondition.CE)
                                    {
                                        if (promoBuyLine.Value1 <= docLine.LineTotal)
                                        {
                                            arlMultiRate.Add((int)(docLine.LineTotal / promoBuyLine.Value1));
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                    else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                                    {
                                        if (docLine.LineTotal >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || docLine.LineTotal <= promoBuyLine.Value2))
                                        {
                                            docLine.UCheckPromo = "Y";
                                            checkGrid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (promoBuyLine.LineType == PromoLineType.ItemGroup)
                        {
                            if (docLine.ItemGroup == promoBuyLine.LineCode && (string.IsNullOrEmpty(promoBuyLine.LineUom) || docLine.UoMCode == promoBuyLine.LineUom))
                            {
                                if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                {
                                    groupQuantity += (docLine.Quantity ?? 0);
                                }
                                else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                {
                                    groupAmount += (docLine.LineTotal ?? 0);
                                }

                                docLine.UCheckPromo = "Y";
                            }
                        }
                        else if (promoBuyLine.LineType == PromoLineType.Collection)
                        {
                            if (docLine.UCollection == promoBuyLine.LineCode && (string.IsNullOrEmpty(promoBuyLine.LineUom) || docLine.UoMCode == promoBuyLine.LineUom))
                            {
                                if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                {
                                    collectQuantity += (docLine.Quantity ?? 0);
                                }
                                else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                {
                                    collectAmount += (docLine.LineTotal ?? 0);
                                }

                                docLine.UCheckPromo = "Y";
                            }
                        }
                        else if (promoBuyLine.LineType == PromoLineType.OneTimeGroup)
                        {
                            if (promoBuyLine.Lines != null && promoBuyLine.Lines.Count > 0)
                            {
                                var lstGroupItemCode = promoBuyLine.Lines.Where(x => x.LineType == PromoLineType.ItemCode && x.LineCode == docLine.ItemCode && x.LineUom == docLine.UoMCode && !x.IsCount);
                                if (lstGroupItemCode.Any())
                                {
                                    otQuantity += (docLine.Quantity ?? 0);
                                    otAmount += (docLine.LineTotal ?? 0);

                                    promoBuyLine.Lines[promoBuyLine.Lines.IndexOf(lstGroupItemCode.FirstOrDefault())].IsCount = true;
                                }
                                else
                                {
                                    var lstGroupBarCode = promoBuyLine.Lines.Where(x => x.LineType == PromoLineType.BarCode && x.LineCode == docLine.BarCode && !x.IsCount);
                                    if (lstGroupBarCode.Any())
                                    {
                                        otQuantity += (docLine.Quantity ?? 0);
                                        otAmount += (docLine.LineTotal ?? 0);

                                        promoBuyLine.Lines[promoBuyLine.Lines.IndexOf(lstGroupBarCode.FirstOrDefault())].IsCount = true;

                                    }
                                }

                                //foreach (SPromoOTGroup item in promoBuyLine.Lines)
                                //{
                                //    if (item.IsCount)
                                //    {
                                //        //  kiểm tra nếu nó đã được đưa vào tính toán thì không tính nữa
                                //        continue;
                                //    }
                                //    if ((item.LineType == PromoLineType.ItemCode && docLine.ItemCode == item.LineCode && docLine.UoMCode == item.LineUom)
                                //                || (item.LineType == PromoLineType.BarCode && docLine.BarCode == item.LineCode))
                                //    {
                                //        if (promoBuyLine.ValueType == PromoCondition.Quantity)
                                //        {
                                //            otQuantity += (docLine.Quantity ?? 0);
                                //        }
                                //        else if (promoBuyLine.ValueType == PromoCondition.Amount)
                                //        {
                                //            otAmount += (docLine.LineTotal ?? 0);
                                //        }

                                //        item.IsCount = true;
                                //        break;
                                //    }
                                //}
                            }
                        }
                    }

                    if (promoBuyLine.LineType == PromoLineType.ItemGroup)
                    {
                        if (promoBuyLine.ValueType == PromoCondition.Quantity)
                        {
                            if (promoBuyLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoBuyLine.Value1 <= groupQuantity)
                                {
                                    arlMultiRate.Add((int)(groupQuantity / promoBuyLine.Value1));
                                    checkGrid = true;
                                }
                            }
                            else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                            {
                                if (groupQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || groupQuantity <= promoBuyLine.Value2.Value))
                                {
                                    checkGrid = true;
                                }
                            }
                        }
                        else if (promoBuyLine.ValueType == PromoCondition.Amount)
                        {
                            if (promoBuyLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoBuyLine.Value1 <= groupAmount)
                                {
                                    arlMultiRate.Add((int)(groupAmount / promoBuyLine.Value1));
                                    checkGrid = true;
                                }
                            }
                            else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                            {
                                if (groupAmount >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || groupAmount <= promoBuyLine.Value2.Value))
                                {
                                    checkGrid = true;
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.Collection)
                    {
                        if (promoBuyLine.ValueType == PromoCondition.Quantity)
                        {
                            if (promoBuyLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoBuyLine.Value1 <= collectQuantity)
                                {
                                    arlMultiRate.Add((int)(collectQuantity / promoBuyLine.Value1));
                                    checkGrid = true;
                                }
                            }
                            else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                            {
                                if (collectQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || collectQuantity <= promoBuyLine.Value2.Value))
                                {
                                    checkGrid = true;
                                }
                            }
                        }
                        else if (promoBuyLine.ValueType == PromoCondition.Amount)
                        {
                            if (promoBuyLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoBuyLine.Value1 <= collectAmount)
                                {
                                    arlMultiRate.Add((int)(collectAmount / promoBuyLine.Value1));
                                    checkGrid = true;
                                }
                            }
                            else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                            {
                                if (collectAmount >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || collectAmount <= promoBuyLine.Value2.Value))
                                {
                                    checkGrid = true;
                                }
                            }
                        }
                    }
                    else if (promoBuyLine.LineType == PromoLineType.OneTimeGroup)
                    {
                        if (promoBuyLine.ValueType == PromoCondition.Quantity)
                        {
                            if (promoBuyLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoBuyLine.Value1 <= otQuantity)
                                {
                                    arlMultiRate.Add((int)(otQuantity / promoBuyLine.Value1));
                                    checkGrid = true;
                                }
                            }
                            else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                            {
                                if (otQuantity >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || otQuantity <= promoBuyLine.Value2.Value))
                                {
                                    checkGrid = true;
                                }
                            }
                        }
                        else if (promoBuyLine.ValueType == PromoCondition.Amount)
                        {
                            if (promoBuyLine.Condition1 == PromoCondition.CE)
                            {
                                if (promoBuyLine.Value1 <= otAmount)
                                {
                                    arlMultiRate.Add((int)(otAmount / promoBuyLine.Value1));
                                    checkGrid = true;
                                }
                            }
                            else if (promoBuyLine.Condition1 == PromoCondition.FROM && promoBuyLine.Condition2 == PromoCondition.TO)
                            {
                                if (otAmount >= promoBuyLine.Value1
                                        && (promoBuyLine.Value2 == null || promoBuyLine.Value2.Value == 0 || otAmount <= promoBuyLine.Value2.Value))
                                {
                                    checkGrid = true;
                                }
                            }
                        }
                    }

                    arlcheckAll.Add(checkGrid);
                }
            }

            bool checkResult = false;
            foreach (bool c in arlcheckAll)
            {
                if (c)
                {
                    checkResult = c;
                }
                else
                {
                    checkResult = c;
                    break;
                }
            }

            if (arlMultiRate.Count > 0)
            {
                arlMultiRate.Sort();
                if (MultiRate < (int)(arlMultiRate[0]))
                    MultiRate = (int)(arlMultiRate[0]);
                else
                    MultiRate = 1;
            }

            if (checkResult)
            {
                double maxQtyByBill = promotion.MaxQtyByReceipt ?? 99999999999;
                if ((promotion.MaxQtyByStore ?? 0) != 0)
                {
                    //  get remaining qty
                    double remainQty = GetRemainingPromoQuantiy(doc.UCompanyCode, doc.StoreId, promotion.PromoId);
                    if ((promotion.MaxQtyByReceipt ?? 0) != 0)
                    {
                        maxQtyByBill = Math.Min(promotion.MaxQtyByReceipt.Value, remainQty);
                    }
                    else
                    {
                        maxQtyByBill = remainQty;
                    }
                }

                if (arlMultiRate.Count <= 0)
                {
                    MultiRate = (int)Math.Min(999999999, maxQtyByBill);
                }

                //  apply promo to SO
                foreach (var promoGetLine in lstGetLine)
                {
                    double otQuantity = 0;
                    double otAmount = 0;

                    double groupQuantity = 0;
                    double groupAmount = 0;

                    double collectQuantity = 0;
                    double collectAmount = 0;

                    List<DocumentLine> lstNewLine = new List<DocumentLine>();

                    double getValue = promoGetLine.GetValue ?? 0;
                    if (getValue == 0)
                    {
                        continue;
                    }

                    foreach (DocumentLine docLine in doc.DocumentLines)
                    {
                        if (docLine.UCheckPromo == "YY")
                        {
                            continue;
                        }

                        if (getValue <= 0)
                        {
                            break;
                        }

                        if (!string.IsNullOrEmpty(docLine.UPromoCode) && docLine.UPromoCode != "0"
                            && (string.IsNullOrEmpty(promotion.IsCombine) || promotion.IsCombine == "N"
                                || (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(docLine.USchemaCode) && docLine.USchemaCode != schemaId)))
                        {
                            continue;
                        }
                        if (promoGetLine.LineType == PromoLineType.ItemCode)
                        {
                            if (promoGetLine.LineCode == docLine.ItemCode && promoGetLine.LineUom == docLine.UoMCode)
                            {
                                //  new
                                if ((promotion.TotalBuyFrom ?? 0) > 0)
                                {
                                    if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp mặt hàng là bàn cân, qty số thập phân
                                    {
                                        double totalPayableCheck = totalPayable - (docLine.LineTotal ?? 0);
                                        if (CheckTotalValue(promotion, totalPayableCheck))
                                        {
                                            totalPayable = totalPayableCheck;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else // trường hợp mặt hàng thường, có qty số nguyên
                                    {
                                        int i = (int)docLine.Quantity.Value;
                                        while (i > 0)
                                        {
                                            double totalPayableCheck = totalPayable - ((docLine.UnitPrice ?? 0) * i);
                                            if (CheckTotalValue(promotion, totalPayableCheck))
                                            {
                                                if (docLine.Quantity.Value - i > 0)
                                                {
                                                    DocumentLine newLine = docLine.Clone();
                                                    newLine.Quantity = docLine.Quantity.Value - i;
                                                    newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                                    newLine.UCheckPromo = "YY";
                                                    lstNewLine.Add(newLine);
                                                }
                                                docLine.Quantity = i;
                                                docLine.LineTotal = GetLineTotal(docLine);
                                                totalPayable = totalPayableCheck;
                                                break;
                                            }
                                            i--;
                                        }

                                        if (i == 0)
                                        {
                                            continue;
                                        }
                                    }

                                    //double range = Math.Round(totalPayable - promotion.TotalBuyFrom.Value, RoundLengthQty, MidpointRounding.AwayFromZero);
                                    //if (range >= (docLine.LineTotal ?? 0))
                                    //{
                                    //    totalPayable -= (docLine.LineTotal ?? 0);
                                    //}
                                    //else if (range > 0)
                                    //{
                                    //    double newQty = 0;
                                    //    if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp bàn cân chia qty lẻ
                                    //    {
                                    //        newQty = Math.Round(range / (docLine.LineTotal ?? 1) * docLine.Quantity.Value, RoundLengthQty, MidpointRounding.ToPositiveInfinity);
                                    //    }
                                    //    else
                                    //    {
                                    //        newQty = Math.Round(range / (docLine.LineTotal ?? 1) * docLine.Quantity.Value, 0, MidpointRounding.ToPositiveInfinity);
                                    //    }

                                    //    if (newQty <= 0)
                                    //    {
                                    //        continue;
                                    //    }

                                    //    if (docLine.Quantity.Value - newQty > 0)
                                    //    {
                                    //        DocumentLine newLine = docLine.Clone();
                                    //        newLine.Quantity = docLine.Quantity.Value - newQty;
                                    //        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                    //        newLine.UCheckPromo = "YY";
                                    //        lstNewLine.Add(newLine);
                                    //    }

                                    //    docLine.Quantity = newQty;
                                    //    totalPayable -= range;
                                    //}
                                    //else
                                    //{
                                    //    continue;
                                    //}
                                }

                                if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                                {
                                    double ItemPrice = docLine.UnitPrice ?? 0;
                                    double FixedPrice = promoGetLine.GetValue ?? 0;
                                    if (FixedPrice < ItemPrice)
                                    {
                                        double maxQtyLine = maxQtyByBill;
                                        if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                        {
                                            maxQtyLine = promoGetLine.MaxQtyDis ?? 0;
                                        }
                                        double maxQtyDis = Math.Min(MultiRate, Math.Min(maxQtyLine, maxQtyByBill));

                                        //double maxQtyDis = Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate);
                                        //double maxQtyDis = Math.Min(Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate), maxQtyByBill);
                                        if (docLine.Quantity > maxQtyDis && (arlMultiRate.Count > 0 || (promoGetLine.MaxQtyDis ?? 0) > 0))
                                        {
                                            double newQty = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            var newDocLine = lstNewLine.Where(x => x.ItemCode == docLine.ItemCode && x.UoMCode == docLine.UoMCode).FirstOrDefault();
                                            if (newDocLine != null)
                                            {
                                                lstNewLine[lstNewLine.IndexOf(newDocLine)].Quantity += newQty;
                                            }
                                            else
                                            {
                                                DocumentLine newLine = docLine.Clone();
                                                newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                                                newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                                newLine.UCheckPromo = "YY";
                                                lstNewLine.Add(newLine);
                                            }

                                            docLine.Quantity = maxQtyDis;
                                            docLine.LineTotal = GetLineTotal(docLine);
                                        }

                                        maxQtyByBill -= (docLine.Quantity ?? 0);

                                        docLine.UCheckPromo = "YY";
                                        //double ItemPrice = docLine.UnitPrice ?? 0;
                                        //double FixedPrice = promoGetLine.GetValue ?? 0;
                                        var DisPrcnt = (ItemPrice - FixedPrice) * 100 / ItemPrice;
                                        if (DisPrcnt < 0)
                                        {
                                            DisPrcnt = 0;
                                        }

                                        docLine.DiscountPercent = DisPrcnt;
                                        docLine.PromoType = PromoValueType.FixedPrice;

                                        getValue = 0;

                                        docLine.UPromoCode += promotion.PromoId + ", ";
                                        docLine.UPromoName = promotion.PromoName;
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                }
                                else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                                {
                                    double maxQtyLine = maxQtyByBill;
                                    if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                    {
                                        maxQtyLine = promoGetLine.MaxQtyDis ?? 0;
                                    }
                                    double maxQtyDis = Math.Min(MultiRate, Math.Min(maxQtyLine, maxQtyByBill));

                                    //double maxQtyDis = Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate);
                                    //double maxQtyDis = Math.Min(Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate), maxQtyByBill);
                                    if (docLine.Quantity > maxQtyDis && (arlMultiRate.Count > 0 || (promoGetLine.MaxQtyDis ?? 0) > 0))
                                    {
                                        DocumentLine newLine = docLine.Clone();
                                        //newLine.Quantity -= maxQtyDis;
                                        newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                                        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                        newLine.UCheckPromo = "YY";
                                        lstNewLine.Add(newLine);

                                        docLine.Quantity = maxQtyDis;
                                        docLine.LineTotal = GetLineTotal(docLine);
                                    }

                                    maxQtyByBill -= (docLine.Quantity ?? 0);

                                    docLine.UCheckPromo = "YY";
                                    double maxDiscount = promoGetLine.MaxAmtDis ?? 0;
                                    double maxPercent = maxDiscount / docLine.LineTotal.Value * 100;
                                    if (maxPercent <= 0 || double.IsNaN(maxPercent))
                                    {
                                        maxPercent = promoGetLine.GetValue ?? 0;
                                    }

                                    if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                    {
                                        docLine.DiscountPercent = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                                    }
                                    else
                                    {
                                        double firstPrnct = docLine.DiscountPercent ?? 0;
                                        double newPrnct = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                                        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                        docLine.DiscountPercent = lastPrcnt;
                                    }
                                    docLine.PromoType = PromoValueType.DiscountPercent;
                                    getValue = 0;
                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                                else if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                                {
                                    double maxQtyLine = maxQtyByBill;
                                    if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                    {
                                        maxQtyLine = promoGetLine.MaxQtyDis ?? 0;
                                    }
                                    double maxQtyDis = Math.Min(MultiRate, Math.Min(maxQtyLine, maxQtyByBill));

                                    //double maxQtyDis = Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate);
                                    //double maxQtyDis = Math.Min(Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate), maxQtyByBill);
                                    if ((arlMultiRate.Count > 0 || (promoGetLine.MaxQtyDis ?? 0) > 0) && docLine.Quantity > maxQtyDis)
                                    {
                                        DocumentLine newLine = docLine.Clone();
                                        //newLine.Quantity -= maxQtyDis;
                                        newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                                        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                        newLine.UCheckPromo = "YY";
                                        lstNewLine.Add(newLine);

                                        docLine.Quantity = maxQtyDis;
                                        docLine.LineTotal = GetLineTotal(docLine);
                                    }

                                    maxQtyByBill -= (docLine.Quantity ?? 0);

                                    docLine.UCheckPromo = "YY";

                                    double PromoAmt = promoGetLine.GetValue.Value * Math.Min(MultiRate, docLine.Quantity ?? 0);
                                    double disPrcnt = docLine.DiscountPercent ?? 0;
                                    double lineTotal = docLine.LineTotal ?? 0;
                                    double newAmt = lineTotal * (100 - disPrcnt) / 100;
                                    //double newAmt = Math.Round(lineTotal * (100 - disPrcnt) / 100, RoundLength);
                                    if (lineTotal <= 0)
                                        PromoAmt = 0;

                                    double ItemDiscount = Math.Min(100, 100 - (newAmt - PromoAmt) / lineTotal * 100);

                                    docLine.DiscountPercent = ItemDiscount;
                                    docLine.PromoType = PromoValueType.DiscountAmount;
                                    getValue = 0;
                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                                else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                                {
                                    //double maxQty = promoGetLine.GetValue.Value * MultiRate;
                                    double maxQty = Math.Round(Math.Min(getValue * MultiRate, maxQtyByBill), RoundLengthQty, MidpointRounding.AwayFromZero);
                                    if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                    {
                                        maxQty = Math.Min(maxQty, promoGetLine.MaxQtyDis.Value);
                                    }

                                    if (docLine.Quantity > maxQty)
                                    {
                                        DocumentLine newLine = docLine.Clone();
                                        //newLine.Quantity -= maxQty;
                                        newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                        newLine.UCheckPromo = "YY";
                                        lstNewLine.Add(newLine);

                                        docLine.Quantity = maxQty;
                                        docLine.LineTotal = GetLineTotal(docLine);

                                        getValue -= (newLine.Quantity ?? 0);
                                    }
                                    getValue -= (docLine.Quantity.Value / MultiRate);
                                    maxQtyByBill -= (docLine.Quantity ?? 0);

                                    docLine.UCheckPromo = "YY";
                                    docLine.DiscountPercent = 100;
                                    docLine.PromoType = PromoValueType.FixedQuantity;
                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.BarCode)
                        {
                            if (promoGetLine.LineCode == docLine.BarCode)
                            {
                                //  new
                                if ((promotion.TotalBuyFrom ?? 0) > 0)
                                {
                                    if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp mặt hàng là bàn cân, qty số thập phân
                                    {
                                        double totalPayableCheck = totalPayable - (docLine.LineTotal ?? 0);
                                        if (CheckTotalValue(promotion, totalPayableCheck))
                                        {
                                            totalPayable = totalPayableCheck;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else // trường hợp mặt hàng thường, có qty số nguyên
                                    {
                                        int i = (int)docLine.Quantity.Value;
                                        while (i > 0)
                                        {
                                            double totalPayableCheck = totalPayable - ((docLine.UnitPrice ?? 0) * i);
                                            if (CheckTotalValue(promotion, totalPayableCheck))
                                            {
                                                if (docLine.Quantity.Value - i > 0)
                                                {
                                                    DocumentLine newLine = docLine.Clone();
                                                    newLine.Quantity = docLine.Quantity.Value - i;
                                                    newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                                    newLine.UCheckPromo = "YY";
                                                    lstNewLine.Add(newLine);
                                                }
                                                docLine.Quantity = i;
                                                docLine.LineTotal = GetLineTotal(docLine);
                                                totalPayable = totalPayableCheck;
                                                break;
                                            }
                                            i--;
                                        }

                                        if (i == 0)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                if (promoGetLine.ValueType == PromoValueType.FixedPrice)
                                {
                                    double ItemPrice = docLine.UnitPrice ?? 0;
                                    double FixedPrice = promoGetLine.GetValue ?? 0;
                                    if (FixedPrice < ItemPrice)
                                    {
                                        double maxQtyLine = maxQtyByBill;
                                        if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                        {
                                            maxQtyLine = promoGetLine.MaxQtyDis ?? 0;
                                        }
                                        double maxQtyDis = Math.Min(MultiRate, Math.Min(maxQtyLine, maxQtyByBill));

                                        //double maxQtyDis = Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate);
                                        //double maxQtyDis = Math.Min(Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate), maxQtyByBill);
                                        if (docLine.Quantity > maxQtyDis && (arlMultiRate.Count > 0 || (promoGetLine.MaxQtyDis ?? 0) > 0))
                                        {
                                            DocumentLine newLine = docLine.Clone();
                                            //newLine.Quantity -= maxQtyDis;
                                            newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                                            newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                            newLine.UCheckPromo = "YY";
                                            lstNewLine.Add(newLine);

                                            docLine.Quantity = maxQtyDis;
                                            docLine.LineTotal = GetLineTotal(docLine);
                                        }

                                        maxQtyByBill -= (docLine.Quantity ?? 0);

                                        docLine.UCheckPromo = "YY";
                                        //double ItemPrice = docLine.UnitPrice ?? 0;
                                        //double FixedPrice = promoGetLine.GetValue ?? 0;
                                        var DisPrcnt = (ItemPrice - FixedPrice) * 100 / ItemPrice;
                                        if (DisPrcnt < 0)
                                        {
                                            DisPrcnt = 0;
                                        }

                                        docLine.DiscountPercent = DisPrcnt;
                                        docLine.PromoType = PromoValueType.FixedPrice;

                                        getValue = 0;

                                        docLine.UPromoCode += promotion.PromoId + ", ";
                                        docLine.UPromoName = promotion.PromoName;
                                        if (!string.IsNullOrEmpty(schemaId))
                                        {
                                            docLine.USchemaCode = schemaId;
                                            docLine.USchemaName = schemaName;
                                        }
                                        if (!string.IsNullOrEmpty(voucherNum))
                                        {
                                            docLine.RefTransId = voucherNum;
                                            docLine.ApplyType = "CRM";
                                        }
                                    }
                                }
                                else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
                                {
                                    double maxQtyLine = maxQtyByBill;
                                    if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                    {
                                        maxQtyLine = promoGetLine.MaxQtyDis ?? 0;
                                    }
                                    double maxQtyDis = Math.Min(MultiRate, Math.Min(maxQtyLine, maxQtyByBill));

                                    //double maxQtyDis = Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate);
                                    //double maxQtyDis = Math.Min(Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate), maxQtyByBill);
                                    if (docLine.Quantity > maxQtyDis && (arlMultiRate.Count > 0 || (promoGetLine.MaxQtyDis ?? 0) > 0))
                                    {
                                        DocumentLine newLine = docLine.Clone();
                                        //newLine.Quantity -= maxQtyDis;
                                        newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                                        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                        newLine.UCheckPromo = "YY";
                                        lstNewLine.Add(newLine);

                                        docLine.Quantity = maxQtyDis;
                                        docLine.LineTotal = GetLineTotal(docLine);
                                    }

                                    maxQtyByBill -= (docLine.Quantity ?? 0);

                                    docLine.UCheckPromo = "YY";
                                    double maxDiscount = promoGetLine.MaxAmtDis ?? 0;
                                    double maxPercent = maxDiscount / docLine.LineTotal.Value * 100;
                                    if (maxPercent <= 0 || double.IsNaN(maxPercent))
                                    {
                                        maxPercent = promoGetLine.GetValue ?? 0;
                                    }

                                    if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                                    {
                                        docLine.DiscountPercent = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                                    }
                                    else
                                    {
                                        double firstPrnct = docLine.DiscountPercent ?? 0;
                                        double newPrnct = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                                        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                        docLine.DiscountPercent = lastPrcnt;
                                    }
                                    docLine.PromoType = PromoValueType.DiscountPercent;

                                    getValue = 0;

                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                                else if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
                                {
                                    double maxQtyLine = maxQtyByBill;
                                    if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                    {
                                        maxQtyLine = promoGetLine.MaxQtyDis ?? 0;
                                    }
                                    double maxQtyDis = Math.Min(MultiRate, Math.Min(maxQtyLine, maxQtyByBill));

                                    //double maxQtyDis = Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate);
                                    //double maxQtyDis = Math.Min(Math.Min(MultiRate, promoGetLine.MaxQtyDis ?? MultiRate), maxQtyByBill);
                                    if ((arlMultiRate.Count > 0 || (promoGetLine.MaxQtyDis ?? 0) > 0) && docLine.Quantity > maxQtyDis)
                                    {
                                        DocumentLine newLine = docLine.Clone();
                                        //newLine.Quantity -= maxQtyDis;
                                        newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                                        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                        newLine.UCheckPromo = "YY";
                                        lstNewLine.Add(newLine);

                                        docLine.Quantity = maxQtyDis;
                                        docLine.LineTotal = GetLineTotal(docLine);
                                    }

                                    maxQtyByBill -= (docLine.Quantity ?? 0);

                                    docLine.UCheckPromo = "YY";
                                    double PromoAmt = promoGetLine.GetValue.Value * Math.Min(MultiRate, docLine.Quantity ?? 0);
                                    double disPrcnt = docLine.DiscountPercent ?? 0;
                                    double lineTotal = docLine.LineTotal ?? 0;
                                    double newAmt = lineTotal * (100 - disPrcnt) / 100;
                                    //double newAmt = Math.Round(lineTotal * (100 - disPrcnt) / 100, RoundLength);
                                    if (lineTotal <= 0)
                                        PromoAmt = 0;

                                    double ItemDiscount = Math.Min(100, 100 - (newAmt - PromoAmt) / lineTotal * 100);

                                    docLine.DiscountPercent = ItemDiscount;
                                    docLine.PromoType = PromoValueType.DiscountAmount;

                                    getValue = 0;

                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                                else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
                                {
                                    //double maxQty = promoGetLine.GetValue.Value * MultiRate;
                                    double maxQty = Math.Round(Math.Min(getValue * MultiRate, maxQtyByBill), RoundLengthQty, MidpointRounding.AwayFromZero);
                                    if ((promoGetLine.MaxQtyDis ?? 0) > 0)
                                    {
                                        maxQty = Math.Min(maxQty, promoGetLine.MaxQtyDis.Value);
                                    }

                                    if (docLine.Quantity > maxQty)
                                    {
                                        DocumentLine newLine = docLine.Clone();
                                        //newLine.Quantity -= maxQty;
                                        newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                        newLine.UCheckPromo = "YY";
                                        lstNewLine.Add(newLine);

                                        docLine.Quantity = maxQty;
                                        docLine.LineTotal = GetLineTotal(docLine);

                                        getValue -= (newLine.Quantity ?? 0);
                                    }
                                    getValue -= (docLine.Quantity.Value / MultiRate);

                                    maxQtyByBill -= (docLine.Quantity ?? 0);

                                    docLine.UCheckPromo = "YY";
                                    docLine.DiscountPercent = 100;
                                    docLine.PromoType = PromoValueType.FixedQuantity;

                                    docLine.UPromoCode += promotion.PromoId + ", ";
                                    docLine.UPromoName = promotion.PromoName;
                                    if (!string.IsNullOrEmpty(schemaId))
                                    {
                                        docLine.USchemaCode = schemaId;
                                        docLine.USchemaName = schemaName;
                                    }
                                    if (!string.IsNullOrEmpty(voucherNum))
                                    {
                                        docLine.RefTransId = voucherNum;
                                        docLine.ApplyType = "CRM";
                                    }
                                }
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.ItemGroup)
                        {
                            if (docLine.ItemGroup == promoGetLine.LineCode && (string.IsNullOrEmpty(promoGetLine.LineUom) || docLine.UoMCode == promoGetLine.LineUom))
                            {
                                //  new
                                if ((promotion.TotalBuyFrom ?? 0) > 0)
                                {
                                    if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp mặt hàng là bàn cân, qty số thập phân
                                    {
                                        double totalPayableCheck = totalPayable - (docLine.LineTotal ?? 0);
                                        if (CheckTotalValue(promotion, totalPayableCheck))
                                        {
                                            totalPayable = totalPayableCheck;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else // trường hợp mặt hàng thường, có qty số nguyên
                                    {
                                        int i = (int)docLine.Quantity.Value;
                                        while (i > 0)
                                        {
                                            double totalPayableCheck = totalPayable - ((docLine.UnitPrice ?? 0) * i);
                                            if (CheckTotalValue(promotion, totalPayableCheck))
                                            {
                                                if (docLine.Quantity.Value - i > 0)
                                                {
                                                    DocumentLine newLine = docLine.Clone();
                                                    newLine.Quantity = docLine.Quantity.Value - i;
                                                    newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                                    newLine.UCheckPromo = "YY";
                                                    lstNewLine.Add(newLine);
                                                }
                                                docLine.Quantity = i;
                                                docLine.LineTotal = GetLineTotal(docLine);
                                                totalPayable = totalPayableCheck;
                                                break;
                                            }
                                            i--;
                                        }

                                        if (i == 0)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                groupAmount += (docLine.LineTotal ?? 0);
                                groupQuantity += (docLine.Quantity ?? 0);
                                docLine.UCheckPromo = promoGetLine.LineCode;
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.Collection)
                        {
                            if (docLine.UCollection == promoGetLine.LineCode && (string.IsNullOrEmpty(promoGetLine.LineUom) || docLine.UoMCode == promoGetLine.LineUom))
                            {
                                //  new
                                if ((promotion.TotalBuyFrom ?? 0) > 0)
                                {
                                    if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp mặt hàng là bàn cân, qty số thập phân
                                    {
                                        double totalPayableCheck = totalPayable - (docLine.LineTotal ?? 0);
                                        if (CheckTotalValue(promotion, totalPayableCheck))
                                        {
                                            totalPayable = totalPayableCheck;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else // trường hợp mặt hàng thường, có qty số nguyên
                                    {
                                        int i = (int)docLine.Quantity.Value;
                                        while (i > 0)
                                        {
                                            double totalPayableCheck = totalPayable - ((docLine.UnitPrice ?? 0) * i);
                                            if (CheckTotalValue(promotion, totalPayableCheck))
                                            {
                                                if (docLine.Quantity.Value - i > 0)
                                                {
                                                    DocumentLine newLine = docLine.Clone();
                                                    newLine.Quantity = docLine.Quantity.Value - i;
                                                    newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                                    newLine.UCheckPromo = "YY";
                                                    lstNewLine.Add(newLine);
                                                }
                                                docLine.Quantity = i;
                                                docLine.LineTotal = GetLineTotal(docLine);
                                                totalPayable = totalPayableCheck;
                                                break;
                                            }
                                            i--;
                                        }

                                        if (i == 0)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                collectQuantity += (docLine.Quantity ?? 0);
                                collectAmount += (docLine.LineTotal ?? 0);
                                docLine.UCheckPromo = promoGetLine.LineCode;
                            }
                        }
                        else if (promoGetLine.LineType == PromoLineType.OneTimeGroup)
                        {
                            if (((promoGetLine.MaxQtyDis ?? 0) > 0 && promoGetLine.MaxQtyDis.Value < otQuantity)
                                || ((promoGetLine.MaxAmtDis ?? 0) > 0 && promoGetLine.MaxAmtDis < otAmount))
                            {
                                continue;
                            }

                            if (promoGetLine.Lines != null && promoGetLine.Lines.Count > 0)
                            {
                                var lstGroupItemCode = promoGetLine.Lines.Where(x => x.LineType == PromoLineType.ItemCode && x.LineCode == docLine.ItemCode && x.LineUom == docLine.UoMCode && !x.IsCount);
                                if (lstGroupItemCode.Any())
                                {
                                    SPromoOTGroup otGet = lstGroupItemCode.FirstOrDefault();
                                    //  new
                                    if ((promotion.TotalBuyFrom ?? 0) > 0)
                                    {
                                        if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp mặt hàng là bàn cân, qty số thập phân
                                        {
                                            double totalPayableCheck = totalPayable - (docLine.LineTotal ?? 0);
                                            if (CheckTotalValue(promotion, totalPayableCheck))
                                            {
                                                totalPayable = totalPayableCheck;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else // trường hợp mặt hàng thường, có qty số nguyên
                                        {
                                            int i = (int)docLine.Quantity.Value;
                                            while (i > 0)
                                            {
                                                double totalPayableCheck = totalPayable - ((docLine.UnitPrice ?? 0) * i);
                                                if (CheckTotalValue(promotion, totalPayableCheck))
                                                {
                                                    if (docLine.Quantity.Value - i > 0)
                                                    {
                                                        DocumentLine newLine = docLine.Clone();
                                                        newLine.Quantity = docLine.Quantity.Value - i;
                                                        newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                                        newLine.UCheckPromo = "YY";
                                                        newLine.LineTotal = GetLineTotal(newLine);
                                                        lstNewLine.Add(newLine);
                                                    }
                                                    docLine.Quantity = i;
                                                    docLine.LineTotal = GetLineTotal(docLine);
                                                    totalPayable = totalPayableCheck;
                                                    break;
                                                }
                                                i--;
                                            }

                                            if (i == 0)
                                            {
                                                continue;
                                            }
                                        }
                                    }

                                    otQuantity += (docLine.Quantity ?? 0);
                                    otAmount += (docLine.LineTotal ?? 0);
                                    docLine.UCheckPromo = otGet.GroupID;
                                    promoGetLine.Lines[promoGetLine.Lines.IndexOf(otGet)].IsCount = true;
                                }
                                else
                                {
                                    var lstGroupBarCode = promoGetLine.Lines.Where(x => x.LineType == PromoLineType.BarCode && x.LineCode == docLine.BarCode && !x.IsCount);
                                    if (lstGroupBarCode.Any())
                                    {
                                        SPromoOTGroup otGet = lstGroupItemCode.FirstOrDefault();
                                        //  new
                                        if ((promotion.TotalBuyFrom ?? 0) > 0)
                                        {
                                            if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp mặt hàng là bàn cân, qty số thập phân
                                            {
                                                double totalPayableCheck = totalPayable - (docLine.LineTotal ?? 0);
                                                if (CheckTotalValue(promotion, totalPayableCheck))
                                                {
                                                    totalPayable = totalPayableCheck;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            else // trường hợp mặt hàng thường, có qty số nguyên
                                            {
                                                int i = (int)docLine.Quantity.Value;
                                                while (i > 0)
                                                {
                                                    double totalPayableCheck = totalPayable - ((docLine.UnitPrice ?? 0) * i);
                                                    if (CheckTotalValue(promotion, totalPayableCheck))
                                                    {
                                                        if (docLine.Quantity.Value - i > 0)
                                                        {
                                                            DocumentLine newLine = docLine.Clone();
                                                            newLine.Quantity = docLine.Quantity.Value - i;
                                                            newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                                            newLine.UCheckPromo = "YY";
                                                            newLine.LineTotal = GetLineTotal(newLine);
                                                            lstNewLine.Add(newLine);
                                                        }
                                                        docLine.Quantity = i;
                                                        docLine.LineTotal = GetLineTotal(docLine);
                                                        totalPayable = totalPayableCheck;
                                                        break;
                                                    }
                                                    i--;
                                                }

                                                if (i == 0)
                                                {
                                                    continue;
                                                }
                                            }
                                        }

                                        otQuantity += (docLine.Quantity ?? 0);
                                        otAmount += (docLine.LineTotal ?? 0);
                                        docLine.UCheckPromo = otGet.GroupID;
                                        promoGetLine.Lines[promoGetLine.Lines.IndexOf(otGet)].IsCount = true;
                                    }
                                }

                                //foreach (SPromoOTGroup item in promoGetLine.Lines)
                                //{
                                //    if (item.IsCount)
                                //    {
                                //        //  kiểm tra nếu nó đã được đưa vào tính toán thì không tính nữa
                                //        continue;
                                //    }
                                //    if ((item.LineType == PromoLineType.ItemCode && docLine.ItemCode == item.LineCode && docLine.UoMCode == item.LineUom)
                                //                || (item.LineType == PromoLineType.BarCode && docLine.BarCode == item.LineCode))
                                //    {
                                //        //  new
                                //        if ((promotion.TotalBuyFrom ?? 0) > 0)
                                //        {
                                //            if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))   // trường hợp mặt hàng là bàn cân, qty số thập phân
                                //            {
                                //                double totalPayableCheck = totalPayable - (docLine.LineTotal ?? 0);
                                //                if (CheckTotalValue(promotion, totalPayableCheck))
                                //                {
                                //                    totalPayable = totalPayableCheck;
                                //                }
                                //                else
                                //                {
                                //                    continue;
                                //                }
                                //            }
                                //            else // trường hợp mặt hàng thường, có qty số nguyên
                                //            {
                                //                int i = (int)docLine.Quantity.Value;
                                //                while (i > 0)
                                //                {
                                //                    double totalPayableCheck = totalPayable - ((docLine.UnitPrice ?? 0) * i);
                                //                    if (CheckTotalValue(promotion, totalPayableCheck))
                                //                    {
                                //                        if (docLine.Quantity.Value - i > 0)
                                //                        {
                                //                            DocumentLine newLine = docLine.Clone();
                                //                            newLine.Quantity = docLine.Quantity.Value - i;
                                //                            newLine.LineNum = doc.DocumentLines.Count + lstNewLine.Count;
                                //                            newLine.UCheckPromo = "YY";
                                //                            newLine.LineTotal = GetLineTotal(newLine);
                                //                            lstNewLine.Add(newLine);
                                //                        }
                                //                        docLine.Quantity = i;
                                //                        docLine.LineTotal = GetLineTotal(docLine);
                                //                        totalPayable = totalPayableCheck;
                                //                        break;
                                //                    }
                                //                    i--;
                                //                }

                                //                if (i == 0)
                                //                {
                                //                    continue;
                                //                }
                                //            }
                                //        }

                                //        otQuantity += (docLine.Quantity ?? 0);
                                //        otAmount += (docLine.LineTotal ?? 0);

                                //        docLine.UCheckPromo = item.GroupID;
                                //        item.IsCount = true;
                                //        break;
                                //    }
                                //}
                            }
                        }

                        if (maxQtyByBill <= 0)
                        {
                            break;
                        }
                    }

                    if (promoGetLine.LineType == PromoLineType.ItemGroup)
                    {
                        double maxQtyDis = promoGetLine.MaxQtyDis ?? groupQuantity;
                        double maxAmtDis = promoGetLine.MaxAmtDis ?? groupAmount;

                        if (maxQtyDis == 0)
                        {
                            maxQtyDis = groupQuantity;
                        }

                        if (maxAmtDis == 0)
                        {
                            maxAmtDis = groupAmount;
                        }

                        List<DocumentLine> lstLineOrderBy = doc.DocumentLines.Where(l => l.ItemGroup == promoGetLine.LineCode && l.UCheckPromo == promoGetLine.LineCode).ToList();
                        foreach (DocumentLine doclineCheck in lstLineOrderBy)
                        {
                            if (groupAmount <= 0 || maxQtyDis <= 0 || maxAmtDis <= 0)
                            {
                                break;
                            }
                            //double maxQty = Math.Min(maxQtyDis, doclineCheck.Quantity.Value);
                            double maxQty = Math.Min(Math.Min(maxQtyDis, doclineCheck.Quantity.Value), maxQtyByBill);
                            DocumentLine line = ApplyPromoForMixAndMatch(promoGetLine, doclineCheck, doc.DocumentLines.Count + lstNewLine.Count, promotion.PromoId, promotion.PromoName, schemaId, schemaName, voucherNum, maxQty, maxAmtDis, MultiRate, out DocumentLine newLine);

                            groupAmount -= (doclineCheck.LineTotal ?? 0);
                            maxQtyDis -= (line.Quantity ?? 0);
                            maxAmtDis -= (line.UnitPrice ?? 0) * (line.Quantity ?? 0) * (line.DiscountPercent ?? 0) / 100;
                            maxQtyByBill -= (line.Quantity ?? 0);

                            for (int i = 0; i < doc.DocumentLines.Count; i++)
                            {
                                DocumentLine docLine = doc.DocumentLines[i];
                                if (docLine.LineNum == line.LineNum && docLine.ItemCode == line.ItemCode)
                                {
                                    doc.DocumentLines[i] = line;
                                    break;
                                }
                            }
                            if (newLine != null)
                            {
                                lstNewLine.Add(newLine);
                            }
                        }
                    }
                    else if (promoGetLine.LineType == PromoLineType.Collection)
                    {
                        double maxQtyDis = promoGetLine.MaxQtyDis ?? collectQuantity;
                        double maxAmtDis = promoGetLine.MaxAmtDis ?? collectAmount;

                        if (maxQtyDis == 0)
                        {
                            maxQtyDis = collectQuantity;
                        }

                        if (maxAmtDis == 0)
                        {
                            maxAmtDis = collectAmount;
                        }

                        List<DocumentLine> lstLineOrderBy = doc.DocumentLines.Where(l => l.UCollection == promoGetLine.LineCode && l.UCheckPromo == promoGetLine.LineCode).ToList();
                        foreach (DocumentLine doclineCheck in lstLineOrderBy)
                        {
                            if (collectAmount <= 0 || maxQtyDis <= 0 || maxAmtDis <= 0)
                            {
                                break;
                            }
                            //double maxQty = Math.Min(maxQtyDis, doclineCheck.Quantity.Value);
                            double maxQty = Math.Min(Math.Min(maxQtyDis, doclineCheck.Quantity.Value), maxQtyByBill);
                            DocumentLine line = ApplyPromoForMixAndMatch(promoGetLine, doclineCheck, doc.DocumentLines.Count + lstNewLine.Count, promotion.PromoId, promotion.PromoName, schemaId, schemaName, voucherNum, maxQty, maxAmtDis, MultiRate, out DocumentLine newLine);

                            collectAmount -= (doclineCheck.LineTotal ?? 0);
                            maxQtyDis -= (line.Quantity ?? 0);
                            maxAmtDis -= (line.UnitPrice ?? 0) * (line.Quantity ?? 0) * (line.DiscountPercent ?? 0) / 100;
                            maxQtyByBill -= (line.Quantity ?? 0);

                            for (int i = 0; i < doc.DocumentLines.Count; i++)
                            {
                                DocumentLine docLine = doc.DocumentLines[i];
                                if (docLine.LineNum == line.LineNum && docLine.ItemCode == line.ItemCode)
                                {
                                    doc.DocumentLines[i] = line;
                                    break;
                                }
                            }
                            if (newLine != null)
                            {
                                lstNewLine.Add(newLine);
                            }
                        }
                    }
                    else if (promoGetLine.LineType == PromoLineType.OneTimeGroup)
                    {
                        double maxQtyDis = promoGetLine.MaxQtyDis ?? otQuantity;
                        double maxAmtDis = promoGetLine.MaxAmtDis ?? otAmount;

                        if (maxQtyDis == 0)
                        {
                            maxQtyDis = otQuantity;
                        }

                        if (maxAmtDis == 0)
                        {
                            maxAmtDis = otAmount;
                        }

                        List<DocumentLine> lstLineOrderBy = doc.DocumentLines.OrderBy(l => l.Quantity.HasValue ? l.Quantity.Value : 0).Where(l => l.UCheckPromo == promoGetLine.LineCode).ToList();
                        foreach (DocumentLine doclineCheck in lstLineOrderBy)
                        {
                            if (otAmount <= 0 || maxQtyDis <= 0 || maxAmtDis <= 0)
                            {
                                break;
                            }
                            //double maxQty = Math.Min(maxQtyDis, doclineCheck.Quantity.Value);
                            double maxQty = Math.Min(Math.Min(maxQtyDis, doclineCheck.Quantity.Value), maxQtyByBill);
                            DocumentLine line = ApplyPromoForMixAndMatch(promoGetLine, doclineCheck, doc.DocumentLines.Count + lstNewLine.Count, promotion.PromoId, promotion.PromoName, schemaId, schemaName, voucherNum, maxQty, maxAmtDis, MultiRate, out DocumentLine newLine);

                            otAmount -= (doclineCheck.LineTotal ?? 0);
                            maxQtyDis -= (line.Quantity ?? 0);
                            maxAmtDis -= (line.UnitPrice ?? 0) * (line.Quantity ?? 0) * (line.DiscountPercent ?? 0) / 100;
                            maxQtyByBill -= (line.Quantity ?? 0);

                            for (int i = 0; i < doc.DocumentLines.Count; i++)
                            {
                                DocumentLine docLine = doc.DocumentLines[i];
                                if (docLine.LineNum == line.LineNum && docLine.ItemCode == line.ItemCode)
                                {
                                    doc.DocumentLines[i] = line;
                                    break;
                                }
                            }
                            if (newLine != null)
                            {
                                lstNewLine.Add(newLine);
                            }
                        }
                    }

                    if (lstNewLine.Count > 0)
                    {
                        doc.DocumentLines.AddRange(lstNewLine);
                    }

                    if (maxQtyByBill <= 0)
                    {
                        break;
                    }
                }

                //Set_ReCalcGrid(ref doc);
                //Set_SumTotalAmt(ref doc);

                //string MaxGetValue = promotion.MaxTotalGetValue.ToString() == "" || Convert.ToDouble(promotion.MaxTotalGetValue.ToString()) <= 0 ? null : promotion.MaxTotalGetValue.ToString();
                //if (promotion.TotalGetType == PromoValueType.DiscountAmount)
                //{
                //    if ((promotion.MaxTotalGetValue ?? 0) > 0 && totalPayable > promotion.MaxTotalGetValue.Value)
                //    {
                //        doc.DiscountPercent = promotion.TotalGetValue.Value / promotion.MaxTotalGetValue.Value * 100;
                //    }
                //    else
                //    {
                //        doc.DiscountPercent = promotion.TotalGetValue.Value / totalPayable * 100;
                //    }
                //}
                //else if (promotion.TotalGetType == PromoValueType.DiscountPercent)
                //{
                //    if (doc.DiscountPercent == null || doc.DiscountPercent.Value == 0)
                //    {
                //        double tmpValue = totalPayable * promotion.TotalGetValue.Value / 100;
                //        if ((promotion.MaxTotalGetValue ?? 0) > 0 && tmpValue > promotion.MaxTotalGetValue.Value)
                //        {
                //            double newPercent = Convert.ToDouble(promotion.MaxTotalGetValue.Value) * 100 / totalPayable;
                //            doc.DiscountPercent = newPercent;
                //        }
                //        else
                //        {
                //            doc.DiscountPercent = promotion.TotalGetValue.Value;
                //        }
                //    }
                //    else if (promotion.IsCombine == "Y")
                //    {
                //        double firstPrnct = doc.DiscountPercent.Value;
                //        double newPrnct = promotion.TotalGetValue.Value;
                //        double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                //        doc.DiscountPercent = lastPrcnt;
                //    }
                //}
                //else if (promotion.TotalGetType == PromoValueType.FixedPrice)
                //{
                //    doc.DiscountPercent = (totalPayable - promotion.TotalGetValue.Value) / totalPayable * 100;
                //}

                double newPercent = 0;
                if (promotion.TotalGetType == PromoValueType.DiscountAmount)
                {
                    if ((promotion.MaxTotalGetValue ?? 0) > 0 && totalPayable > promotion.MaxTotalGetValue.Value)
                    {
                        newPercent = Math.Min(promotion.TotalGetValue.Value, promotion.MaxTotalGetValue.Value) / totalPayable * 100;
                        //newPercent = promotion.TotalGetValue.Value / promotion.MaxTotalGetValue.Value * 100;
                    }
                    else
                    {
                        newPercent = promotion.TotalGetValue.Value / totalPayable * 100;
                    }
                }
                else if (promotion.TotalGetType == PromoValueType.DiscountPercent)
                {
                    double tmpValue = totalPayable * promotion.TotalGetValue.Value / 100;
                    if ((promotion.MaxTotalGetValue ?? 0) > 0 && tmpValue > promotion.MaxTotalGetValue.Value)
                    {
                        newPercent = Convert.ToDouble(promotion.MaxTotalGetValue.Value) * 100 / totalPayable;
                    }
                    else
                    {
                        newPercent = promotion.TotalGetValue ?? 0;
                    }
                }
                else if (promotion.TotalGetType == PromoValueType.FixedPrice)
                {
                    newPercent = (totalPayable - promotion.TotalGetValue.Value) / totalPayable * 100;
                }

                newPercent = Math.Min(newPercent, 100);

                if (promotion.IsCombine == "Y" && !string.IsNullOrEmpty(schemaId) && !string.IsNullOrEmpty(allowChain) && allowChain != "Y")
                {
                    double firstPrnct = doc.DiscountPercent ?? 0;
                    double lastPrcnt = (firstPrnct + newPercent) - (firstPrnct * newPercent) / 100;
                    doc.DiscountPercent = lastPrcnt;

                    if (doc.DiscountPercent > 0)
                    {
                        doc.PromotionId += promotion.PromoId + ",";
                    }
                }
                else if (string.IsNullOrEmpty(schemaId) || (doc.DiscountPercent ?? 0) == 0)
                {
                    doc.DiscountPercent = newPercent;

                    if (doc.DiscountPercent > 0)
                    {
                        doc.PromotionId += promotion.PromoId + ",";
                    }
                }
            }

            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            return doc;
        }

        private DocumentLine ApplyPromoForMixAndMatch(SPromoGet promoGetLine, DocumentLine docLine, int lineNum, string promoId, string promoName, string schemaId, string schemaName, string voucherNum, double maxQtyDis, double maxDiscount, int multiRate, out DocumentLine newLine)
        {
            newLine = null;
            if (promoGetLine.ValueType == PromoValueType.FixedPrice)
            {
                double ItemPrice = docLine.UnitPrice ?? 0;
                double FixedPrice = promoGetLine.GetValue ?? 0;
                if (FixedPrice < ItemPrice)
                {
                    if (docLine.Quantity > maxQtyDis && maxQtyDis > 0)
                    {
                        newLine = docLine.Clone();
                        //newLine.Quantity -= maxQtyDis;
                        newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                        newLine.LineNum = lineNum;
                        newLine.UCheckPromo = "YY";

                        docLine.Quantity = maxQtyDis;
                        docLine.LineTotal = GetLineTotal(docLine);
                        docLine.UCheckPromo = "YY";
                    }

                    //double ItemPrice = docLine.UnitPrice ?? 0;
                    //double FixedPrice = promoGetLine.GetValue ?? 0;
                    var DisPrcnt = (ItemPrice - FixedPrice) * 100 / ItemPrice;
                    if (DisPrcnt < 0)
                    {
                        DisPrcnt = 0;
                    }

                    docLine.DiscountPercent = DisPrcnt;
                    docLine.PromoType = PromoValueType.FixedPrice;

                    docLine.UPromoCode += promoId + ", ";
                    docLine.UPromoName = promoName;
                    if (!string.IsNullOrEmpty(schemaId))
                    {
                        docLine.USchemaCode = schemaId;
                        docLine.USchemaName = schemaName;
                    }
                    if (!string.IsNullOrEmpty(voucherNum))
                    {
                        docLine.RefTransId = voucherNum;
                        docLine.ApplyType = "CRM";
                    }
                }
            }
            else if (promoGetLine.ValueType == PromoValueType.DiscountPercent)
            {
                if (docLine.Quantity > maxQtyDis && maxQtyDis > 0)
                {
                    newLine = docLine.Clone();
                    //newLine.Quantity -= maxQtyDis;
                    newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                    newLine.LineNum = lineNum;
                    newLine.UCheckPromo = "YY";

                    docLine.Quantity = maxQtyDis;
                    docLine.LineTotal = GetLineTotal(docLine);
                    docLine.UCheckPromo = "YY";
                }

                double maxPercent = maxDiscount / docLine.LineTotal.Value * 100;
                if (maxPercent <= 0 || double.IsNaN(maxPercent))
                {
                    maxPercent = promoGetLine.GetValue ?? 0;
                }

                if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                {
                    docLine.DiscountPercent = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                }
                else
                {
                    double firstPrnct = docLine.DiscountPercent ?? 0;
                    double newPrnct = Math.Min(promoGetLine.GetValue.Value, maxPercent);
                    double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                    docLine.DiscountPercent = lastPrcnt;
                }
                docLine.PromoType = PromoValueType.DiscountPercent;

                docLine.UPromoCode += promoId + ", ";
                docLine.UPromoName = promoName;
                if (!string.IsNullOrEmpty(schemaId))
                {
                    docLine.USchemaCode = schemaId;
                    docLine.USchemaName = schemaName;
                }
                if (!string.IsNullOrEmpty(voucherNum))
                {
                    docLine.RefTransId = voucherNum;
                    docLine.ApplyType = "CRM";
                }
            }
            else if (promoGetLine.ValueType == PromoValueType.DiscountAmount)
            {
                if (docLine.Quantity > maxQtyDis && maxQtyDis > 0)
                {
                    newLine = docLine.Clone();
                    //newLine.Quantity -= maxQtyDis;
                    newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQtyDis, RoundLengthQty, MidpointRounding.AwayFromZero);
                    newLine.LineNum = lineNum;
                    newLine.UCheckPromo = "YY";

                    docLine.Quantity = maxQtyDis;
                    docLine.LineTotal = GetLineTotal(docLine);
                    docLine.UCheckPromo = "YY";
                }

                double maxMuitiRate = Math.Min(multiRate, docLine.Quantity ?? 0);
                double PromoAmt = promoGetLine.GetValue.Value * maxMuitiRate;
                if (maxDiscount > 0)
                {
                    PromoAmt = Math.Min(promoGetLine.GetValue.Value * maxMuitiRate, maxDiscount);
                }
                double disPrcnt = docLine.DiscountPercent ?? 0;
                double lineTotal = docLine.LineTotal ?? 0;
                double newAmt = lineTotal * (100 - disPrcnt) / 100;
                //double newAmt = Math.Round(lineTotal * (100 - disPrcnt) / 100, RoundLength);
                if (lineTotal <= 0)
                    PromoAmt = 0;

                double ItemDiscount = Math.Min(100, 100 - (newAmt - PromoAmt) / lineTotal * 100);

                docLine.DiscountPercent = ItemDiscount;
                docLine.PromoType = PromoValueType.DiscountAmount;

                docLine.UPromoCode += promoId + ", ";
                docLine.UPromoName = promoName;
                if (!string.IsNullOrEmpty(schemaId))
                {
                    docLine.USchemaCode = schemaId;
                    docLine.USchemaName = schemaName;
                }
                if (!string.IsNullOrEmpty(voucherNum))
                {
                    docLine.RefTransId = voucherNum;
                    docLine.ApplyType = "CRM";
                }
            }
            else if (promoGetLine.ValueType == PromoValueType.FixedQuantity)
            {
                if ((promoGetLine.GetValue ?? 0) > 0 && docLine.Quantity > promoGetLine.GetValue * multiRate)
                {
                    double fixQty = Math.Min(maxQtyDis, Math.Round(promoGetLine.GetValue.Value * multiRate, RoundLengthQty, MidpointRounding.AwayFromZero));

                    newLine = docLine.Clone();
                    //newLine.Quantity -= fixQty;
                    newLine.Quantity = Math.Round(docLine.Quantity.Value - fixQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                    newLine.LineNum = lineNum;
                    newLine.UCheckPromo = "YY";

                    docLine.Quantity = fixQty;
                    docLine.LineTotal = GetLineTotal(docLine);
                    docLine.UCheckPromo = "YY";
                }

                docLine.DiscountPercent = 100;
                docLine.PromoType = PromoValueType.FixedQuantity;

                docLine.UPromoCode += promoId + ", ";
                docLine.UPromoName = promoName;
                if (!string.IsNullOrEmpty(schemaId))
                {
                    docLine.USchemaCode = schemaId;
                    docLine.USchemaName = schemaName;
                }
                if (!string.IsNullOrEmpty(voucherNum))
                {
                    docLine.RefTransId = voucherNum;
                    docLine.ApplyType = "CRM";
                }
            }

            return docLine;
        }

        private void Set_ReCalcGrid(ref Document doc)
        {
            int lineNum = 1;
            foreach (DocumentLine docLine in doc.DocumentLines)
            {
                var taxRate = docLine.TaxPercentagePerRow ?? 0;
                var ItemPrice = docLine.UnitPrice ?? 0;
                var ItemQty = docLine.Quantity ?? 0;
                var ItemDisRate = docLine.DiscountPercent ?? 0;
                // Lập đã xin phép Minh ngày 2023/2/2 Nếu là weighScale thì làm trong Price, LineTotal
                var total = ItemPrice * ItemQty;
                if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))
                {
                    total = Math.Round(ItemPrice * ItemQty, doc.RoundingDigit, MidpointRounding.AwayFromZero);
                }  
                var totalWithDiscount = total - Math.Round(total * ItemDisRate / 100, doc.RoundingDigit, MidpointRounding.AwayFromZero);
                if (docLine.PromoType == PromoValueType.BonusAmount || docLine.PromoType == PromoValueType.BonusPercent)
                {
                    totalWithDiscount = total;
                }
                var totalWithTax = totalWithDiscount + totalWithDiscount * taxRate / 100;

                docLine.LineTotal = totalWithTax;
                //docLine.LineTotal = Math.Round(totalWithTax, RoundLength);

                docLine.UCheckPromo = string.Empty;
                docLine.LineNum = lineNum++;
            }
        }

        private void Set_SumTotalAmt(ref Document doc)
        {
            //try
            //{
            var TotalQty = 0.0d;
            var TotalLine = 0.0d;
            var TotalLineDis = 0.0d;
            double totalLineTax = 0.0d;
            double totalPayable = 0.0d;
            //double TotalDepositAmnt = 0.0d;
            double TotalReturnAmnt = 0.0d;
            //double TotalAmtFur = 0.0d;
            //double TotalSumReceipt = 0.0d;
            //double totalWeight = 0;
            if (doc.DocumentLines.Count > 0)
            {
                foreach (DocumentLine docLine in doc.DocumentLines)
                {
                    var ItemPrice = docLine.UnitPrice ?? 0;
                    var ItemQty = docLine.Quantity ?? 0;
                    var ItemDisRate = docLine.DiscountPercent ?? 0;

                    var taxRate = docLine.TaxPercentagePerRow ?? 0;

                    var LineTotal = docLine.LineTotal ?? 0;

                    var total = ItemPrice * ItemQty;
                    if (!string.IsNullOrEmpty(docLine.WeighScaleBarcode))
                    {
                        total = Math.Round(ItemPrice * ItemQty, doc.RoundingDigit, MidpointRounding.AwayFromZero);
                    } 
                    var discountAmt = Math.Round(total * ItemDisRate / 100, doc.RoundingDigit, MidpointRounding.AwayFromZero);
                    var totalWithDiscount = total - discountAmt;

                    if (docLine.PromoType == PromoValueType.BonusAmount || docLine.PromoType == PromoValueType.BonusPercent)
                    {
                        totalWithDiscount = total;
                    }
                    var taxAmt = totalWithDiscount * taxRate / 100;
                    var totalWithTax = totalWithDiscount + taxAmt;

                    totalLineTax += taxAmt;
                    TotalLine += total;
                    TotalLineDis += discountAmt;
                    totalPayable += totalWithTax;
                    //TotalSumReceipt += LineTotal;
                    //TotalDepositAmnt += depositAmnt;
                    //totalWeight += (cbm * ItemQty);
                    if (ItemQty > 0)
                        TotalQty += ItemQty;
                    if (totalWithTax < 0)
                        TotalReturnAmnt += totalWithTax;
                    docLine.LineTotal = totalWithTax;
                    docLine.UDisPrcnt = docLine.DiscountPercent;
                    docLine.UDisAmt = discountAmt;
                    docLine.UPriceAfDis = ItemPrice * (100 - ItemDisRate) / 100;
                    //docLine.UPriceAfDis = Math.Round(ItemPrice * (100 - ItemDisRate) / 100, RoundLength);
                    if (docLine.PromoType == PromoValueType.BonusAmount || docLine.PromoType == PromoValueType.BonusPercent)
                    {
                        docLine.UPriceAfDis = ItemPrice;
                        //docLine.UPriceAfDis = Math.Round(ItemPrice, RoundLength);
                    }
                    docLine.UTotalAfDis = totalWithDiscount;
                    docLine.VatPerPriceAfDis = (docLine.UPriceAfDis ?? 0) * (docLine.TaxPercentagePerRow ?? 0) / 100;
                    //docLine.VatPerPriceAfDis = Math.Round((docLine.UPriceAfDis ?? 0) * (docLine.TaxPercentagePerRow ?? 0) / 100, RoundLength);
                    docLine.PriceAfDisAndVat = docLine.VatPerPriceAfDis + (docLine.UPriceAfDis ?? 0);
                }
            }
            //TotalLineDis = Math.Round(TotalLineDis = Math.Round(TotalLineDis / 100, RoundLength) * 100;
            //TotalLine = Math.Round(TotalLine / 100, RoundLength) * 100;
            //totalPayable = Math.Round(totalPayable, RoundLength);
            //totalLineTax = Math.Round(totalLineTax / 100, RoundLength) * 100;

            txtTotalPayable = totalPayable;
        }

        private double GetLineTotal(DocumentLine docLine)
        {
            var total = (docLine.UnitPrice ?? 0) * (docLine.Quantity ?? 0);

            var discountAmt = total * (docLine.DiscountPercent ?? 0) / 100;

            var totalWithDiscount = total - discountAmt;

            var taxAmt = totalWithDiscount * (docLine.TaxPercentagePerRow ?? 0) / 100;

            var totalWithTax = totalWithDiscount + taxAmt;

            return totalWithTax;
        }

        //private DocumentLine Add_PromoLineToGrid(int Index, bool ByBarcode, string BarCode, string ItemCode, string ItemName, string UOMID, double Qty, double ManualPrice, double ItemDis, string ServiecRefNo, string PromoType, double PromoPrcntge, double PromoAmt, string PromoBaseItem, string DiscBaseObj, string cardCode, DateTime docDate, string companyCode, string storeId, string cardGroup)
        private DocumentLine Add_PromoLineToGrid(int Index, bool ByBarcode, string BarCode, string ItemCode, string UOMID, double Qty, double ManualPrice, double ItemDis, string PromoType, double PromoAmt, string companyCode, string storeId, string cardGroup)
        {
            try
            {
                ItemViewModel itemInfo = null;

                if (!ByBarcode)
                {
                    itemInfo = this.GetItemInfo(companyCode, storeId, ItemCode, UOMID, string.Empty, cardGroup);
                }
                else
                {
                    itemInfo = this.GetItemInfo(companyCode, storeId, string.Empty, string.Empty, BarCode, cardGroup);
                }

                if (itemInfo == null)
                {
                    return null;
                }
                string ItemName = "";
                var ItemBarCode = BarCode;
                var ItemPrice = 0.0d;
                var ItemDiscount = 0.0d;
                var SubTotal = 0.0d;
                //var isFree = "N";
                var TaxRate = 0;// 1.0d;
                var ItemGroupCod = "";
                var UomEntry = 0;
                var VatGroup = "";
                //var TaxCode = string.Empty;
                //string size = string.Empty;
                //string color = string.Empty;
                //var ItemGroup = string.Empty;
                //var ItemCollection = string.Empty;
                //int deliveryType = 0;
                //double depositPcnt = 0.00;

                if (itemInfo != null)
                {
                    ItemBarCode = itemInfo.BarCode;
                    ItemCode = itemInfo.ItemCode;
                    UOMID = itemInfo.UomCode;
                    ItemName = itemInfo.ItemName;

                    if (!itemInfo.DefaultPrice.HasValue)
                    {
                        itemInfo.DefaultPrice = itemInfo.PriceAfterTax;
                    }
                    if (PromoType == PromoValueType.DiscountAmount)
                    {
                        if (itemInfo.DefaultPrice == null || itemInfo.DefaultPrice.Value <= 0)
                        {
                            return null;    // không chấp nhận item khuyến mãi không có giá
                        }

                        ItemPrice = Convert.ToDouble(itemInfo.DefaultPrice.Value);
                        if (ItemPrice < PromoAmt)
                            PromoAmt = 0;
                        ItemDiscount = PromoAmt / ItemPrice * 100;
                    }
                    else if (PromoType == PromoValueType.DiscountPercent)
                    {
                        if (ItemDis < 100 && (itemInfo.DefaultPrice == null || itemInfo.DefaultPrice.Value <= 0))
                        {
                            return null;    // không chấp nhận item khuyến mãi không có giá
                        }
                        ItemPrice = Convert.ToDouble(itemInfo.DefaultPrice.Value);
                        ItemDiscount = ItemDis;
                    }
                    else if (PromoType == PromoValueType.FixedPrice)
                    {
                        ItemPrice = Convert.ToDouble(itemInfo.DefaultPrice.Value);
                        if (ItemPrice != 0)
                        {
                            ItemDiscount = Math.Max(ItemPrice - ManualPrice, 0) / ItemPrice * 100;
                        }
                        else
                        {
                            ItemDiscount = 0;
                        }
                    }
                    else if (PromoType == PromoValueType.FixedQuantity)
                    {
                        ItemPrice = Convert.ToDouble(itemInfo.DefaultPrice.Value);
                        ItemDiscount = 100;
                    }
                    else if (PromoType == PromoValueType.BonusAmount)
                    {
                        if (itemInfo.DefaultPrice == null || itemInfo.DefaultPrice.Value <= 0)
                        {
                            return null;    // không chấp nhận item khuyến mãi không có giá
                        }
                        ItemPrice = Convert.ToDouble(itemInfo.DefaultPrice.Value);
                        if (ItemPrice <= 0 || ItemPrice < PromoAmt)
                            PromoAmt = 0;
                        ItemDiscount = PromoAmt / ItemPrice * 100;
                    }
                    else if (PromoType == PromoValueType.BonusPercent)
                    {
                        if (itemInfo.DefaultPrice == null || itemInfo.DefaultPrice.Value <= 0)
                        {
                            return null;    // không chấp nhận item khuyến mãi không có giá
                        }
                        ItemPrice = Convert.ToDouble(itemInfo.DefaultPrice.Value);
                        ItemDiscount = ItemDis;
                    }
                    else
                    {
                        ItemPrice = Convert.ToDouble(itemInfo.DefaultPrice.Value);
                        ItemDiscount = ItemDis;
                    }

                    ItemGroupCod = itemInfo.ItemGroupId;
                    VatGroup = itemInfo.SalesTaxCode;
                }

                DocumentLine newLine = new DocumentLine
                {
                    BarCode = ItemBarCode,
                    DiscountPercent = ItemDiscount,
                    ItemCode = ItemCode,
                    ItemDescription = ItemName,
                    ItemGroup = ItemGroupCod,
                    UoMCode = UOMID,
                    LineNum = Index,
                    LineTotal = SubTotal,
                    Quantity = Math.Round(Qty, RoundLengthQty, MidpointRounding.AwayFromZero),
                    TaxPercentagePerRow = TaxRate,
                    UnitPrice = ItemPrice,
                    //ItmsGrpCod = ItemGroupCod,
                    UoMEntry = UomEntry,
                    PromoType = PromoType,
                    VatGroup = VatGroup,
                    UIsPromo = "1",
                    CustomField1 = itemInfo.CustomField1,
                    CustomField2 = itemInfo.CustomField2,
                    CustomField3 = itemInfo.CustomField3,
                    CustomField4 = itemInfo.CustomField4,
                    CustomField5 = itemInfo.CustomField5,
                    CustomField6 = itemInfo.CustomField6,
                    CustomField7 = itemInfo.CustomField7,
                    CustomField8 = itemInfo.CustomField8,
                    CustomField9 = itemInfo.CustomField9,
                    CustomField10 = itemInfo.CustomField10,
                    SalesTaxCode = itemInfo.SalesTaxCode,
                    SalesTaxRate = itemInfo.SalesTaxRate,
                    PurchaseTaxCode = itemInfo.PurchaseTaxCode,
                    PurchaseTaxRate = itemInfo.PurchaseTaxRate,
                    IsSerial = itemInfo.IsSerial,
                    IsVoucher = itemInfo.isVoucher,
                    ItemCategory1 = itemInfo.ItemCategory_1,
                    ItemCategory2 = itemInfo.ItemCategory_2,
                    ItemCategory3 = itemInfo.ItemCategory_3,
                    PriceListId = itemInfo.PriceListId,
                    ProductId = itemInfo.ProductId
                };

                return newLine;
            }
            catch
            {
                return null;
            }
        }

        private bool Apply_Single_Promotion_Content(ref DocumentLine docLine, ref List<DocumentLine> newLinePromo, int documentLineCount, DateTime docDate, string promoId, string promoName, SPromoGet promoLine, string schemaId, string schemaName, string companyCode, string StoreId, string voucherNum, string cardGroup)
        {
            int MultiRate = 1;
            bool result = false;
            if (promoLine.ConditionType == PromoCondition.Quantity)
            {
                if (promoLine.Condition1 == PromoCondition.CE)
                {
                    if (promoLine.Value1.Value <= docLine.Quantity.Value)
                    {
                        //MultiRate = Convert.ToInt32(docLine.Quantity.Value) / Convert.ToInt32(promoLine.Value1.Value);
                        MultiRate = (int)Math.Round((docLine.Quantity.Value / promoLine.Value1.Value), RoundLengthQty, MidpointRounding.AwayFromZero);
                        double ItemPrice;

                        double maxQty = Math.Min(docLine.Quantity.Value, Math.Round(promoLine.Value1.Value * MultiRate, RoundLengthQty, MidpointRounding.AwayFromZero));
                        if ((promoLine.MaxQtyDis ?? 0) > 0)
                        {
                            maxQty = Math.Min(maxQty, promoLine.MaxQtyDis.Value);
                        }

                        if (promoLine.ValueType == PromoValueType.FixedPrice)
                        {
                            //  tính lại giá trị của dispercent
                            ItemPrice = docLine.UnitPrice ?? 0;
                            double FixedPrice = promoLine.GetValue ?? 0;
                            if (FixedPrice < ItemPrice)
                            {
                                if (docLine.Quantity > maxQty)
                                {
                                    DocumentLine newLine = docLine.Clone();
                                    //newLine.Quantity -= maxQty;
                                    newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                    newLine.LineNum = documentLineCount + newLinePromo.Count;
                                    newLinePromo.Add(newLine);

                                    docLine.Quantity = maxQty;
                                    docLine.LineTotal = GetLineTotal(docLine);
                                }

                                //taxRate = 0;// docLine.TaxPercentagePerRow ?? 0;
                                //ItemPrice = docLine.UnitPrice ?? 0;
                                //var FixedPrice = promoLine.GetValue / (100 + taxRate) * 100;
                                var DisPrcnt = (ItemPrice - FixedPrice) * 100 / ItemPrice;
                                if (DisPrcnt < 0 || double.IsNaN(DisPrcnt))
                                {
                                    DisPrcnt = 0;
                                }

                                docLine.DiscountPercent = DisPrcnt;
                                docLine.PromoType = PromoValueType.FixedPrice;

                                docLine.UPromoCode += promoId.ToString() + ", ";
                                docLine.UPromoName = promoName;
                                if (!string.IsNullOrEmpty(schemaId))
                                {
                                    docLine.USchemaCode = schemaId;
                                    docLine.USchemaName = schemaName;
                                }
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    docLine.RefTransId = voucherNum;
                                    docLine.ApplyType = "CRM";
                                }
                                result = true;
                            }
                        }
                        else if (promoLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            //  add line mới với quantity = getvalue, unitprice = 0

                            docLine.PromoType = PromoValueType.FixedQuantity;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }

                            double fixedQty = promoLine.GetValue.Value * MultiRate;
                            if ((promoLine.MaxQtyDis ?? 0) > 0 && promoLine.MaxQtyDis.Value < fixedQty)
                            {
                                fixedQty = promoLine.MaxQtyDis ?? 0;
                            }

                            int index = documentLineCount + newLinePromo.Count;
                            var newDocLine = Add_PromoLineToGrid(index, false, string.Empty, promoLine.LineCode, promoLine.LineUom, fixedQty, 0, 0, promoLine.ValueType, 0, companyCode, StoreId, cardGroup);
                            newDocLine.UPromoCode += promoId.ToString() + ", ";
                            newDocLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                newDocLine.USchemaCode = schemaId;
                                newDocLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                newDocLine.RefTransId = voucherNum;
                                newDocLine.ApplyType = "CRM";
                            }
                            newLinePromo.Add(newDocLine);
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            //  thay đổi dispercent trên line
                            if (docLine.Quantity > maxQty)
                            {
                                DocumentLine newLine = docLine.Clone();
                                //newLine.Quantity -= maxQty;
                                newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                newLine.LineNum = documentLineCount + newLinePromo.Count;
                                newLinePromo.Add(newLine);

                                docLine.Quantity = maxQty;
                                docLine.LineTotal = GetLineTotal(docLine);
                            }

                            double maxDiscount = promoLine.MaxAmtDis ?? 0;
                            double maxPercent = maxDiscount / docLine.LineTotal.Value * 100;
                            if (maxPercent <= 0 || double.IsNaN(maxPercent))
                            {
                                maxPercent = promoLine.GetValue ?? 0;
                            }

                            if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                            {
                                docLine.DiscountPercent = Math.Min(promoLine.GetValue.Value, maxPercent);
                            }
                            else
                            {
                                double firstPrnct = docLine.DiscountPercent ?? 0;
                                double newPrnct = Math.Min(promoLine.GetValue.Value, maxPercent);
                                double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                docLine.DiscountPercent = lastPrcnt;
                            }
                            docLine.PromoType = PromoValueType.DiscountPercent;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            if ((promoLine.MaxQtyDis ?? 0) > 0 && docLine.Quantity > maxQty)
                            {
                                DocumentLine newLine = docLine.Clone();
                                //newLine.Quantity -= maxQty;
                                newLine.Quantity = Math.Round(docLine.Quantity.Value - maxQty, RoundLengthQty, MidpointRounding.AwayFromZero);
                                newLine.LineNum = documentLineCount + newLinePromo.Count;
                                newLinePromo.Add(newLine);

                                docLine.Quantity = maxQty;
                                docLine.LineTotal = GetLineTotal(docLine);
                            }

                            double PromoAmt = promoLine.GetValue.Value * MultiRate;
                            if ((promoLine.MaxAmtDis ?? 0) > 0 && PromoAmt > promoLine.MaxAmtDis.Value)
                            {
                                PromoAmt = promoLine.MaxAmtDis ?? 0;
                            }
                            double disPrcnt = docLine.DiscountPercent ?? 0;
                            double lineTotal = docLine.LineTotal ?? 0;
                            double newAmt = lineTotal * (100 - disPrcnt) / 100;
                            //double newAmt = Math.Round(lineTotal * (100 - disPrcnt) / 100, RoundLength);
                            if (lineTotal <= 0)
                                PromoAmt = 0;

                            double ItemDiscount = Math.Min(100, 100 - (newAmt - PromoAmt) / lineTotal * 100);

                            docLine.DiscountPercent = ItemDiscount;
                            docLine.PromoType = PromoValueType.DiscountAmount;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                    }
                }
                else if (promoLine.Condition1 == PromoCondition.FROM && promoLine.Condition2 == PromoCondition.TO)
                {
                    if (docLine.Quantity.Value >= promoLine.Value1.Value
                        && (promoLine.Value2 == null || promoLine.Value2.Value == 0 || docLine.Quantity <= promoLine.Value2))
                    {
                        double taxRate, ItemPrice;

                        if (promoLine.ValueType == PromoValueType.FixedPrice)
                        {
                            taxRate = 0;// docLine.TaxPercentagePerRow ?? 0;
                            ItemPrice = docLine.UnitPrice ?? 0;
                            var FixedPrice = promoLine.GetValue ?? 0 / (100 + taxRate) * 100;
                            var DisPrcnt = (ItemPrice - FixedPrice) * 100 / ItemPrice;
                            if (FixedPrice < ItemPrice)
                            {
                                if (DisPrcnt < 0 || double.IsNaN(DisPrcnt))
                                {
                                    DisPrcnt = 0;
                                }

                                docLine.DiscountPercent = DisPrcnt;
                                docLine.PromoType = PromoValueType.FixedPrice;

                                docLine.UPromoCode += promoId.ToString() + ", ";
                                docLine.UPromoName = promoName;
                                if (!string.IsNullOrEmpty(schemaId))
                                {
                                    docLine.USchemaCode = schemaId;
                                    docLine.USchemaName = schemaName;
                                }
                                if (!string.IsNullOrEmpty(voucherNum))
                                {
                                    docLine.RefTransId = voucherNum;
                                    docLine.ApplyType = "CRM";
                                }

                                result = true;
                            }
                        }
                        else if (promoLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            docLine.PromoType = PromoValueType.FixedQuantity;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }

                            int index = documentLineCount + newLinePromo.Count;
                            var newDocLine = Add_PromoLineToGrid(index, false, string.Empty, promoLine.LineCode, promoLine.LineUom, promoLine.GetValue.Value, 0, 0, promoLine.ValueType, 0, companyCode, StoreId, cardGroup);
                            newDocLine.UPromoCode += promoId.ToString() + ", ";
                            newDocLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                newDocLine.USchemaCode = schemaId;
                                newDocLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                newDocLine.RefTransId = voucherNum;
                                newDocLine.ApplyType = "CRM";
                            }
                            newLinePromo.Add(newDocLine);
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            double maxDiscount = promoLine.MaxAmtDis ?? 0;
                            double maxPercent = maxDiscount / docLine.LineTotal.Value * 100;
                            if (maxPercent <= 0 || double.IsNaN(maxPercent))
                            {
                                maxPercent = promoLine.GetValue ?? 0;
                            }

                            if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                            {
                                docLine.DiscountPercent = Math.Min(promoLine.GetValue.Value, maxPercent);
                            }
                            else
                            {
                                double firstPrnct = docLine.DiscountPercent ?? 0;
                                double newPrnct = Math.Min(promoLine.GetValue.Value, maxPercent);
                                double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                docLine.DiscountPercent = lastPrcnt;
                            }

                            docLine.PromoType = PromoValueType.DiscountPercent;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            double PromoAmt = promoLine.GetValue ?? 0;
                            double disPrcnt = docLine.DiscountPercent ?? 0;
                            double lineTotal = docLine.LineTotal ?? 0;
                            double newAmt = lineTotal * (100 - disPrcnt) / 100;
                            //double newAmt = Math.Round(lineTotal * (100 - disPrcnt) / 100, RoundLength);
                            if (lineTotal <= 0)
                                PromoAmt = 0;

                            double ItemDiscount = Math.Min(100, 100 - (newAmt - PromoAmt) / lineTotal * 100);

                            docLine.DiscountPercent = ItemDiscount;
                            docLine.PromoType = PromoValueType.DiscountAmount;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                    }
                }
            }
            else if (promoLine.ConditionType == PromoCondition.Amount)
            {
                if (promoLine.Condition1 == PromoCondition.CE)
                {
                    if (promoLine.Value1 <= docLine.LineTotal)
                    {
                        //MultiRate = Convert.ToInt32(docLine.LineTotal.Value) / Convert.ToInt32(promoLine.Value1.Value);
                        MultiRate = (int)Math.Round((docLine.LineTotal.Value / promoLine.Value1.Value), RoundLengthQty, MidpointRounding.AwayFromZero);
                        //double taxRate, ItemPrice;

                        //if(promoLine.ValueType == PromoValueType.FixedPrice)
                        //{
                        //    if ((promoLine.MaxQtyDis ?? 0) > 0)
                        //    {
                        //        DocumentLine newLine = docLine.Clone();
                        //        newLine.Quantity = promoLine.MaxQtyDis.Value;
                        //        newLine.LineNum = documentLineCount + newLinePromo.Count;
                        //        newLinePromo.Add(newLine);

                        //        docLine.Quantity -= promoLine.MaxQtyDis.Value;
                        //        docLine.LineTotal = GetLineTotal(docLine);
                        //    }

                        //    taxRate = 0;// docLine.TaxPercentagePerRow ?? 0;
                        //    ItemPrice = docLine.UnitPrice ?? 0;
                        //    var FixedPrice = promoLine.GetValue ?? 0 / (100 + taxRate) * 100;
                        //    var DisPrcnt = (ItemPrice - FixedPrice) * 100 / ItemPrice;
                        //    if (DisPrcnt < 0)
                        //    {
                        //        DisPrcnt = 0;
                        //    }

                        //    docLine.DiscountPercent = DisPrcnt;
                        //    docLine.PromoType = PromoValueType.FixedPrice;

                        //    docLine.UPromoCode += promoId.ToString() + ", ";
                        //    docLine.UPromoName = promoName;
                        //    if (!string.IsNullOrEmpty(schemaId))
                        //    {
                        //        docLine.USchemaCode = schemaId;
                        //        docLine.USchemaName = schemaName;
                        //    }
                        //    if (!string.IsNullOrEmpty(voucherNum))
                        //    {
                        //        docLine.RefTransId = voucherNum;
                        //        docLine.ApplyType = "CRM";
                        //    }
                        //    result = true;
                        //}
                        //else
                        if (promoLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            docLine.PromoType = PromoValueType.FixedQuantity;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }

                            double fixedQty = promoLine.GetValue.Value * MultiRate;
                            if ((promoLine.MaxQtyDis ?? 0) > 0 && promoLine.MaxQtyDis.Value < fixedQty)
                            {
                                fixedQty = promoLine.MaxQtyDis ?? 0;
                            }
                            int index = documentLineCount + newLinePromo.Count;
                            var newDocLine = Add_PromoLineToGrid(index, true, promoLine.LineCode, string.Empty, promoLine.LineUom, fixedQty, 0, 0, promoLine.ValueType, 0, companyCode, StoreId, cardGroup);
                            newDocLine.UPromoCode += promoId.ToString() + ", ";
                            newDocLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                newDocLine.USchemaCode = schemaId;
                                newDocLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                newDocLine.RefTransId = voucherNum;
                                newDocLine.ApplyType = "CRM";
                            }
                            newLinePromo.Add(newDocLine);
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            double maxAmtApply = Math.Min(docLine.LineTotal.Value, promoLine.Value1.Value * MultiRate);
                            double maxDiscount = promoLine.GetValue.Value / maxAmtApply * 100;
                            if ((promoLine.MaxAmtDis ?? 0) > 0 && maxDiscount > promoLine.MaxAmtDis.Value)
                            {
                                maxDiscount = promoLine.MaxAmtDis ?? 0;
                            }
                            //double maxDiscount = promoLine.MaxAmtDis ?? 0;
                            double maxPercent = maxDiscount / docLine.LineTotal.Value * 100;
                            if (maxPercent <= 0 || double.IsNaN(maxPercent))
                            {
                                maxPercent = promoLine.GetValue ?? 0;
                            }

                            if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                            {
                                docLine.DiscountPercent = Math.Min(promoLine.GetValue.Value, maxPercent);
                            }
                            else
                            {
                                double firstPrnct = docLine.DiscountPercent ?? 0;
                                double newPrnct = Math.Min(promoLine.GetValue.Value, maxPercent);
                                double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                docLine.DiscountPercent = lastPrcnt;
                            }
                            docLine.PromoType = PromoValueType.DiscountPercent;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            double PromoAmt = promoLine.GetValue.Value * MultiRate;
                            if ((promoLine.MaxAmtDis ?? 0) > 0 && PromoAmt > promoLine.MaxAmtDis.Value)
                            {
                                PromoAmt = promoLine.MaxAmtDis ?? 0;
                            }
                            double disPrcnt = docLine.DiscountPercent ?? 0;
                            double lineTotal = docLine.LineTotal ?? 0;
                            double newAmt = lineTotal * (100 - disPrcnt) / 100;
                            //double newAmt = Math.Round(lineTotal * (100 - disPrcnt) / 100, RoundLength);
                            if (lineTotal <= 0)
                                PromoAmt = 0;

                            double ItemDiscount = Math.Min(100, 100 - (newAmt - PromoAmt) / lineTotal * 100);

                            docLine.DiscountPercent = ItemDiscount;
                            docLine.PromoType = PromoValueType.DiscountAmount;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                    }
                }
                else if (promoLine.Condition1 == PromoCondition.FROM && promoLine.Condition2 == PromoCondition.TO)
                {
                    if (docLine.LineTotal >= promoLine.Value1
                        && (promoLine.Value2 == null || promoLine.Value2.Value == 0 || docLine.LineTotal <= promoLine.Value2))
                    {
                        //double taxRate, ItemPrice;

                        //if(promoLine.ValueType == PromoValueType.FixedPrice)
                        //{
                        //    taxRate = 0;// docLine.TaxPercentagePerRow ?? 0;
                        //    ItemPrice = docLine.UnitPrice ?? 0;
                        //    var FixedPrice = promoLine.GetValue ?? 0 / (100 + taxRate) * 100;
                        //    var DisPrcnt = (ItemPrice - FixedPrice) * 100 / ItemPrice;
                        //    if (DisPrcnt < 0)
                        //    {
                        //        DisPrcnt = 0;
                        //    }

                        //    docLine.DiscountPercent = DisPrcnt;
                        //    docLine.PromoType = PromoValueType.FixedPrice;

                        //    docLine.UPromoCode += promoId.ToString() + ", ";
                        //    docLine.UPromoName = promoName;
                        //    if (!string.IsNullOrEmpty(schemaId))
                        //    {
                        //        docLine.USchemaCode = schemaId;
                        //        docLine.USchemaName = schemaName;
                        //    }
                        //    if (!string.IsNullOrEmpty(voucherNum))
                        //    {
                        //        docLine.RefTransId = voucherNum;
                        //        docLine.ApplyType = "CRM";
                        //    }
                        //    result = true;
                        //}
                        if (promoLine.ValueType == PromoValueType.FixedQuantity)
                        {
                            docLine.PromoType = PromoValueType.FixedQuantity;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }

                            int index = documentLineCount + newLinePromo.Count;
                            var newDocLine = Add_PromoLineToGrid(index, true, promoLine.LineCode, string.Empty, promoLine.LineUom, promoLine.GetValue.Value, 0, 0, promoLine.ValueType, 0, companyCode, StoreId, cardGroup);
                            newDocLine.UPromoCode += promoId.ToString() + ", ";
                            newDocLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                newDocLine.USchemaCode = schemaId;
                                newDocLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                newDocLine.RefTransId = voucherNum;
                                newDocLine.ApplyType = "CRM";
                            }
                            newLinePromo.Add(newDocLine);
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountPercent)
                        {
                            double maxDiscount = promoLine.MaxAmtDis ?? 0;
                            double maxPercent = maxDiscount / docLine.LineTotal.Value * 100;
                            if (maxPercent <= 0 || double.IsNaN(maxPercent))
                            {
                                maxPercent = promoLine.GetValue ?? 0;
                            }

                            if (docLine.DiscountPercent == null || docLine.DiscountPercent.Value == 0)
                            {
                                docLine.DiscountPercent = Math.Min(promoLine.GetValue.Value, maxPercent);
                            }
                            else
                            {
                                double firstPrnct = docLine.DiscountPercent ?? 0;
                                double newPrnct = Math.Min(promoLine.GetValue.Value, maxPercent);
                                double lastPrcnt = (firstPrnct + newPrnct) - (firstPrnct * newPrnct) / 100;
                                docLine.DiscountPercent = lastPrcnt;
                            }
                            docLine.PromoType = PromoValueType.DiscountPercent;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                        else if (promoLine.ValueType == PromoValueType.DiscountAmount)
                        {
                            double PromoAmt = promoLine.GetValue ?? 0;
                            if ((promoLine.MaxAmtDis ?? 0) > 0 && PromoAmt > promoLine.MaxAmtDis.Value)
                            {
                                PromoAmt = promoLine.MaxAmtDis ?? 0;
                            }
                            double disPrcnt = docLine.DiscountPercent ?? 0;
                            double lineTotal = docLine.LineTotal ?? 0;
                            double newAmt = lineTotal * (100 - disPrcnt) / 100;
                            //double newAmt = Math.Round(lineTotal * (100 - disPrcnt) / 100, RoundLength);
                            if (lineTotal <= 0)
                                PromoAmt = 0;

                            double ItemDiscount = Math.Min(100, 100 - (newAmt - PromoAmt) / lineTotal * 100);

                            docLine.DiscountPercent = ItemDiscount;
                            docLine.PromoType = PromoValueType.DiscountAmount;

                            docLine.UPromoCode += promoId.ToString() + ", ";
                            docLine.UPromoName = promoName;
                            if (!string.IsNullOrEmpty(schemaId))
                            {
                                docLine.USchemaCode = schemaId;
                                docLine.USchemaName = schemaName;
                            }
                            if (!string.IsNullOrEmpty(voucherNum))
                            {
                                docLine.RefTransId = voucherNum;
                                docLine.ApplyType = "CRM";
                            }
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        private DataTable CreateTablePromoHeader()
        {
            DataTable dt = new DataTable("PROMO_HEADER");
            dt.Columns.Add("PromoId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("PromoType");
            dt.Columns.Add("PromoName");
            dt.Columns.Add("CustomerType");
            dt.Columns.Add("ValidDateFrom", typeof(DateTime));
            dt.Columns.Add("ValidDateTo", typeof(DateTime));
            dt.Columns.Add("ValidTimeFrom");
            dt.Columns.Add("ValidTimeTo");
            dt.Columns.Add("IsMon");
            dt.Columns.Add("IsTue");
            dt.Columns.Add("IsWed");
            dt.Columns.Add("IsThu");
            dt.Columns.Add("IsFri");
            dt.Columns.Add("IsSat");
            dt.Columns.Add("IsSun");
            dt.Columns.Add("TotalBuyFrom");
            dt.Columns.Add("TotalBuyTo");
            dt.Columns.Add("TotalGetType");
            dt.Columns.Add("TotalGetValue");
            dt.Columns.Add("MaxTotalGetValue");
            dt.Columns.Add("IsCombine");
            dt.Columns.Add("IsVoucher");
            dt.Columns.Add("CreatedBy");
            dt.Columns.Add("CreatedOn", typeof(DateTime));
            dt.Columns.Add("ModifiedBy");
            dt.Columns.Add("ModifiedOn", typeof(DateTime));
            dt.Columns.Add("Status");
            dt.Columns.Add("SAPPromoId");
            dt.Columns.Add("SAPBonusBuyId");
            dt.Columns.Add("SchemaId");
            dt.Columns.Add("MaxQtyByReceipt");
            dt.Columns.Add("MaxQtyByStore");
            //dt.Columns.Add("CustomField1");
            //dt.Columns.Add("CustomField2");
            //dt.Columns.Add("CustomField3");

            return dt;
        }

        private DataTable CreateTablePromoBuy()
        {
            DataTable dt = new DataTable("PROMO_BUY");
            dt.Columns.Add("PromoId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LineNum");
            dt.Columns.Add("LineType");
            dt.Columns.Add("LineCode");
            dt.Columns.Add("LineName");
            dt.Columns.Add("LineUom");
            dt.Columns.Add("ValueType");
            dt.Columns.Add("Condition_1");
            dt.Columns.Add("Value_1");
            dt.Columns.Add("Condition_2");
            dt.Columns.Add("Value_2");
            ////dt.Columns.Add("InActive");
            ////dt.Columns.Add("ModifiedDate");

            return dt;
        }

        private DataTable CreateTablePromoGet()
        {
            DataTable dt = new DataTable("PROMO_GET");
            dt.Columns.Add("PromoId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LineNum");
            dt.Columns.Add("LineType");
            dt.Columns.Add("LineCode");
            dt.Columns.Add("LineName");
            dt.Columns.Add("LineUom");
            dt.Columns.Add("ConditionType");
            dt.Columns.Add("Condition_1");
            dt.Columns.Add("Value_1");
            dt.Columns.Add("Condition_2");
            dt.Columns.Add("Value_2");
            dt.Columns.Add("ValueType");
            dt.Columns.Add("GetValue");
            dt.Columns.Add("MaxAmtDis");
            dt.Columns.Add("MaxQtyDis");
            ////dt.Columns.Add("InActive");
            ////dt.Columns.Add("ModifiedDate");

            return dt;
        }

        private DataTable CreateTablePromoStore()
        {
            DataTable dt = new DataTable("PROMO_STORE");
            dt.Columns.Add("PromoId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LineNum");
            dt.Columns.Add("StoreValue");

            return dt;
        }

        private DataTable CreateTablePromoCustomer()
        {
            DataTable dt = new DataTable("PROMO_CUSTOMER");
            dt.Columns.Add("PromoId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LineNum");
            dt.Columns.Add("CustomerValue");
            dt.Columns.Add("CustomerType");

            return dt;
        }

        private DataTable CreateTablePromoOTGroup(string name)
        {
            DataTable dt = new DataTable(name);
            dt.Columns.Add("PromoId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("GroupID");
            dt.Columns.Add("LineNum");
            dt.Columns.Add("LineType");
            dt.Columns.Add("LineCode");
            dt.Columns.Add("LineName");
            dt.Columns.Add("LineUoM");

            return dt;
        }

        //private Document MergeLineDocument(Document doc)
        //{
        //    Document newDoc = doc.Clone();

        //    newDoc.DocumentLines = new List<DocumentLine>();
        //    int lineNum = 1;
        //    foreach (DocumentLine docLine in doc.DocumentLines)
        //    {
        //        if (docLine.UIsPromo != "1")
        //        {
        //            DocumentLine lineCheck = newDoc.DocumentLines.Where(l => l.ItemCode == docLine.ItemCode).FirstOrDefault();
        //            if (lineCheck == null)
        //            {
        //                docLine.LineNum = lineNum++;
        //                newDoc.DocumentLines.Add(docLine);
        //            }
        //            else
        //            {
        //                for (int i = 0; i < newDoc.DocumentLines.Count; i++)
        //                {
        //                    if(newDoc.DocumentLines[i].ItemCode==lineCheck.ItemCode && newDoc.DocumentLines[i].LineNum == lineCheck.LineNum)
        //                    {
        //                        newDoc.DocumentLines[i].Quantity += docLine.Quantity;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    Set_ReCalcGrid(ref newDoc);
        //    Set_SumTotalAmt(ref newDoc);

        //    return newDoc;
        //}

        private Document ApplyPaymentDiscount(Document doc, PromotionViewModel promotion)
        {
            double sumPaymentDis = 0;

            if (doc.PromotionApply == null)
            {
                doc.PromotionApply = new List<PromotionViewModel>();
            }

            foreach (TSalesPayment payment in doc.SalesPayments)
            {
                if (promotion.IsCombine != "Y")
                {
                    if (!string.IsNullOrEmpty(payment.PromoId) && !string.IsNullOrEmpty(payment.PromoId.Trim()))
                    {
                        continue;
                    }
                }

                List<SPromoGet> promoGets = promotion.PromoGets.Where(x => x.InActive != "Y").ToList();
                foreach (var promoGet in promoGets)
                {
                    if ((promoGet.LineType == PromoLineType.PaymentCode && promoGet.LineCode == payment.PaymentCode)
                        || (promoGet.LineType == PromoLineType.PaymentType && promoGet.LineCode == payment.PaymentType))
                    {
                        if (promoGet.ConditionType == PromoCondition.Amount)
                        {
                            if (promoGet.Condition1 == PromoCondition.CE)
                            {
                                if (promoGet.Value1.Value <= (double)(payment.CollectedAmount ?? 0))
                                {
                                    double paymentDiscount = 0;
                                    if (promoGet.ValueType == PromoValueType.DiscountPercent)
                                    {
                                        //payment.PaymentDiscount += Math.Round((payment.CollectedAmount ?? 0) * (decimal)(promoGet.GetValue ?? 0) / 100, RoundLengthQty, MidpointRounding.AwayFromZero);
                                        paymentDiscount = Math.Round((double)(payment.CollectedAmount ?? 0) * (promoGet.GetValue ?? 0) / 100, RoundLengthQty, MidpointRounding.AwayFromZero);

                                    }
                                    else if (promoGet.ValueType == PromoValueType.DiscountAmount)
                                    {
                                        paymentDiscount = promoGet.GetValue.Value;
                                    }

                                    if ((promoGet.MaxAmtDis ?? 0) > 0)
                                    {
                                        paymentDiscount = Math.Min(paymentDiscount, promoGet.MaxAmtDis.Value);
                                    }

                                    if ((promotion.MaxTotalGetValue ?? 0) > 0)
                                    {
                                        if (sumPaymentDis + paymentDiscount > promotion.MaxTotalGetValue.Value)
                                        {
                                            paymentDiscount = Math.Max(0, promotion.MaxTotalGetValue.Value - sumPaymentDis);
                                        }
                                    }

                                    sumPaymentDis += paymentDiscount;
                                    payment.PaymentDiscount = (payment.PaymentDiscount ?? 0) + (decimal)paymentDiscount;
                                    payment.PromoId += promotion.PromoId + ", ";
                                }
                            }
                            else if (promoGet.Condition1 == PromoCondition.FROM)
                            {
                                double collectedAmt = (double)(payment.CollectedAmount ?? 0);
                                if (collectedAmt >= (promoGet.Value1 ?? 0) && ((promoGet.Value2 ?? 0) == 0 || collectedAmt <= promoGet.Value2))
                                {
                                    double paymentDiscount = 0;
                                    if (promoGet.ValueType == PromoValueType.DiscountPercent)
                                    {
                                        paymentDiscount = Math.Round((double)(payment.CollectedAmount ?? 0) * (promoGet.GetValue ?? 0) / 100, RoundLengthQty, MidpointRounding.AwayFromZero);
                                    }
                                    else if (promoGet.ValueType == PromoValueType.DiscountAmount)
                                    {
                                        paymentDiscount = promoGet.GetValue.Value;
                                    }

                                    if ((promoGet.MaxAmtDis ?? 0) > 0)
                                    {
                                        paymentDiscount = Math.Min(paymentDiscount, promoGet.MaxAmtDis.Value);
                                    }

                                    if ((promotion.MaxTotalGetValue ?? 0) > 0)
                                    {
                                        if (sumPaymentDis + paymentDiscount > promotion.MaxTotalGetValue.Value)
                                        {
                                            paymentDiscount = Math.Max(0, promotion.MaxTotalGetValue.Value - sumPaymentDis);
                                        }
                                    }

                                    sumPaymentDis += paymentDiscount;
                                    payment.PaymentDiscount = (payment.PaymentDiscount ?? 0) + (decimal)paymentDiscount;
                                    payment.PromoId += promotion.PromoId + ", ";
                                }
                            }
                        }
                    }
                }

                string[] spl = payment.PromoId.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string Xstr in spl)
                {
                    if (doc.PromotionApply.Where(x => x.PromoId == Xstr).SingleOrDefault() == null)
                    {
                        if (dicPromotions.ContainsKey(Xstr))
                        {
                            doc.PromotionApply.Add(dicPromotions[Xstr]);
                            doc.PromotionApply[doc.PromotionApply.Count - 1].PromoBuys.Clear();
                            doc.PromotionApply[doc.PromotionApply.Count - 1].PromoGets.Clear();
                            doc.PromotionApply[doc.PromotionApply.Count - 1].PromoCustomers.Clear();
                            doc.PromotionApply[doc.PromotionApply.Count - 1].PromoStores.Clear();
                        }
                    }
                }
            }
            return doc;
        }

        #endregion

        #region Manual

        public List<SPromoHeader> CheckPromotions(string companyCode, string storeId, int promoType, string customerCode, string customerGrp, double totalBuy, DateTime docDate)
        {
            List<SPromoHeader> sPromos = new List<SPromoHeader>();
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("PromoType", promoType);
                    parameters.Add("CustomerCode", customerCode);
                    parameters.Add("CustomerGrp", customerGrp);
                    parameters.Add("CurrentDate", docDate.ToString(StringFormatConst.SQLDateParam));
                    parameters.Add("CurrentTime", docDate.ToString(StringFormatConst.SQLTimeParam));
                    parameters.Add("IsMon", docDate.DayOfWeek == DayOfWeek.Monday ? "Y" : "''");
                    parameters.Add("IsTue", docDate.DayOfWeek == DayOfWeek.Tuesday ? "Y" : "''");
                    parameters.Add("IsWed", docDate.DayOfWeek == DayOfWeek.Wednesday ? "Y" : "''");
                    parameters.Add("IsThu", docDate.DayOfWeek == DayOfWeek.Thursday ? "Y" : "''");
                    parameters.Add("IsFri", docDate.DayOfWeek == DayOfWeek.Friday ? "Y" : "''");
                    parameters.Add("IsSat", docDate.DayOfWeek == DayOfWeek.Saturday ? "Y" : "''");
                    parameters.Add("IsSun", docDate.DayOfWeek == DayOfWeek.Sunday ? "Y" : "''");
                    parameters.Add("TotalBuy", totalBuy);

                    string query = $"USP_S_S_CheckPromotions '{companyCode}', '{storeId}', '{promoType}', '{customerCode}', '{customerGrp}', '{docDate.ToString(StringFormatConst.SQLDateParam)}', {docDate.ToString(StringFormatConst.SQLTimeParam)}, '', '', '', 'Y', '', '', '', '{totalBuy}'";
                    //var rs = db.Query<SPromoHeader>(query, null, commandType: CommandType.Text);

                    var r = db.Query<SPromoHeader>("USP_S_S_CheckPromotions", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        sPromos = r.ToList();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return sPromos;
        }

        public List<SPromoHeader> CheckPromotionsBySchema(string companyCode, string storeId, int promoType, string customerCode, string customerGrp, double totalBuy, DateTime docDate, string schemaId)
        {
            List<SPromoHeader> sPromos = new List<SPromoHeader>();
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("PromoType", promoType);
                    parameters.Add("CustomerCode", customerCode);
                    parameters.Add("CustomerGrp", customerGrp);
                    parameters.Add("CurrentDate", docDate.ToString(StringFormatConst.SQLDateParam));
                    parameters.Add("CurrentTime", docDate.ToString(StringFormatConst.SQLTimeParam));
                    parameters.Add("IsMon", docDate.DayOfWeek == DayOfWeek.Monday ? "Y" : "''");
                    parameters.Add("IsTue", docDate.DayOfWeek == DayOfWeek.Tuesday ? "Y" : "''");
                    parameters.Add("IsWed", docDate.DayOfWeek == DayOfWeek.Wednesday ? "Y" : "''");
                    parameters.Add("IsThu", docDate.DayOfWeek == DayOfWeek.Thursday ? "Y" : "''");
                    parameters.Add("IsFri", docDate.DayOfWeek == DayOfWeek.Friday ? "Y" : "''");
                    parameters.Add("IsSat", docDate.DayOfWeek == DayOfWeek.Saturday ? "Y" : "''");
                    parameters.Add("IsSun", docDate.DayOfWeek == DayOfWeek.Sunday ? "Y" : "''");
                    parameters.Add("TotalBuy", totalBuy);
                    parameters.Add("SchemaId", schemaId);

                    string query = $"USP_S_S_CheckPromotionsBySchema '{companyCode}', '{storeId}', '{promoType}', '{customerCode}', '{customerGrp}', '{docDate.ToString(StringFormatConst.SQLDateParam)}', {docDate.ToString(StringFormatConst.SQLTimeParam)}, 'Y', '', '', '', '', '', '', '{totalBuy}', '{schemaId}'";
                    var rs = db.Query<SPromoHeader>(query, null, commandType: CommandType.Text);

                    var r = db.Query<SPromoHeader>("USP_S_S_CheckPromotionsBySchema", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        sPromos = r.ToList();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return sPromos;
        }

        public List<SPromoSchema> CheckSchemaHeader(string companyCode, string storeId, string customerCode, string customerGrp, double totalBuy, DateTime docDate)
        {
            List<SPromoSchema> schemas = new List<SPromoSchema>();
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("CustomerCode", customerCode);
                    parameters.Add("CustomerGrp", customerGrp);
                    parameters.Add("CurrentDate", docDate.ToString(StringFormatConst.SQLDateParam));
                    parameters.Add("CurrentTime", docDate.ToString(StringFormatConst.SQLTimeParam));
                    parameters.Add("IsMon", docDate.DayOfWeek == DayOfWeek.Monday ? "Y" : "''");
                    parameters.Add("IsTue", docDate.DayOfWeek == DayOfWeek.Tuesday ? "Y" : "''");
                    parameters.Add("IsWed", docDate.DayOfWeek == DayOfWeek.Wednesday ? "Y" : "''");
                    parameters.Add("IsThu", docDate.DayOfWeek == DayOfWeek.Thursday ? "Y" : "''");
                    parameters.Add("IsFri", docDate.DayOfWeek == DayOfWeek.Friday ? "Y" : "''");
                    parameters.Add("IsSat", docDate.DayOfWeek == DayOfWeek.Saturday ? "Y" : "''");
                    parameters.Add("IsSun", docDate.DayOfWeek == DayOfWeek.Sunday ? "Y" : "''");
                    parameters.Add("TotalBuy", totalBuy);

                    var r = db.Query<SPromoSchema>("USP_S_S_GetSchemaPromo", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        schemas = r.ToList();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return schemas;
        }

        public Document ApplySingleSchema(Document doc, string schemaId)
        {
            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            SchemaViewModel schema = null;
            if (dicPromoSchemas.ContainsKey(schemaId))
            {
                schema = dicPromoSchemas[schemaId];
            }
            else
            {
                schema = cacheService.GetCachedData<SchemaViewModel>(string.Format(PrefixCacheSchema, schemaId));
                if (schema == null)
                {
                    double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value);
                    DateTime docDate = DateTime.Parse(doc.DocDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));

                    #region Get Schema single

                    using (IDbConnection db = sPromotion.GetConnection())
                    {
                        try
                        {
                            if (db.State == ConnectionState.Closed)
                                db.Open();

                            var parameters = new DynamicParameters();
                            parameters.Add("CompanyCode", doc.UCompanyCode);
                            parameters.Add("SchemaId", schemaId);
                            var r = db.Query<SPromoSchema>("USP_S_S_PromoSchema", parameters, commandType: CommandType.StoredProcedure);
                            if (r.Any())
                            {
                                schema = _mapper.Map<SchemaViewModel>(r.FirstOrDefault());

                                //parameters.Add("CompanyCode", companyCode);
                                //parameters.Add("SchemaId", schemaId);
                                parameters.Add("StoreId", doc.StoreId);
                                parameters.Add("PromoType", 0);
                                parameters.Add("CustomerCode", doc.CardCode);
                                parameters.Add("CustomerGrp", doc.CardGroup);
                                parameters.Add("CurrentDate", docDate.ToString(StringFormatConst.SQLDateParam));
                                parameters.Add("CurrentTime", docDate.ToString(StringFormatConst.SQLTimeParam));
                                parameters.Add("IsMon", docDate.DayOfWeek == DayOfWeek.Monday ? "Y" : "''");
                                parameters.Add("IsTue", docDate.DayOfWeek == DayOfWeek.Tuesday ? "Y" : "''");
                                parameters.Add("IsWed", docDate.DayOfWeek == DayOfWeek.Wednesday ? "Y" : "''");
                                parameters.Add("IsThu", docDate.DayOfWeek == DayOfWeek.Thursday ? "Y" : "''");
                                parameters.Add("IsFri", docDate.DayOfWeek == DayOfWeek.Friday ? "Y" : "''");
                                parameters.Add("IsSat", docDate.DayOfWeek == DayOfWeek.Saturday ? "Y" : "''");
                                parameters.Add("IsSun", docDate.DayOfWeek == DayOfWeek.Sunday ? "Y" : "''");
                                parameters.Add("TotalBuy", docTotal);

                                var lines = db.Query<SPromoHeader>("USP_S_S_SchemaLinePromo", parameters, commandType: CommandType.StoredProcedure);
                                if (lines.Any())
                                {
                                    schema.PromotionLines = lines.ToList();
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            if (db.State == ConnectionState.Open)
                                db.Close();
                        }
                    }

                    #endregion

                }

                if (schema != null)
                {
                    dicPromoSchemas.Add(schemaId, schema);
                    cacheService.CacheData(schema, string.Format(PrefixCacheSchema, schemaId), timeCachePromo);
                }
            }
            if (schema != null)
            {
                var resDoc = ApplyPromoSchema(doc, schema);
                if (resDoc != null)
                {
                    doc = resDoc;

                    if (doc.PromotionApply == null)
                    {
                        doc.PromotionApply = new List<PromotionViewModel>();
                    }

                    foreach (var line in doc.DocumentLines)
                    {
                        if (!string.IsNullOrEmpty(line.UPromoCode))
                        {
                            string[] spl = line.UPromoCode.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (string Xstr in spl)
                            {
                                if (doc.PromotionApply.Where(x => x.PromoId == Xstr).SingleOrDefault() == null)
                                {
                                    if (dicPromotions.ContainsKey(Xstr))
                                    {
                                        doc.PromotionApply.Add(dicPromotions[Xstr]);
                                        doc.PromotionApply[doc.PromotionApply.Count - 1].PromoBuys.Clear();
                                        doc.PromotionApply[doc.PromotionApply.Count - 1].PromoGets.Clear();
                                        doc.PromotionApply[doc.PromotionApply.Count - 1].PromoCustomers.Clear();
                                        doc.PromotionApply[doc.PromotionApply.Count - 1].PromoStores.Clear();
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(doc.PromotionId))
                    {
                        //if (doc.PromotionApply.Where(x => x.PromoId == doc.PromotionId).SingleOrDefault() == null)
                        //{
                        //    if (dicPromotions.ContainsKey(doc.PromotionId))
                        //    {
                        //        doc.PromotionApply.Add(dicPromotions[doc.PromotionId]);
                        //    }
                        //}

                        string[] spl = doc.PromotionId.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Xstr in spl)
                        {
                            if (doc.PromotionApply.Where(x => x.PromoId == Xstr).SingleOrDefault() == null)
                            {
                                if (dicPromotions.ContainsKey(Xstr))
                                {
                                    doc.PromotionApply.Add(dicPromotions[Xstr]);
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoBuys.Clear();
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoGets.Clear();
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoCustomers.Clear();
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoStores.Clear();
                                }
                            }
                        }
                    }
                }
            }

            return doc;
        }

        public Document ApplySinglePromotion(Document doc, string promoId)
        {
            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            PromotionViewModel promo;
            if (!dicPromotions.ContainsKey(promoId))
            {
                promo = cacheService.GetCachedData<PromotionViewModel>(string.Format(PrefixCachePromo, promoId));
                if (promo == null)
                {
                    promo = this.GetPromotion(doc.UCompanyCode, promoId, out _);
                    if (promo != null)
                    {
                        cacheService.CacheData(promo, string.Format(PrefixCachePromo, promoId), timeCachePromo);
                    }
                }
                if (promo != null)
                {
                    dicPromotions.Add(promoId, promo);
                }
            }
            else
            {
                promo = dicPromotions[promoId];
            }

            if (promo == null)
            {
                return doc;
            }

            if (!string.IsNullOrEmpty(doc.PromotionCode))
            {
                if (promo.IsVoucher == null || !promo.IsVoucher.Value)
                {
                    return doc;
                }
            }

            Document result = ApplyPromo(doc, string.Empty, promo);

            if (result != null)
            {
                if (result.PromotionApply == null)
                {
                    result.PromotionApply = new List<PromotionViewModel>();
                }

                foreach (var line in result.DocumentLines)
                {
                    if (!string.IsNullOrEmpty(line.UPromoCode))
                    {
                        string[] spl = line.UPromoCode.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Xstr in spl)
                        {
                            if (result.PromotionApply.Where(x => x.PromoId == Xstr).SingleOrDefault() == null)
                            {
                                if (dicPromotions.ContainsKey(Xstr))
                                {
                                    result.PromotionApply.Add(dicPromotions[Xstr].Clone());
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoBuys.Clear();
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoGets.Clear();
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoCustomers.Clear();
                                    doc.PromotionApply[doc.PromotionApply.Count - 1].PromoStores.Clear();
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(doc.PromotionId))
                {
                    //if (result.PromotionApply.Where(x => x.PromoId == doc.PromotionId).SingleOrDefault() == null)
                    //{
                    //    if (dicPromotions.ContainsKey(doc.PromotionId))
                    //    {
                    //        result.PromotionApply.Add(dicPromotions[doc.PromotionId]);
                    //    }
                    //}

                    string[] spl = doc.PromotionId.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string Xstr in spl)
                    {
                        if (doc.PromotionApply.Where(x => x.PromoId == Xstr).SingleOrDefault() == null)
                        {
                            if (dicPromotions.ContainsKey(Xstr))
                            {
                                doc.PromotionApply.Add(dicPromotions[Xstr].Clone());
                                doc.PromotionApply[doc.PromotionApply.Count - 1].PromoBuys.Clear();
                                doc.PromotionApply[doc.PromotionApply.Count - 1].PromoGets.Clear();
                                doc.PromotionApply[doc.PromotionApply.Count - 1].PromoCustomers.Clear();
                                doc.PromotionApply[doc.PromotionApply.Count - 1].PromoStores.Clear();
                            }
                        }
                    }
                }
            }

            if (result != null && !string.IsNullOrEmpty(result.PromotionCode))
            {
                result.VoucherIsApply = true;
            }

            return result;
        }

        public List<SPromoSchema> SimulatorSchema(Document doc)
        {
            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            List<SPromoSchema> schemasRes = new List<SPromoSchema>();

            double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value);
            DateTime docDate = DateTime.Parse(doc.DocDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));
            //  apply promo schema
            List<SPromoSchema> schemas = this.CheckSchemaHeader(doc.UCompanyCode, doc.StoreId, doc.CardCode, doc.CardGroup, docTotal, docDate);
            if (schemas != null && schemas.Count > 0)
            {
                foreach (SPromoSchema schm in schemas)
                {
                    Document srcDoc = doc.Clone();

                    if (schm != null)
                    {
                        SchemaViewModel schema = null;
                        if (schm.SchemaName != "SchemaDefault")
                        {
                            if (!dicPromoSchemas.ContainsKey(schm.SchemaId))
                            {
                                schema = cacheService.GetCachedData<SchemaViewModel>(string.Format(PrefixCacheSchema, schm.SchemaId));
                                if (schema == null)
                                {
                                    schema = this.GetSchemaPromo(schm, out _);
                                }

                                if (schema != null)
                                {
                                    dicPromoSchemas.Add(schm.SchemaId, schema);
                                }
                            }
                            else
                            {
                                schema = dicPromoSchemas[schm.SchemaId];
                            }
                        }
                        else
                        {
                            schema = _mapper.Map<SchemaViewModel>(schm);
                            if (schema != null)
                            {
                                schema.PromotionLines = this.CheckPromotionsBySchema(srcDoc.UCompanyCode, srcDoc.StoreId, 0, srcDoc.CardCode, srcDoc.CardGroup, docTotal, docDate, schm.SchemaId);
                            }
                        }

                        if (schema != null)
                        {
                            var resDoc = ApplyPromoSchema(srcDoc, schema);
                            if (resDoc != null)
                            {
                                if (!string.IsNullOrEmpty(resDoc.PromotionId))
                                {
                                    schemasRes.Add(schm);
                                }
                                else
                                {
                                    foreach (var line in resDoc.DocumentLines)
                                    {
                                        if (!string.IsNullOrEmpty(line.UPromoCode))
                                        {
                                            string[] spl = line.UPromoCode.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                            if (spl.Length > 0)
                                            {
                                                schemasRes.Add(schm);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return schemasRes;
        }

        public List<SPromoHeader> SimulatorPromotions(Document doc)
        {
            Set_ReCalcGrid(ref doc);
            Set_SumTotalAmt(ref doc);

            List<SPromoHeader> promosRes = new List<SPromoHeader>();

            double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value);
            DateTime docDate = DateTime.Parse(doc.DocDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));
            List<SPromoHeader> promoHeaders = this.CheckPromotions(doc.UCompanyCode, doc.StoreId, 0, doc.CardCode, doc.CardGroup, docTotal, docDate);
            if (promoHeaders != null && promoHeaders.Count > 0)
            {
                foreach (SPromoHeader prHeader in promoHeaders)
                {
                    Document srcDoc = doc.Clone();

                    if (!string.IsNullOrEmpty(srcDoc.PromotionCode))
                    {
                        if (srcDoc.PromotionCode == prHeader.PromoId && prHeader.IsVoucher == true)
                        {
                            PromotionViewModel promo = null;
                            string promoId = prHeader.PromoId;
                            if (!dicPromotions.ContainsKey(promoId))
                            {
                                promo = cacheService.GetCachedData<PromotionViewModel>(string.Format(PrefixCachePromo, prHeader.PromoId));
                                if (promo == null)
                                {
                                    promo = this.GetPromotionViewModel(prHeader);
                                }
                                if (promo != null)
                                {
                                    dicPromotions.Add(promoId, promo);
                                }
                            }
                            else
                            {
                                promo = dicPromotions[promoId];
                            }

                            var res = ApplyPromo(srcDoc, string.Empty, promo);
                            if (res != null)
                            {
                                if (!string.IsNullOrEmpty(res.PromotionId))
                                {
                                    promosRes.Add(prHeader);
                                }
                                else
                                {
                                    foreach (var line in res.DocumentLines)
                                    {
                                        if (!string.IsNullOrEmpty(line.UPromoCode))
                                        {
                                            string[] spl = line.UPromoCode.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                            if (spl.Length > 0)
                                            {
                                                promosRes.Add(prHeader);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (prHeader.IsVoucher != true)
                    {
                        PromotionViewModel promo = null;
                        if (!dicPromotions.ContainsKey(prHeader.PromoId))
                        {
                            promo = cacheService.GetCachedData<PromotionViewModel>(string.Format(PrefixCachePromo, prHeader.PromoId));
                            if (promo == null)
                            {
                                promo = this.GetPromotionViewModel(prHeader);
                            }

                            if (promo != null)
                            {
                                dicPromotions.Add(prHeader.PromoId, promo);
                            }
                        }
                        else
                        {
                            promo = dicPromotions[prHeader.PromoId];
                        }

                        var res = ApplyPromo(srcDoc, string.Empty, promo);
                        if (res != null)
                        {
                            if (!string.IsNullOrEmpty(res.PromotionId))
                            {
                                promosRes.Add(prHeader);
                            }
                            else
                            {
                                foreach (var line in res.DocumentLines)
                                {
                                    if (!string.IsNullOrEmpty(line.UPromoCode))
                                    {
                                        string[] spl = line.UPromoCode.Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                                        if (spl.Length > 0)
                                        {
                                            promosRes.Add(prHeader);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return promosRes;
        }

        private double GetRemainingPromoQuantiy(string companyCode, string storeId, string promoId)
        {
            double qtyRemain = 999999999;
            using (IDbConnection db = sPromotion.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("PromoId", promoId);

                    var qty = db.ExecuteScalar($"USP_GetRemainingPromoQuantity", param: parameters, commandType: CommandType.StoredProcedure);

                    if (qty != null)
                    {
                        double.TryParse(qty.ToString(), out qtyRemain);
                    }
                }
                catch { }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return qtyRemain;
        }


        public GenericResult CheckInActiveGetBuy(InActivePromoViewModel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(model.PromoLineType))
                {
                    result.Message = "PromoLineType must be not null.";
                    return result;
                }

                if (!string.IsNullOrEmpty(model.PromoId))
                {
                    if (model.PromoLineType.ToLower() == "get")
                    {
                        string queryGet = $"UPDATE S_PromoGet SET InActive = '{model.InActive}', ModifiedDate = GETDATE() Where PromoId='{model.PromoId}' AND CompanyCode = '{model.CompanyCode}' AND LineNum = '{model.LineNum}'";
                        var delAffectedRows = sPromotion.Execute(queryGet, null, commandType: CommandType.Text);
                        result.Success = true;
                        result.Message = "Update InActive successfully.";
                        return result;
                    }

                    if (model.PromoLineType.ToLower() == "buy")
                    {
                        string queryBuy = $"UPDATE S_PromoBuy SET InActive = '{model.InActive}', ModifiedDate = GETDATE() Where PromoId='{model.PromoId}' AND CompanyCode = '{model.CompanyCode}' AND LineNum = '{model.LineNum}'";
                        var delAffectedRows = sPromotion.Execute(queryBuy, null, commandType: CommandType.Text);
                        result.Success = true;
                        result.Message = "Update InActive successfully.";
                        return result;
                    }

                    if (dicPromotions.ContainsKey(model.PromoId))
                    {
                        dicPromotions.Remove(model.PromoId);
                    }

                    commonService.ClearRedisCache(string.Format(PrefixCachePromo, model.PromoId), string.Empty);

                    result.Data = null;
                    result.Success = false;
                    result.Message = "Update InActive Error. ";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public Document ApplyPaymentDiscount(Document doc)
        {
            double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value);
            DateTime docDate = DateTime.Parse(doc.DocDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));

            List<SPromoHeader> promoHeaders = this.CheckPromotions(doc.UCompanyCode, doc.StoreId, PromoType.PaymentDiscountCode, doc.CardCode, doc.CardGroup, docTotal, docDate);
            if (promoHeaders != null && promoHeaders.Count > 0)
            {
                foreach (SPromoHeader prHeader in promoHeaders)
                {
                    PromotionViewModel promo = null;
                    string promoId = prHeader.PromoId;
                    if (!dicPromotions.ContainsKey(promoId))
                    {
                        promo = cacheService.GetCachedData<PromotionViewModel>(string.Format(PrefixCachePromo, prHeader.PromoId));
                        if (promo == null)
                        {
                            promo = this.GetPromotionViewModel(prHeader);
                        }
                        if (promo != null)
                        {
                            dicPromotions.Add(promoId, promo);
                        }
                    }
                    else
                    {
                        promo = dicPromotions[promoId];
                    }

                    var res = ApplyPaymentDiscount(doc, promo);
                    if (res != null)
                    {
                        doc = res;
                    }
                }
            }

            return doc;
        }

        #endregion
    }
}

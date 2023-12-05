using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Models;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace RPFO.Application.Implements
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<SLoyaltyHeader> sLoyalty;
        //private string LogPath = "C:\\RPFO.API.Log\\";
        private string txtTotalPayable;

        private Dictionary<string, LoyaltyViewModel> dicLoyalty = new Dictionary<string, LoyaltyViewModel>();
        private string PrefixCache = "LOYALTY-{0}";
        private string PrefixLucky = "LK00";
        TimeSpan timeCachePromo = TimeSpan.FromMinutes(20);
        private IResponseCacheService cacheService;

        private readonly IGenericRepository<TSalesHeader> _saleHeaderRepository;
        private readonly IGenericRepository<TSalesPayment> _salepaymentLineRepository;

        public LoyaltyService(IMapper mapper, IGenericRepository<SLoyaltyHeader> _sLoyalty, IResponseCacheService responseCacheService, IGenericRepository<TSalesHeader> saleHeaderRepository, IGenericRepository<TSalesPayment> salepaymentLineRepository, IConfiguration config)
        {
            this._mapper = mapper;
            this.sLoyalty = _sLoyalty;
            this.cacheService = responseCacheService;
            string dbName = _sLoyalty.GetConnection().Database;
            if (!string.IsNullOrEmpty(dbName))
            {
                PrefixCache = "LOYALTY-{0}-" + dbName;
            }

            _saleHeaderRepository = saleHeaderRepository;
            _salepaymentLineRepository = salepaymentLineRepository;

            PrefixLucky = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixLucky"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
        }

        public List<MLoyaltyType> GetLoyaltyTypes(out string msg)
        {
            List<MLoyaltyType> loyaltyTypes = new List<MLoyaltyType>();
            msg = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var r = db.Query<MLoyaltyType>("USP_S_M_LoyaltyType", commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        loyaltyTypes = r.ToList();
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

            return loyaltyTypes;
        }

        public List<LoyaltyHeaderViewModel> SearchLoyalty(string companyCode, string loyaltyId, int? loyaltyType, string loyaltyName, string customerType, string customerValue, DateTime? validDateFrom, DateTime? validDateTo, int? validTimeFrom, int? validTimeTo, string isMon, string isTue, string isWed, string isThu, string isFri, string isSat, string isSun, string status)
        {
            List<LoyaltyHeaderViewModel> loyalty = new List<LoyaltyHeaderViewModel>();
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("LoyaltyId", loyaltyId);
                    parameters.Add("LoyaltyType", loyaltyType);
                    parameters.Add("LoyaltyName", loyaltyName);
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
                    parameters.Add("Status", status);
                    string validDateFromStr = validDateFrom == null ? null : validDateFrom.Value.ToString(StringFormatConst.SQLDateParam);
                    string validDateToStr = validDateFrom == null ? null : validDateTo.Value.ToString(StringFormatConst.SQLDateParam);

                    //string query = $"USP_S_S_SearchLoyalty '{companyCode}','{loyaltyId}','{loyaltyType}','{loyaltyName}','{customerType}'" +
                    //    $",'{customerValue}','{validDateFromStr}','{validDateToStr}','{validTimeFrom}','{validTimeTo}','{isMon}'" +
                    //    $",'{isTue}','{isWed}','{isThu}','{isFri}','{isSat}','{isSun}','{status}'";

                    var r = db.Query<LoyaltyHeaderViewModel>("USP_S_S_SearchLoyalty", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        loyalty = r.ToList();
                    }
                }
                catch
                {

                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return loyalty;
        }

        private List<SLoyaltyHeader> GetLoyalty(string companyCode, string storeId, int loyaltyType, string customerCode, string customerGrp, double totalBuy, DateTime docDate)
        {
            List<SLoyaltyHeader> sLoyalty = new List<SLoyaltyHeader>();
            using (IDbConnection db = this.sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("storeId", storeId);
                    parameters.Add("LoyaltyType", loyaltyType);
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
                    //string query = $"USP_S_S_GetLoyaltyByType '{companyCode}', '{loyaltyType}',' {loyaltyType}', '{customerCode}', '{customerGrp}', '{docDate.ToString(StringFormatConst.SQLDateParam)}'" +
                    //$", {docDate.ToString(StringFormatConst.SQLTimeParam)}, 'Y', null, null, '', '', '', '', '{totalBuy}'";
                    //var rs = db.Query<SLoyaltyHeader>(query, null, commandType: CommandType.Text);
                    var r = db.Query<SLoyaltyHeader>("USP_S_S_GetLoyaltyByType", param: parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        sLoyalty = r.ToList();
                    }
                }
                catch { }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return sLoyalty;
        }

        public LoyaltyViewModel GetLoyalty(string companyCode, string loyaltyId, out string msg)
        {
            LoyaltyViewModel loyalty = null;
            msg = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("LoyaltyId", loyaltyId);
                    var r = db.Query<SLoyaltyHeader>("USP_S_S_LoyaltyHeader", parameters, commandType: CommandType.StoredProcedure);
                    if (r.Any())
                    {
                        loyalty = _mapper.Map<LoyaltyViewModel>(r.FirstOrDefault());

                        var cus = db.Query<SLoyaltyCustomer>("USP_S_S_LoyaltyCustomer", parameters, commandType: CommandType.StoredProcedure);
                        if (cus.Any())
                        {
                            loyalty.LoyaltyCustomers = cus.ToList();
                        }
                        else
                        {
                            loyalty.LoyaltyCustomers = new List<SLoyaltyCustomer>();
                        }
                        var store = db.Query<SLoyaltyStore>("USP_S_S_LoyaltyStore", parameters, commandType: CommandType.StoredProcedure);
                        if (store.Any())
                        {
                            loyalty.LoyaltyStores = store.ToList();
                        }
                        else
                        {
                            loyalty.LoyaltyStores = new List<SLoyaltyStore>();
                        }

                        var buy = db.Query<SLoyaltyBuy>("USP_S_S_LoyaltyBuy", parameters, commandType: CommandType.StoredProcedure);
                        if (buy.Any())
                        {
                            loyalty.LoyaltyBuy = buy.ToList();
                        }
                        else
                        {
                            loyalty.LoyaltyBuy = new List<SLoyaltyBuy>();
                        }

                        var earn = db.Query<SLoyaltyEarn>("USP_S_S_LoyaltyEarn", parameters, commandType: CommandType.StoredProcedure);
                        if (earn.Any())
                        {
                            loyalty.LoyaltyEarns = earn.ToList();
                        }
                        else
                        {
                            loyalty.LoyaltyEarns = new List<SLoyaltyEarn>();
                        }

                        var exclude = db.Query<SLoyaltyExclude>("USP_S_S_LoyaltyExclude", parameters, commandType: CommandType.StoredProcedure);
                        if (exclude.Any())
                        {
                            loyalty.LoyaltyExcludes = exclude.ToList();
                        }
                        else
                        {
                            loyalty.LoyaltyExcludes = new List<SLoyaltyExclude>();
                        }
                    }
                    else
                    {
                        msg = "Cannot find loyalty id: " + loyaltyId;
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

            if (loyalty != null)
            {
                cacheService.CacheData(loyalty, string.Format(PrefixCache, loyalty.LoyaltyId), timeCachePromo);
            }

            return loyalty;
        }

        public LoyaltyViewModel GetLoyaltyViewModel(SLoyaltyHeader loyaltyHeader, out string msg)
        {
            LoyaltyViewModel loyalty = null;
            msg = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", loyaltyHeader.CompanyCode);
                    parameters.Add("LoyaltyId", loyaltyHeader.LoyaltyId);

                    loyalty = _mapper.Map<LoyaltyViewModel>(loyaltyHeader);

                    var cus = db.Query<SLoyaltyCustomer>("USP_S_S_LoyaltyCustomer", parameters, commandType: CommandType.StoredProcedure);
                    if (cus.Any())
                    {
                        loyalty.LoyaltyCustomers = cus.ToList();
                    }
                    else
                    {
                        loyalty.LoyaltyCustomers = new List<SLoyaltyCustomer>();
                    }
                    var store = db.Query<SLoyaltyStore>("USP_S_S_LoyaltyStore", parameters, commandType: CommandType.StoredProcedure);
                    if (store.Any())
                    {
                        loyalty.LoyaltyStores = store.ToList();
                    }
                    else
                    {
                        loyalty.LoyaltyStores = new List<SLoyaltyStore>();
                    }

                    var buy = db.Query<SLoyaltyBuy>("USP_S_S_LoyaltyBuy", parameters, commandType: CommandType.StoredProcedure);
                    if (buy.Any())
                    {
                        loyalty.LoyaltyBuy = buy.ToList();
                    }
                    else
                    {
                        loyalty.LoyaltyBuy = new List<SLoyaltyBuy>();
                    }

                    var earn = db.Query<SLoyaltyEarn>("USP_S_S_LoyaltyEarn", parameters, commandType: CommandType.StoredProcedure);
                    if (earn.Any())
                    {
                        loyalty.LoyaltyEarns = earn.ToList();
                    }
                    else
                    {
                        loyalty.LoyaltyEarns = new List<SLoyaltyEarn>();
                    }

                    var exclude = db.Query<SLoyaltyExclude>("USP_S_S_LoyaltyExclude", parameters, commandType: CommandType.StoredProcedure);
                    if (exclude.Any())
                    {
                        loyalty.LoyaltyExcludes = exclude.ToList();
                    }
                    else
                    {
                        loyalty.LoyaltyExcludes = new List<SLoyaltyExclude>();
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

            //check
            if (loyalty != null)
            {
                cacheService.CacheData(loyalty, string.Format(PrefixCache, loyalty.LoyaltyId), timeCachePromo);
            }

            return loyalty;
        }

        public bool InsertUpdateLoyalty(LoyaltyViewModel loyalty, out string loyaltyId, out string msg)
        {
            bool result = false;
            msg = "";
            loyaltyId = "";
            var model = loyalty.ToJson();
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    if (string.IsNullOrEmpty(loyalty.LoyaltyId))
                    {
                        string key = sLoyalty.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('LT','{loyalty.CompanyCode}', '')", null, commandType: CommandType.Text).ToString();
                        loyalty.LoyaltyId = key;
                    }

                    var checkName = db.Query<SLoyaltyHeader>($"SELECT LoyaltyName FROM S_LoyaltyHeader WHERE LoyaltyName = N'{loyalty.LoyaltyName}' AND LoyaltyId <> '{loyalty.LoyaltyId}'", commandType: CommandType.Text);

                    if (checkName.Count() > 0)
                    {
                        result = false;
                        msg += "Name of loyalty has existed.";

                        return result;
                    }

                    string validDateFrom = loyalty.ValidDateFrom == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : loyalty.ValidDateFrom.Value.ToString(StringFormatConst.SQLDateParam);
                    string validDateTo = loyalty.ValidDateTo == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : loyalty.ValidDateTo.Value.ToString(StringFormatConst.SQLDateParam);

                    //  check lucky draw
                    //if (loyalty.LoyaltyType != null && loyalty.LoyaltyType.Value == LoyaltyType.LuckyDrawCode && loyalty.Status == "Y")
                    //{
                    //    string luckyDrawValid = $"SELECT LoyaltyId, LoyaltyName FROM S_LoyaltyHeader WHERE LoyaltyType = {LoyaltyType.LuckyDrawCode} AND [Status] = 'Y' AND (('{validDateFrom}' >= ValidDateFrom AND '{validDateFrom}' <= ValidDateTo) OR ('{validDateTo}' >= ValidDateFrom AND '{validDateTo}' <= ValidDateTo))";

                    //    var checkLucky = db.Query<SLoyaltyHeader>(luckyDrawValid, commandType: CommandType.Text);
                    //    if (checkLucky.Count() > 0)
                    //    {
                    //        result = false;
                    //        msg += "There exists a lucky draw in the same time period.";

                    //        return result;
                    //    }
                    //}

                    DataTable dtHeader = CreateTableLoyaltyHeader();
                    DataRow drHeader = dtHeader.NewRow();

                    drHeader["LoyaltyId"] = loyalty.LoyaltyId;
                    drHeader["CompanyCode"] = loyalty.CompanyCode;
                    drHeader["LoyaltyType"] = loyalty.LoyaltyType;
                    drHeader["LoyaltyName"] = loyalty.LoyaltyName;
                    drHeader["CustomerType"] = loyalty.CustomerType;
                    drHeader["ValidDateFrom"] = validDateFrom;
                    drHeader["ValidDateTo"] = validDateTo;
                    drHeader["ValidTimeFrom"] = loyalty.ValidTimeFrom ?? 0;
                    drHeader["ValidTimeTo"] = loyalty.ValidTimeTo ?? 2359;
                    drHeader["IsMon"] = loyalty.IsMon;
                    drHeader["IsTue"] = loyalty.IsTue;
                    drHeader["IsWed"] = loyalty.IsWed;
                    drHeader["IsThu"] = loyalty.IsThu;
                    drHeader["IsFri"] = loyalty.IsFri;
                    drHeader["IsSat"] = loyalty.IsSat;
                    drHeader["IsSun"] = loyalty.IsSun;
                    drHeader["TotalBuyFrom"] = loyalty.TotalBuyFrom;
                    drHeader["TotalBuyTo"] = loyalty.TotalBuyTo;
                    drHeader["TotalEarnType"] = loyalty.TotalEarnType;
                    drHeader["TotalEarnValue"] = loyalty.TotalEarnValue;
                    drHeader["MaxTotalEarnValue"] = loyalty.MaxTotalEarnValue;
                    drHeader["CreatedBy"] = loyalty.CreatedBy;
                    drHeader["CreatedOn"] = DateTime.Now.ToString(StringFormatConst.SQLDateTimeParam);
                    drHeader["ModifiedBy"] = loyalty.ModifiedBy;
                    drHeader["ModifiedOn"] = DateTime.Now.ToString(StringFormatConst.SQLDateTimeParam);
                    drHeader["Status"] = loyalty.Status;

                    dtHeader.Rows.Add(drHeader);

                    //  USP_I_S_LoyaltyHeader
                    //  USP_D_S_LoyaltyContent
                    //  USP_I_S_LoyaltyCustomer
                    //  USP_I_S_LoyaltyStore
                    //  USP_I_S_LoyaltyEarn

                    DataTable dtLoyaltyCustomer = CreateTableLoyaltyCustomer();

                    //  insert customer
                    foreach (SLoyaltyCustomer customer in loyalty.LoyaltyCustomers)
                    {
                        DataRow dr = dtLoyaltyCustomer.NewRow();

                        dr["LoyaltyId"] = loyalty.LoyaltyId;
                        dr["CompanyCode"] = loyalty.CompanyCode;
                        dr["LineNum"] = customer.LineNum;
                        dr["CustomerValue"] = customer.CustomerValue;
                        dr["CustomerType"] = customer.CustomerType;

                        dtLoyaltyCustomer.Rows.Add(dr);
                    }

                    DataTable dtLoyaltyStore = CreateTablePromoStore();

                    foreach (SLoyaltyStore store in loyalty.LoyaltyStores)
                    {
                        DataRow dr = dtLoyaltyStore.NewRow();

                        dr["LoyaltyId"] = loyalty.LoyaltyId;
                        dr["CompanyCode"] = loyalty.CompanyCode;
                        dr["LineNum"] = store.LineNum;
                        dr["StoreValue"] = store.StoreValue;

                        dtLoyaltyStore.Rows.Add(dr);
                    }

                    DataTable dtLoayaltyBuy = CreateTablePromoBuy();
                    int lineNum = 1;
                    //  insert Loyalty buy
                    foreach (SLoyaltyBuy loyaltyBuy in loyalty.LoyaltyBuy)
                    {
                        if (loyaltyBuy.LineNum == 0)
                        {
                            loyaltyBuy.LineNum = lineNum++;
                        }

                        DataRow drLoyaltyBuy = dtLoayaltyBuy.NewRow();
                        drLoyaltyBuy["LoyaltyId"] = loyalty.LoyaltyId;
                        drLoyaltyBuy["CompanyCode"] = loyalty.CompanyCode;
                        drLoyaltyBuy["LineNum"] = loyaltyBuy.LineNum;
                        drLoyaltyBuy["LineType"] = loyaltyBuy.LineType;
                        drLoyaltyBuy["LineCode"] = loyaltyBuy.LineCode;
                        drLoyaltyBuy["LineName"] = loyaltyBuy.LineName;
                        drLoyaltyBuy["LineUom"] = loyaltyBuy.LineUom;
                        drLoyaltyBuy["ValueType"] = loyaltyBuy.ValueType;
                        drLoyaltyBuy["Condition_1"] = loyaltyBuy.Condition1;
                        drLoyaltyBuy["Value_1"] = loyaltyBuy.Value1;
                        drLoyaltyBuy["Condition_2"] = loyaltyBuy.Condition2;
                        drLoyaltyBuy["Value_2"] = loyaltyBuy.Value2;

                        dtLoayaltyBuy.Rows.Add(drLoyaltyBuy);
                    }

                    DataTable dtLoayaltyEarn = CreateTableLoyaltyEarn();
                    lineNum = 1;
                    //  insert Loyalty earn
                    foreach (SLoyaltyEarn loyaltyEarn in loyalty.LoyaltyEarns)
                    {
                        if (loyaltyEarn.LineNum == 0)
                        {
                            loyaltyEarn.LineNum = lineNum;
                            lineNum++;
                        }

                        DataRow dr = dtLoayaltyEarn.NewRow();

                        dr["LoyaltyId"] = loyalty.LoyaltyId;
                        dr["CompanyCode"] = loyalty.CompanyCode;
                        dr["LineNum"] = loyaltyEarn.LineNum;
                        dr["LineType"] = loyaltyEarn.LineType;
                        dr["LineCode"] = loyaltyEarn.LineCode;
                        dr["LineName"] = loyaltyEarn.LineName;
                        dr["LineUom"] = loyaltyEarn.LineUom;
                        dr["ConditionType"] = loyaltyEarn.ConditionType;
                        dr["Condition_1"] = loyaltyEarn.Condition1;
                        dr["Value_1"] = loyaltyEarn.Value1;
                        dr["Condition_2"] = loyaltyEarn.Condition2;
                        dr["Value_2"] = loyaltyEarn.Value2;
                        dr["ValueType"] = loyaltyEarn.ValueType;
                        dr["EarnValue"] = loyaltyEarn.EarnValue;
                        dr["MaxPointApply"] = loyaltyEarn.MaxPointApply;

                        dtLoayaltyEarn.Rows.Add(dr);
                    }

                    DataTable dtLoyaltyExclude = CreateTableLoyaltyExclude();

                    foreach (SLoyaltyExclude exclude in loyalty.LoyaltyExcludes)
                    {
                        DataRow dr = dtLoyaltyExclude.NewRow();

                        dr["LoyaltyId"] = loyalty.LoyaltyId;
                        dr["CompanyCode"] = loyalty.CompanyCode;
                        dr["LineType"] = exclude.LineType;
                        dr["LineCode"] = exclude.LineCode;
                        dr["LineName"] = exclude.LineName;
                        dr["LineUom"] = exclude.LineName;

                        dtLoyaltyExclude.Rows.Add(dr);
                    }

                    string userId = string.IsNullOrEmpty(loyalty.ModifiedBy) ? loyalty.CreatedBy : loyalty.ModifiedBy;
                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", userId);
                    parameters.Add("S_LoyaltyHeader", dtHeader.AsTableValuedParameter());
                    parameters.Add("S_LoyaltyCustomer", dtLoyaltyCustomer.AsTableValuedParameter());
                    parameters.Add("S_LoyaltyStore", dtLoyaltyStore.AsTableValuedParameter());
                    parameters.Add("S_LoyaltyBuy", dtLoayaltyBuy.AsTableValuedParameter());
                    parameters.Add("S_LoyaltyEarn", dtLoayaltyEarn.AsTableValuedParameter());
                    parameters.Add("S_LoyaltyExclude", dtLoyaltyExclude.AsTableValuedParameter());

                    var res = db.Query("USP_IU_S_Loyalty", param: parameters, commandType: CommandType.StoredProcedure);
                    if (res.Any() && res.Count() > 0)
                    {
                        DataTable dataTable = res.ToDataTable();
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            string errorCode = dataTable.Rows[0]["ErrCode"].ToString();
                            if (errorCode == "0")
                            {
                                loyaltyId = loyalty.LoyaltyId;
                                result = true;
                                msg += dataTable.Rows[0]["ErrMsg"].ToString();
                            }
                            else
                            {
                                loyaltyId = loyalty.LoyaltyId;
                                result = false;
                                msg += $"{errorCode} - {dataTable.Rows[0]["ErrMsg"]}";
                            }
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

            if (result && !string.IsNullOrEmpty(loyalty.LoyaltyId))
            {
                if (dicLoyalty.ContainsKey(loyalty.LoyaltyId))
                {
                    dicLoyalty[loyalty.LoyaltyId] = loyalty;
                }
                else
                {
                    dicLoyalty.Add(loyalty.LoyaltyId, loyalty);
                }

                cacheService.CacheData(loyalty, string.Format(PrefixCache, loyalty.LoyaltyId), timeCachePromo);
            }

            return result;
        }

        //public bool InsertUpdateLoyalty(LoyaltyViewModel loyalty, out string msg)
        //{
        //    bool result = false;
        //    msg = "";
        //    using (IDbConnection db = sLoyalty.GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();

        //            using (var trans = db.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var parameters = new DynamicParameters();

        //                    if (string.IsNullOrEmpty(loyalty.LoyaltyId))
        //                    {
        //                        string key = sLoyalty.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('LT','{loyalty.CompanyCode}', '')", null, commandType: CommandType.Text).ToString();
        //                        loyalty.LoyaltyId = key;
        //                    }

        //                    string validDateFrom = loyalty.ValidDateFrom == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : loyalty.ValidDateFrom.Value.ToString(StringFormatConst.SQLDateParam);
        //                    string validDateTo = loyalty.ValidDateTo == null ? DateTime.Now.ToString(StringFormatConst.SQLDateParam) : loyalty.ValidDateTo.Value.ToString(StringFormatConst.SQLDateParam);

        //                    //  check lucky draw
        //                    if (loyalty.LoyaltyType != null && loyalty.LoyaltyType.Value == LoyaltyType.LuckyDrawCode && loyalty.Status == "Y")
        //                    {
        //                        string luckyDrawValid = $"SELECT LoyaltyId, LoyaltyName FROM S_LoyaltyHeader WHERE LoyaltyType = {LoyaltyType.LuckyDrawCode} AND [Status] = 'Y' AND (('{validDateFrom}' >= ValidDateFrom AND '{validDateFrom}' <= ValidDateTo) OR ('{validDateTo}' >= ValidDateFrom AND '{validDateTo}' <= ValidDateTo))";

        //                        var checkLucky = db.Query<SLoyaltyHeader>(luckyDrawValid, null, transaction: trans, commandType: CommandType.Text);
        //                        if (checkLucky.Count() > 0)
        //                        {
        //                            result = false;
        //                            trans.Rollback();
        //                            msg += "There exists a lucky draw in the same time period.";

        //                            return result;
        //                        }
        //                    }

        //                    parameters.Add("LoyaltyId", loyalty.LoyaltyId);
        //                    parameters.Add("CompanyCode", loyalty.CompanyCode);
        //                    parameters.Add("LoyaltyType", loyalty.LoyaltyType);
        //                    parameters.Add("LoyaltyName", loyalty.LoyaltyName);
        //                    parameters.Add("CustomerType", loyalty.CustomerType);
        //                    parameters.Add("ValidDateFrom", validDateFrom);
        //                    parameters.Add("ValidDateTo", validDateTo);
        //                    parameters.Add("ValidTimeFrom", loyalty.ValidTimeFrom ?? 0);
        //                    parameters.Add("ValidTimeTo", loyalty.ValidTimeTo ?? 2359);
        //                    parameters.Add("IsMon", loyalty.IsMon);
        //                    parameters.Add("IsTue", loyalty.IsTue);
        //                    parameters.Add("IsWed", loyalty.IsWed);
        //                    parameters.Add("IsThu", loyalty.IsThu);
        //                    parameters.Add("IsFri", loyalty.IsFri);
        //                    parameters.Add("IsSat", loyalty.IsSat);
        //                    parameters.Add("IsSun", loyalty.IsSun);
        //                    parameters.Add("TotalBuyFrom", loyalty.TotalBuyFrom);
        //                    parameters.Add("TotalBuyTo", loyalty.TotalBuyTo);
        //                    parameters.Add("TotalEarnType", loyalty.TotalEarnType);
        //                    parameters.Add("TotalEarnValue", loyalty.TotalEarnValue);
        //                    parameters.Add("MaxTotalEarnValue", loyalty.MaxTotalEarnValue);
        //                    parameters.Add("Status", loyalty.Status);

        //                    var paramCheck = new DynamicParameters();
        //                    paramCheck.Add("LoyaltyId", loyalty.LoyaltyId);
        //                    paramCheck.Add("CompanyCode", loyalty.CompanyCode);
        //                    var r = db.Query<SLoyaltyHeader>("USP_S_S_LoyaltyHeader", paramCheck, transaction: trans, commandType: CommandType.StoredProcedure);
        //                    if (r.Any() && r.Count() > 0)
        //                    {
        //                        parameters.Add("ModifiedBy", loyalty.ModifiedBy);
        //                        db.Execute("USP_U_S_LoyaltyHeader", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);
        //                    }
        //                    else
        //                    {
        //                        var checkName = db.Query<SLoyaltyHeader>($"SELECT LoyaltyName FROM S_LoyaltyHeader WHERE LoyaltyName = N'{loyalty.LoyaltyName}'", null, transaction: trans, commandType: CommandType.Text);

        //                        if (checkName.Count() > 0)
        //                        {
        //                            result = false;
        //                            trans.Rollback();
        //                            msg += "Name of loyalty has existed.";

        //                            return result;
        //                        }
        //                        //  insert Loyalty header
        //                        parameters.Add("CreatedBy", loyalty.CreatedBy);
        //                        db.Execute("USP_I_S_LoyaltyHeader", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);
        //                    }

        //                    //  delete content for Loyalty
        //                    db.Execute("USP_D_S_LoyaltyContent", paramCheck, transaction: trans, commandType: CommandType.StoredProcedure);

        //                    //  insert customer
        //                    foreach (SLoyaltyCustomer customer in loyalty.LoyaltyCustomers)
        //                    {
        //                        parameters = new DynamicParameters();
        //                        parameters.Add("LoyaltyId", loyalty.LoyaltyId);
        //                        parameters.Add("CompanyCode", loyalty.CompanyCode);
        //                        parameters.Add("LineNum", customer.LineNum);
        //                        parameters.Add("CustomerValue", customer.CustomerValue);
        //                        parameters.Add("CustomerType", customer.CustomerType);

        //                        db.Execute("USP_I_S_LoyaltyCustomer", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);
        //                    }
        //                    foreach (SLoyaltyStore store in loyalty.LoyaltyStores)
        //                    {
        //                        parameters = new DynamicParameters();
        //                        parameters.Add("LoyaltyId", loyalty.LoyaltyId);
        //                        parameters.Add("CompanyCode", loyalty.CompanyCode);
        //                        parameters.Add("LineNum", store.LineNum);
        //                        parameters.Add("StoreValue", store.StoreValue);
        //                        db.Execute("USP_I_S_LoyaltyStore", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);
        //                    }

        //                    int lineNumEarn = 1;
        //                    //  insert Loyalty earn
        //                    foreach (SLoyaltyEarn loyaltyEarn in loyalty.LoyaltyEarns)
        //                    {
        //                        if (loyaltyEarn.LineNum == 0)
        //                        {
        //                            loyaltyEarn.LineNum = lineNumEarn;
        //                            lineNumEarn++;
        //                        }
        //                        parameters = new DynamicParameters();
        //                        parameters.Add("LoyaltyId", loyalty.LoyaltyId);
        //                        parameters.Add("CompanyCode", loyalty.CompanyCode);
        //                        parameters.Add("LineNum", loyaltyEarn.LineNum);
        //                        parameters.Add("LineType", loyaltyEarn.LineType);
        //                        parameters.Add("LineCode", loyaltyEarn.LineCode);
        //                        parameters.Add("LineName", loyaltyEarn.LineName);
        //                        parameters.Add("LineUom", loyaltyEarn.LineUom);
        //                        parameters.Add("ConditionType", loyaltyEarn.ConditionType);
        //                        parameters.Add("Condition_1", loyaltyEarn.Condition1);
        //                        parameters.Add("Value_1", loyaltyEarn.Value1);
        //                        parameters.Add("Condition_2", loyaltyEarn.Condition2);
        //                        parameters.Add("Value_2", loyaltyEarn.Value2);
        //                        parameters.Add("ValueType", loyaltyEarn.ValueType);
        //                        parameters.Add("EarnValue", loyaltyEarn.EarnValue);
        //                        parameters.Add("MaxPointApply", loyaltyEarn.MaxPointApply);

        //                        db.Execute("USP_I_S_LoyaltyEarn", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);
        //                    }

        //                    trans.Commit();
        //                    result = true;
        //                    msg += loyalty.LoyaltyId;
        //                }
        //                catch (Exception ex)
        //                {
        //                    result = false;
        //                    trans.Rollback();
        //                    msg += "Transaction Exception: " + ex.Message;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result = false;
        //            msg += "Exception: " + ex.Message;
        //        }
        //        finally
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //        }
        //    }

        //    if (result && !string.IsNullOrEmpty(loyalty.LoyaltyId))
        //    {
        //        if (dicLoyalty.ContainsKey(loyalty.LoyaltyId))
        //        {
        //            dicLoyalty[loyalty.LoyaltyId] = loyalty;
        //        }
        //        else
        //        {
        //            dicLoyalty.Add(loyalty.LoyaltyId, loyalty);
        //        }

        //        cacheService.CacheData(loyalty, string.Format(PrefixCache, loyalty.LoyaltyId), timeCachePromo);
        //    }

        //    return result;
        //}

        public double ApplyLoyalty(Document srcDoc, out string message)
        {
            //RPFO.Utilities.Helpers.LogUtils.WriteLogData(LogPath, "Loyalty_Farmer", "ApplyLoyalty", srcDoc.ToJson());

            message = "";
            Set_ReCalc(ref srcDoc);

            //double docTotal = 0;
            //List<SLoyaltyExclude> loyaltyExcludes = this.GetLoyaltyExcludes(srcDoc.UCompanyCode);
            //if (loyaltyExcludes != null && loyaltyExcludes.Count > 0)
            //{
            //    foreach (DocumentLine docLine in srcDoc.DocumentLines)
            //    {
            //        var check = loyaltyExcludes.Where(x => x.LineCode == docLine.ItemCode || x.LineCode == docLine.ItemGroup || x.LineCode == docLine.UoMCode);
            //        if (check.Any())
            //        {
            //            docLine.UCheckPromo = "XX";
            //        }
            //        else
            //        {
            //            docTotal += docLine.LineTotal.Value;
            //        }
            //    }

            //    docTotal -= (srcDoc.UDiscountAmount ?? 0);
            //}
            //else
            //{
            //    docTotal = srcDoc.DocumentLines.Sum(l => l.LineTotal.Value) - (srcDoc.UDiscountAmount ?? 0);
            //}

            double docTotal = srcDoc.DocumentLines.Sum(l => l.LineTotal.Value) - (srcDoc.UDiscountAmount ?? 0);
            double point = 0.0;

            if (srcDoc.DocumentStatus == "CANCELED")
            {
                point = ApplyFactorAndRounding(srcDoc, docTotal, point, out string msg);
                return point;
            }

            DateTime docDate = DateTime.Parse(srcDoc.DocDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));

            List<SLoyaltyHeader> loyaltyHeaders = this.GetLoyalty(srcDoc.UCompanyCode, srcDoc.StoreId, 0, srcDoc.CardCode, srcDoc.CardGroup, Math.Abs(docTotal), docDate);
            if (loyaltyHeaders != null && loyaltyHeaders.Count > 0)
            {
                double totalAmount = 0;
                foreach (SLoyaltyHeader loyaltyHeader in loyaltyHeaders)
                {
                    //  bỏ qua type lucky draw
                    if (loyaltyHeader.LoyaltyType == LoyaltyType.LuckyDrawCode)
                    {
                        continue;
                    }

                    LoyaltyViewModel loyaltyView = null;
                    if (dicLoyalty.ContainsKey(loyaltyHeader.LoyaltyId))
                    {
                        loyaltyView = dicLoyalty[loyaltyHeader.LoyaltyId];
                    }
                    else
                    {
                        //check
                        loyaltyView = cacheService.GetCachedData<LoyaltyViewModel>(string.Format(PrefixCache, loyaltyHeader.LoyaltyId));
                        if (loyaltyView == null)
                        {
                            loyaltyView = this.GetLoyaltyViewModel(loyaltyHeader, out string _);
                        }

                        if (loyaltyView != null)
                        {
                            dicLoyalty.Add(loyaltyView.LoyaltyId, loyaltyView);
                        }
                    }

                    if (loyaltyView != null)
                    {
                        double docTotalCheck = 0;
                        List<SLoyaltyExclude> loyaltyExcludes = loyaltyView.LoyaltyExcludes;
                        if (loyaltyExcludes != null && loyaltyExcludes.Count > 0)
                        {
                            foreach (DocumentLine docLine in srcDoc.DocumentLines)
                            {
                                var check = loyaltyExcludes.Where(x => (x.LineCode == docLine.ItemCode && x.LineUom == docLine.UoMCode) || x.LineCode == docLine.ItemGroup || x.LineCode == docLine.UoMCode);
                                if (check.Any())
                                {
                                    docLine.UCheckPromo = "XX";
                                }
                                else
                                {
                                    docTotalCheck += docLine.LineTotal.Value;
                                }
                            }

                            docTotalCheck -= (srcDoc.UDiscountAmount ?? 0);
                        }
                        else
                        {
                            docTotalCheck = docTotal;
                        }

                        if (Math.Abs(docTotalCheck) >= (loyaltyView.TotalBuyFrom ?? 0)
                            && ((loyaltyView.TotalBuyTo ?? 0) == 0 || Math.Abs(docTotalCheck) <= loyaltyView.TotalBuyTo.Value))
                        {
                            point += ApplyLoyalty(srcDoc, loyaltyView);
                            totalAmount += docTotalCheck;
                        }
                    }
                }

                point = ApplyFactorAndRounding(srcDoc, totalAmount, point, out string msg);
                message = msg;
            }

            return point;
        }

        private double ApplyLoyalty(Document doc, LoyaltyViewModel loyaltyView)
        {
            double result = 0.0;
            switch (loyaltyView.LoyaltyType.Value)
            {
                case LoyaltyType.BasedProductsCode:
                    result += Apply_Loyalty_BasedOnProducts(doc, loyaltyView);
                    break;

                case LoyaltyType.BasedSpendingCode:
                    result += Apply_Loyalty_BasedOnSpending(doc, loyaltyView);
                    break;
            }
            return result;
        }

        //private double Get_Loyalty_BasedOnProducts(Document doc, double docTotal, DateTime docDate)
        //{
        //    double result = 0.0;
        //    List<SLoyaltyHeader> loyaltyHeaders = this.GetLoyalty(doc.UCompanyCode, doc.StoreId, LoyaltyType.BasedProductsCode, doc.CardCode, doc.CardGroup, docTotal, docDate);

        //    if (loyaltyHeaders != null && loyaltyHeaders.Count > 0)
        //    {
        //        foreach (SLoyaltyHeader ltHeader in loyaltyHeaders)
        //        {
        //            LoyaltyViewModel loyalty;
        //            if (dicLoyalty.ContainsKey(ltHeader.LoyaltyId))
        //            {
        //                loyalty = dicLoyalty[ltHeader.LoyaltyId];
        //            }
        //            else
        //            {
        //                loyalty = this.GetLoyalty(doc.UCompanyCode, ltHeader.LoyaltyId, out string _);
        //            }

        //            result += Apply_Loyalty_BasedOnProducts(doc, loyalty);
        //        }
        //    }
        //    else
        //    {

        //    }
        //    return result;
        //}

        private double Apply_Loyalty_BasedOnProducts(Document doc, LoyaltyViewModel loyalty)
        {
            double point = 0.0;

            //  apply loyalty
            foreach (DocumentLine docLine in doc.DocumentLines)
            {
                if (docLine.UCheckPromo == "XX")
                {
                    continue;
                }

                foreach (SLoyaltyEarn earnLine in loyalty.LoyaltyEarns)
                {
                    if (earnLine.LineType == LoyaltyLineType.ItemCode)
                    {
                        if (docLine.ItemCode == earnLine.LineCode && docLine.UoMCode == earnLine.LineUom)
                        {
                            point += Apply_Loyalty_BasedOnProducts_Content(docLine, earnLine);
                        }
                    }
                    else if (earnLine.LineType == LoyaltyLineType.BarCode)
                    {
                        if (docLine.BarCode == earnLine.LineCode)
                        {
                            point += Apply_Loyalty_BasedOnProducts_Content(docLine, earnLine);
                        }
                    }
                    else if (earnLine.LineType == LoyaltyLineType.ItemGroup)
                    {
                        if (docLine.ItemGroup == earnLine.LineCode)
                        {
                            point += Apply_Loyalty_BasedOnProducts_Content(docLine, earnLine);
                        }
                    }
                    else if (earnLine.LineType == LoyaltyLineType.Collection)
                    {
                        if (docLine.UCollection == earnLine.LineCode)
                        {
                            point += Apply_Loyalty_BasedOnProducts_Content(docLine, earnLine);
                        }
                    }
                }
            }

            return point;
        }

        private double Apply_Loyalty_BasedOnProducts_Content(DocumentLine docLine, SLoyaltyEarn loyaltyLine)
        {
            double result = 0.0;
            if (loyaltyLine.ConditionType == LoyaltyCondition.Quantity)
            {
                double quantity = Math.Abs(docLine.Quantity.Value);
                if (loyaltyLine.Condition1 == LoyaltyCondition.CE)
                {
                    if (loyaltyLine.Value1.Value <= quantity)
                    {
                        int MultiRate = Convert.ToInt32(docLine.Quantity.Value) / Convert.ToInt32(loyaltyLine.Value1.Value);
                        if (loyaltyLine.ValueType == LoyaltyValueType.FixedPoint)
                        {
                            result = loyaltyLine.EarnValue.Value * MultiRate;
                        }
                        else if (loyaltyLine.ValueType == LoyaltyValueType.PercentAmount)
                        {
                            result = docLine.LineTotal.Value * loyaltyLine.EarnValue.Value / 100;
                        }
                    }
                }
                else if (loyaltyLine.Condition1 == PromoCondition.FROM && loyaltyLine.Condition2 == PromoCondition.TO)
                {
                    if (quantity >= loyaltyLine.Value1.Value
                        && (loyaltyLine.Value2 == null || loyaltyLine.Value2 == 0 || quantity <= loyaltyLine.Value2.Value))
                    {
                        if (loyaltyLine.ValueType == LoyaltyValueType.FixedPoint)
                        {
                            result = loyaltyLine.EarnValue.Value;
                            if (docLine.Quantity.Value < 0)
                            {
                                result += -1;
                            }
                        }
                        else if (loyaltyLine.ValueType == LoyaltyValueType.PercentAmount)
                        {
                            result = docLine.LineTotal.Value * loyaltyLine.EarnValue.Value / 100;
                        }
                    }
                }
            }
            else if (loyaltyLine.ConditionType == PromoCondition.Amount)
            {
                double lineTotal = Math.Abs(docLine.LineTotal.Value);
                if (loyaltyLine.Condition1 == PromoCondition.CE)
                {
                    if (loyaltyLine.Value1.Value <= lineTotal)
                    {
                        int MultiRate = Convert.ToInt32(docLine.LineTotal.Value) / Convert.ToInt32(loyaltyLine.Value1.Value);
                        if (loyaltyLine.ValueType == LoyaltyValueType.FixedPoint)
                        {
                            result = loyaltyLine.EarnValue.Value * MultiRate;
                        }
                        else if (loyaltyLine.ValueType == LoyaltyValueType.PercentAmount)
                        {
                            result = docLine.LineTotal.Value * loyaltyLine.EarnValue.Value / 100;
                        }
                    }
                }
                else if (loyaltyLine.Condition1 == PromoCondition.FROM && loyaltyLine.Condition2 == PromoCondition.TO)
                {
                    if (lineTotal >= loyaltyLine.Value1
                        && (loyaltyLine.Value2 == null || loyaltyLine.Value2 == 0 || lineTotal <= loyaltyLine.Value2))
                    {
                        if (loyaltyLine.ValueType == LoyaltyValueType.FixedPoint)
                        {
                            result = loyaltyLine.EarnValue.Value;
                            if (docLine.LineTotal.Value < 0)
                            {
                                result *= -1;
                            }
                        }
                        else if (loyaltyLine.ValueType == LoyaltyValueType.PercentAmount)
                        {
                            result = docLine.LineTotal.Value * loyaltyLine.EarnValue.Value / 100;
                        }
                    }
                }
            }

            if ((loyaltyLine.MaxPointApply ?? 0) > 0)
            {
                result = Math.Min(result, loyaltyLine.MaxPointApply.Value);
            }

            return result;
        }

        //private double Get_Loyalty_BasedOnSpending(Document doc, double docTotal, DateTime docDate)
        //{
        //    double result = 0.0;
        //    List<SLoyaltyHeader> loyaltyHeaders = this.GetLoyalty(doc.UCompanyCode, doc.StoreId, LoyaltyType.BasedSpendingCode, doc.CardCode, doc.CardGroup, docTotal, docDate);

        //    if (loyaltyHeaders != null && loyaltyHeaders.Count > 0)
        //    {
        //        foreach (SLoyaltyHeader ltHeader in loyaltyHeaders)
        //        {
        //            LoyaltyViewModel loyalty;
        //            if (dicLoyalty.ContainsKey(ltHeader.LoyaltyId))
        //            {
        //                loyalty = dicLoyalty[ltHeader.LoyaltyId];
        //            }
        //            else
        //            {
        //                loyalty = this.GetLoyalty(doc.UCompanyCode, ltHeader.LoyaltyId, out string _);
        //            }

        //            result += Apply_Loyalty_BasedOnSpending(doc, string.Empty, loyalty, doc.PromotionCode);
        //        }
        //    }

        //    return result;
        //}

        private double Apply_Loyalty_BasedOnSpending(Document doc, LoyaltyViewModel loyalty)
        {
            double result = 0.0;
            if (doc.DocumentLines.Count == 0)
            {
                return result;
            }

            // Apply Get Header
            //Set_ReCalc(ref doc);
            Set_SumTotalAmt(ref doc);

            double total = Math.Abs(Convert.ToDouble(txtTotalPayable));
            if (Convert.ToDouble(txtTotalPayable) != 0 && loyalty.TotalBuyFrom <= total
                && (loyalty.TotalBuyTo == null || loyalty.TotalBuyTo >= total))
            {
                //1 - Discount Amount
                //2 - Discount Percent
                //3 - Fixed Amount

                if (loyalty.TotalEarnType == LoyaltyValueType.FixedPoint)
                {
                    result += loyalty.TotalEarnValue.Value;
                }
                else if (loyalty.TotalEarnType == LoyaltyValueType.PercentAmount)
                {
                    double totalPayable = Convert.ToDouble(txtTotalPayable) - (doc.UDiscountAmount ?? 0);
                    double newEarn = totalPayable * loyalty.TotalEarnValue.Value / 100;
                    if ((loyalty.MaxTotalEarnValue ?? 0) > 0)
                    {
                        result += Math.Min(newEarn, loyalty.MaxTotalEarnValue.Value);
                    }
                    else
                    {
                        result += newEarn;
                    }
                }
            }

            Set_ReCalc(ref doc);
            Set_SumTotalAmt(ref doc);

            return result;
        }

        private void Set_ReCalc(ref Document doc)
        {
            foreach (DocumentLine docLine in doc.DocumentLines)
            {
                var taxRate = docLine.TaxPercentagePerRow ?? 0;
                var ItemPrice = docLine.UnitPrice ?? 0;
                var ItemQty = docLine.Quantity ?? 0;
                var ItemDisRate = docLine.DiscountPercent ?? 0;
                var total = ItemPrice * ItemQty;
                var totalWithDiscount = total - (total * ItemDisRate / 100);
                var totalWithTax = totalWithDiscount + totalWithDiscount * taxRate / 100;

                docLine.LineTotal = Math.Round(totalWithTax, doc.RoundingDigit, MidpointRounding.AwayFromZero);
                docLine.UCheckPromo = String.Empty;
            }
        }

        private void Set_SumTotalAmt(ref Document doc)
        {
            //try
            //{
            //var TotalQty = 0.0d;
            //var TotalLine = 0.0d;
            //var TotalLineDis = 0.0d;
            //double totalLineTax = 0.0d;
            double totalPayable = 0.0d;
            //double TotalDepositAmnt = 0.0d;
            //double TotalReturnAmnt = 0.0d;
            //double TotalAmtFur = 0.0d;
            //double TotalSumReceipt = 0.0d;
            //double totalWeight = 0;
            if (doc.DocumentLines.Count > 0)// dgvItem.Rows.Count > 0)
            {
                foreach (DocumentLine docLine in doc.DocumentLines)
                {
                    if (docLine.UCheckPromo == "XX")
                    {
                        continue;
                    }

                    var ItemPrice = docLine.UnitPrice ?? 0;// Convert.ToDouble(drv["citemprice"]);
                    var ItemQty = docLine.Quantity ?? 0;// Convert.ToDouble(drv["citemqty"]);
                    var ItemDisRate = docLine.DiscountPercent ?? 0;// Convert.ToDouble(drv["citemdisc"]);
                    var taxRate = docLine.TaxPercentagePerRow ?? 0;
                    //var depositAmnt = Convert.ToDouble(drv["cDepositAmnt"]);
                    var LineTotal = docLine.LineTotal ?? 0;// Convert.ToDouble(drv["csubtotal"]);
                                                           //double cbm = 0;
                                                           //if (drv["cCbm"] != null)
                                                           //{
                                                           //    double.TryParse(drv["cCbm"].ToString(), out cbm);
                                                           //}

                    var total = ItemPrice * ItemQty;
                    var discountAmt = total * ItemDisRate / 100;
                    var totalWithDiscount = total - discountAmt;
                    var taxAmt = totalWithDiscount * taxRate / 100;
                    var totalWithTax = totalWithDiscount + taxAmt;

                    //totalLineTax += taxAmt;
                    //TotalLine += total;
                    //TotalLineDis += discountAmt;
                    totalPayable += totalWithTax;


                    //if (ItemQty > 0)
                    //    TotalQty += ItemQty;
                    //if (totalWithTax < 0)
                    //    TotalReturnAmnt += totalWithTax;
                    //docLine.LineTotal = totalWithTax;
                    //docLine.UDisPrcnt = docLine.DiscountPercent;
                    //docLine.UDisAmt = discountAmt;
                    //docLine.UPriceAfDis = Math.Round(ItemPrice * (100 - ItemDisRate) / 100, doc.RoundingDigit);
                    //docLine.UTotalAfDis = totalWithDiscount;
                    //docLine.VatPerPriceAfDis = Math.Round((docLine.UPriceAfDis ?? 0) * (docLine.TaxPercentagePerRow ?? 0) / 100, doc.RoundingDigit);
                    //docLine.PriceAfDisAndVat = docLine.VatPerPriceAfDis + (docLine.UPriceAfDis ?? 0);
                }
            }
            //TotalLineDis = Math.Round(TotalLineDis / 100, 0) * 100;
            //TotalLine = Math.Round(TotalLine / 100, 0) * 100;
            totalPayable = Math.Round(totalPayable, 0);
            //totalLineTax = Math.Round(totalLineTax / 100, 0) * 100;

            txtTotalPayable = totalPayable.ToString();
        }

        public bool InsertLoyaltyLog(bool isOut, Document doc, double inPoint, double outPoint, double outAmt, out string msg)
        {
            //string text = $"IsOunt: {isOut}\nInPoint: {inPoint}\nOutPoint: {outPoint}\nOutAmt: {outAmt}\n\n{doc.ToJson()}";
            //Utilities.Helpers.LogUtils.WriteLogData(LogPath, "Loyalty_Farmer", "InsertLog", text);

            bool result = false;
            msg = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    using (var trans = db.BeginTransaction())
                    {
                        try
                        {
                            double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value) - (doc.UDiscountAmount ?? 0);
                            DateTime dtNow = DateTime.Now;
                            var parameters = new DynamicParameters();

                            parameters.Add("TransId", doc.TransId);
                            parameters.Add("CompanyCode", doc.UCompanyCode);
                            parameters.Add("StoreId", doc.StoreId);
                            parameters.Add("CustomerId", doc.CardCode);
                            parameters.Add("CustomerName", doc.CardName);
                            parameters.Add("CardNumber", doc.NumAtCard);
                            parameters.Add("TransDate", doc.DocDate.Value == null ? dtNow.ToString(StringFormatConst.SQLDateTimeParam) : doc.DocDate.Value.ToString(StringFormatConst.SQLDateTimeParam));

                            DateTime expire = new DateTime(dtNow.Year, 12, 31);
                            if (!isOut)
                            {
                                parameters.Add("TransType", "SalesOrder");
                                parameters.Add("InPoint", inPoint);
                                parameters.Add("OutPoint", 0);
                                parameters.Add("InAmt", docTotal);
                                parameters.Add("OutAmt", 0);
                            }
                            else
                            {
                                parameters.Add("TransType", "Payment");
                                parameters.Add("InPoint", 0);
                                parameters.Add("OutPoint", outPoint);
                                parameters.Add("InAmt", 0);
                                parameters.Add("OutAmt", outAmt);
                            }

                            parameters.Add("CreatedBy", doc.UCreatedBy);
                            parameters.Add("CreatedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                            parameters.Add("ModifiedBy", doc.UCreatedBy);
                            parameters.Add("ModifiedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                            parameters.Add("CalcStatus", "A");
                            parameters.Add("ExpireDate", expire.ToString(StringFormatConst.SQLDateParam));

                            //  insert Loyalty Log
                            db.Execute("USP_I_T_LoyaltyLog", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);

                            trans.Commit();
                            result = true;
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

            return result;
        }

        public double ApplyFactorAndRounding(Document doc, double totalAmount, double point, out string message)
        {
            double newPoint = Math.Round(point, 0, MidpointRounding.AwayFromZero);
            message = "";
            //  apply factor
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    //  get data apply factor
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", doc.UCompanyCode);
                    parameters.Add("CustomerId", doc.CardCode);

                    string transType = "SalesOrder";
                    if (doc.DocumentStatus == "CANCELED")
                    {
                        //  canceled
                        //  get original transaction
                        string query = $"SELECT TransId, InPoint FROM T_LoyaltyLog WHERE TransId = '{doc.OriginalRefNo}' AND CustomerId = '{doc.CardCode}' ORDER BY CreatedOn";
                        IEnumerable<TLoyaltyLog> loyaltyLog = db.Query<TLoyaltyLog>(query, commandType: CommandType.Text);
                        if (loyaltyLog.Any())
                        {
                            newPoint = loyaltyLog.FirstOrDefault().InPoint;
                            newPoint *= totalAmount / Math.Abs(totalAmount);
                        }
                        transType = "Canceled";
                    }
                    else
                    {
                        double factor = -1.0;
                        if (doc.DocumentStatus == "RETURN" || (doc.DocumentStatus == "EXCHANGE" && totalAmount < 0))
                        {
                            //  return
                            //  get original transaction
                            string query = $"SELECT TransId, InPoint, RankFactor FROM T_LoyaltyLog WHERE TransId = '{doc.OriginalRefNo}' AND CustomerId = '{doc.CardCode}' ORDER BY CreatedOn";
                            IEnumerable<TLoyaltyLog> loyaltyLog = db.Query<TLoyaltyLog>(query, commandType: CommandType.Text);
                            if (loyaltyLog.Any())
                            {
                                factor = loyaltyLog.FirstOrDefault().RankFactor;
                            }
                            transType = doc.DocumentStatus == "RETURN" ? "Return" : "Exchange";
                        }

                        if (string.IsNullOrEmpty(doc.DocumentStatus) || factor < 0 || (doc.DocumentStatus == "EXCHANGE" && totalAmount > 0))
                        {
                            var obj = db.ExecuteScalar("USP_GetLoyaltyFactorRank", param: parameters, commandType: CommandType.StoredProcedure);
                            if (obj != null)
                            {
                                double.TryParse(obj.ToString(), out factor);
                            }

                            if (doc.DocumentStatus == "EXCHANGE")
                            {
                                transType = "Exchange";
                            }
                        }

                        newPoint = Math.Round(point * factor, 0, MidpointRounding.AwayFromZero);
                    }

                    //var obj = db.ExecuteScalar("USP_GetLoyaltyFactorRank", param: parameters, commandType: CommandType.StoredProcedure);
                    //if (obj != null)
                    //{
                    //    double.TryParse(obj.ToString(), out double factor);
                    //    if (factor > 0)
                    //    {
                    //        newPoint = Math.Round(point * factor, 0, MidpointRounding.AwayFromZero);
                    //    }
                    //}

                    ////  update customer
                    //parameters.Add("Point", newPoint);
                    //db.Execute("USP_UpdateCustomerPoint", param: parameters, commandType: CommandType.StoredProcedure);
                    if (string.IsNullOrEmpty(doc.DocObjectCode) || doc.DocObjectCode != "CheckPoint")
                    {
                        //  insert loyalty log
                        using (var trans = db.BeginTransaction())
                        {
                            try
                            {
                                //double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value) - (doc.UDiscountAmount ?? 0);
                                DateTime dtNow = DateTime.Now;
                                //DateTime dtNow = doc.DocDate ?? DateTime.Now;   //check

                                parameters.Add("TransId", doc.TransId);
                                //parameters.Add("CompanyCode", doc.UCompanyCode);
                                parameters.Add("StoreId", doc.StoreId);
                                //parameters.Add("CustomerId", doc.CardCode);
                                parameters.Add("CustomerName", doc.CardName);
                                parameters.Add("CardNumber", doc.NumAtCard);
                                parameters.Add("TransDate", doc.DocDate.Value == null ? dtNow.ToString(StringFormatConst.SQLDateTimeParam) : doc.DocDate.Value.ToString(StringFormatConst.SQLDateTimeParam));

                                DateTime expire = new DateTime(dtNow.Year, 12, 31);
                                //  in amount
                                parameters.Add("TransType", transType);
                                parameters.Add("InPoint", newPoint);
                                parameters.Add("OutPoint", 0);
                                parameters.Add("InAmt", totalAmount);
                                parameters.Add("OutAmt", 0);

                                parameters.Add("CreatedBy", doc.UCreatedBy);
                                parameters.Add("CreatedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                                parameters.Add("ModifiedBy", doc.UCreatedBy);
                                parameters.Add("ModifiedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                                parameters.Add("CalcStatus", "A");
                                parameters.Add("ExpireDate", expire.ToString(StringFormatConst.SQLDateParam));

                                //string queryCheck = $"USP_I_T_LoyaltyLog '{doc.TransId}', '{doc.UCompanyCode}', '{doc.StoreId}', '{doc.CardCode}', '{doc.CardName}', '{doc.NumAtCard}', '{doc.DocDate.Value.ToString(StringFormatConst.SQLDateTimeParam)}', 'SalesOrder', '{newPoint}', '{0}', '{docTotal}', '{0}', '{doc.UCreatedBy}', '{dtNow.ToString(StringFormatConst.SQLDateTimeParam)}','{doc.UCreatedBy}', '{dtNow.ToString(StringFormatConst.SQLDateTimeParam)}', 'A', '{expire.ToString(StringFormatConst.SQLDateParam)}'";

                                //  insert Loyalty Log
                                db.Execute("USP_I_T_LoyaltyLog", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);

                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                message += "Insert Log Exception: " + ex.Message + "; ";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message += ex.Message + "; ";
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return newPoint;
        }

        //private double Rounding(string roundType, double src)
        //{
        //    double result = 0.0;
        //    if (roundType == RoundingType.NoRounding)
        //    {
        //        result = src;
        //    }
        //    else if (roundType == RoundingType.RoundToFiveHundredth)
        //    {
        //        result = Math.Round(src, 2, MidpointRounding.AwayFromZero);
        //    }
        //    else if (roundType == RoundingType.RoundToTenHundredth)
        //    {

        //    }
        //    else if (roundType == RoundingType.RoundToOne)
        //    {

        //    }
        //    else if (roundType == RoundingType.RoundToTen)
        //    {

        //    }

        //    return result;
        //}

        public double PointConvert(string companyCode, string storeId, double point)
        {
            double amount = 0;
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                var obj = db.ExecuteScalar($"USP_GetLoyaltyPointConvert '{companyCode}', '{storeId}', '{point}'", commandType: CommandType.Text);
                if (obj != null)
                {
                    double.TryParse(obj.ToString(), out amount);
                }
            }

            return amount;
        }

        public List<SLoyaltyPointConvert> GetLoyaltyPointConverts(string companyCode, string storeId)
        {
            List<SLoyaltyPointConvert> pointConverts = new List<SLoyaltyPointConvert>();
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                var r = db.Query<SLoyaltyPointConvert>($"USP_S_S_LoyaltyPointConvert '{companyCode}', '{storeId}'", commandType: CommandType.Text);
                if (r.Any())
                {
                    pointConverts = r.ToList();
                }
            }

            return pointConverts;
        }

        public bool InsertUPdateLoyaltyPointConverts(SLoyaltyPointConvert pointConvert, out string message)
        {
            bool result = false;
            message = "";
            using (IDbConnection db = sLoyalty.GetConnection())
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
                            parameters.Add("CompanyCode", pointConvert.CompanyCode);
                            parameters.Add("StoreId", pointConvert.StoreId);
                            parameters.Add("Point", pointConvert.Point);
                            parameters.Add("Amount", pointConvert.Amount);
                            parameters.Add("CreatedBy", pointConvert.CreatedBy);

                            db.Execute("USP_IU_S_LoyaltyPointConvert", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);

                            trans.Commit();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            trans.Rollback();
                            message += "Transaction Exception: " + ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    message += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return result;
        }

        //public List<SLoyaltyExclude> GetLoyaltyExcludes(string companyCode)
        //{
        //    List<SLoyaltyExclude> loyaltyExcludes = new List<SLoyaltyExclude>();
        //    using (IDbConnection db = sLoyalty.GetConnection())
        //    {
        //        var r = db.Query<SLoyaltyExclude>($"USP_S_S_LoyaltyExclude '{companyCode}'", commandType: CommandType.Text);
        //        if (r.Any())
        //        {
        //            loyaltyExcludes = r.ToList();
        //        }
        //    }

        //    return loyaltyExcludes;
        //}

        //public bool InsertLoyaltyExclude(string companyCode, string lineType, string lineCode, string lineName, out string message)
        //{
        //    bool result = false;
        //    message = "";
        //    using (IDbConnection db = sLoyalty.GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();

        //            using (var trans = db.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var parameters = new DynamicParameters();
        //                    parameters.Add("CompanyCode", companyCode);
        //                    parameters.Add("LineType", lineType);
        //                    parameters.Add("LineCode", lineCode);
        //                    parameters.Add("LineName", lineName);

        //                    db.Execute("USP_I_S_LoyaltyExclude", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);

        //                    trans.Commit();
        //                    result = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    result = false;
        //                    trans.Rollback();
        //                    message += "Transaction Exception: " + ex.Message;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result = false;
        //            message += "Exception: " + ex.Message;
        //        }
        //        finally
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //        }
        //    }

        //    return result;
        //}

        //public bool DeleteLoyaltyExclude(string companyCode, string lineType, string lineCode, out string message)
        //{
        //    bool result = false;
        //    message = "";
        //    using (IDbConnection db = sLoyalty.GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();

        //            using (var trans = db.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var parameters = new DynamicParameters();
        //                    parameters.Add("CompanyCode", companyCode);
        //                    parameters.Add("LineType", lineType);
        //                    parameters.Add("LineCode", lineCode);

        //                    db.Execute("USP_D_S_LoyaltyExclude", param: parameters, transaction: trans, commandType: CommandType.StoredProcedure);

        //                    trans.Commit();
        //                    result = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    result = false;
        //                    trans.Rollback();
        //                    message += "Transaction Exception: " + ex.Message;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result = false;
        //            message += "Exception: " + ex.Message;
        //        }
        //        finally
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //        }
        //    }

        //    return result;
        //}

        public LoyaltyViewModel GetLuckyNo(Document srcDoc, out string message)
        {
            //RPFO.Utilities.Helpers.LogUtils.WriteLogData(LogPath, "Loyalty_Servay", "GetLuckyNo", srcDoc.ToJson());

            message = "";
            Set_ReCalc(ref srcDoc);
            LoyaltyViewModel loyaltyView = null;

            try
            {
                double docTotal = srcDoc.DocumentLines.Sum(l => l.LineTotal.Value) - (srcDoc.UDiscountAmount ?? 0);
                if (docTotal <= 0)
                {
                    return loyaltyView;
                }
                DateTime docDate = DateTime.Parse(srcDoc.DocDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));
                List<SLoyaltyHeader> loyaltyHeaders = this.GetLoyalty(srcDoc.UCompanyCode, srcDoc.StoreId, LoyaltyType.LuckyDrawCode, srcDoc.CardCode, srcDoc.CardGroup, docTotal, docDate);
                //if (loyaltyHeaders != null && loyaltyHeaders.Count > 0)
                //{
                //    using (IDbConnection db = sLoyalty.GetConnection())
                //    {
                //        string query = $"SELECT dbo.[fnc_AutoGenDocumentCode] ('LU', N'{srcDoc.UCompanyCode}',  N'{srcDoc.StoreId}')";
                //        var result = db.ExecuteScalar(query, commandType: CommandType.Text);
                //        if (result != null)
                //        {
                //            luckyNo = result.ToString();
                //        }
                //    }
                //}
                if (loyaltyHeaders != null && loyaltyHeaders.Count > 0)
                {
                    foreach (SLoyaltyHeader loyaltyHeader in loyaltyHeaders)
                    {
                        //  bỏ qua type lucky draw
                        if (loyaltyHeader.LoyaltyType != LoyaltyType.LuckyDrawCode)
                        {
                            continue;
                        }


                        if (dicLoyalty.ContainsKey(loyaltyHeader.LoyaltyId))
                        {
                            loyaltyView = dicLoyalty[loyaltyHeader.LoyaltyId];
                        }
                        else
                        {
                            //check
                            loyaltyView = cacheService.GetCachedData<LoyaltyViewModel>(string.Format(PrefixCache, loyaltyHeader.LoyaltyId));
                            if (loyaltyView == null)
                            {
                                loyaltyView = this.GetLoyaltyViewModel(loyaltyHeader, out string _);
                            }

                            if (loyaltyView != null)
                            {
                                dicLoyalty.Add(loyaltyView.LoyaltyId, loyaltyView);
                            }
                        }

                        if (loyaltyView != null)
                        {
                            double docTotalCheck = 0;
                            List<SLoyaltyExclude> loyaltyExcludes = loyaltyView.LoyaltyExcludes;
                            if (loyaltyExcludes != null && loyaltyExcludes.Count > 0)
                            {
                                foreach (DocumentLine docLine in srcDoc.DocumentLines)
                                {
                                    var check = loyaltyExcludes.Where(x => (x.LineCode == docLine.ItemCode && x.LineUom == docLine.UoMCode) || x.LineCode == docLine.ItemGroup || x.LineCode == docLine.UoMCode);
                                    if (check.Any())
                                    {
                                        docLine.UCheckPromo = "XX";
                                    }
                                    else
                                    {
                                        docTotalCheck += docLine.LineTotal.Value;
                                    }
                                }

                                docTotalCheck -= (srcDoc.UDiscountAmount ?? 0);
                            }
                            else
                            {
                                docTotalCheck = docTotal;
                            }

                            if (docTotalCheck >= (loyaltyView.TotalBuyFrom ?? 0)
                                && ((loyaltyView.TotalBuyTo ?? 0) == 0 || docTotalCheck <= loyaltyView.TotalBuyTo.Value))
                            {
                                ArrayList arlcheckAll = new ArrayList();
                                arlcheckAll.Add(true);
                                foreach (SLoyaltyBuy loyaltyBuy in loyaltyView.LoyaltyBuy)
                                {
                                    bool checkGrid = false;

                                    foreach (DocumentLine docLine in srcDoc.DocumentLines)
                                    {
                                        if (docLine.UCheckPromo == "LX")
                                        {
                                            continue;
                                        }

                                        if (loyaltyBuy.LineType == PromoLineType.ItemCode)
                                        {
                                            if (docLine.ItemCode == loyaltyBuy.LineCode && docLine.UoMCode == loyaltyBuy.LineUom)
                                            {
                                                if (loyaltyBuy.ValueType == PromoCondition.Quantity)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.Quantity)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    else if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.Quantity >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.Quantity <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                }
                                                else if (loyaltyBuy.ValueType == PromoCondition.Amount)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.LineTotal)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    else if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.LineTotal >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.LineTotal <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (loyaltyBuy.LineType == PromoLineType.BarCode)
                                        {
                                            if (docLine.BarCode == loyaltyBuy.LineCode)
                                            {
                                                if (loyaltyBuy.ValueType == PromoCondition.Quantity)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.Quantity)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    else if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.Quantity >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.Quantity <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                }
                                                else if (loyaltyBuy.ValueType == PromoCondition.Amount)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.LineTotal)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    else if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.LineTotal >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.LineTotal <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (loyaltyBuy.LineType == PromoLineType.ItemGroup)
                                        {
                                            if (docLine.ItemGroup == loyaltyBuy.LineCode)
                                            {
                                                if (loyaltyBuy.ValueType == PromoCondition.Quantity)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.Quantity)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    else if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.Quantity >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.Quantity <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                }
                                                else if (loyaltyBuy.ValueType == PromoCondition.Amount)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.LineTotal)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    else if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.LineTotal >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.LineTotal <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (loyaltyBuy.LineType == PromoLineType.Collection)
                                        {
                                            if (docLine.UCollection == loyaltyBuy.LineCode)
                                            {
                                                if (loyaltyBuy.ValueType == PromoCondition.Quantity)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.Quantity)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    else if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.Quantity >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.Quantity <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                }
                                                else if (loyaltyBuy.ValueType == PromoCondition.Amount)
                                                {
                                                    if (loyaltyBuy.Condition1 == PromoCondition.CE)
                                                    {
                                                        if (loyaltyBuy.Value1 <= docLine.LineTotal)
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
                                                    if (loyaltyBuy.Condition1 == PromoCondition.FROM && loyaltyBuy.Condition2 == PromoCondition.TO)
                                                    {
                                                        if (docLine.LineTotal >= loyaltyBuy.Value1
                                                            && (loyaltyBuy.Value2 == null || loyaltyBuy.Value2.Value == 0 || docLine.LineTotal <= loyaltyBuy.Value2))
                                                        {
                                                            checkGrid = true;
                                                            docLine.UCheckPromo = "LX";
                                                            break;
                                                        }
                                                    }
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
                                    { checkResult = c; }
                                    else
                                    {
                                        checkResult = c;
                                        break;
                                    }
                                }

                                if (checkResult)
                                {
                                    using (IDbConnection db = sLoyalty.GetConnection())
                                    {
                                        string query = $"SELECT dbo.[fnc_AutoGenDocumentCode] ('{PrefixLucky}', N'{loyaltyView.LoyaltyId.Substring(loyaltyView.LoyaltyId.Length - 5, 5)}',  N'{srcDoc.StoreId}')";
                                        var result = db.ExecuteScalar(query, commandType: CommandType.Text);
                                        if (result != null)
                                        {
                                            loyaltyView.LuckyNo = result.ToString();
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
            }

            return loyaltyView;
        }

        public bool InsertUpdateMemberCard(Document doc, out string message)
        {
            message = "";
            bool result = false;

            try
            {
                string companyCode = doc.UCompanyCode;
                if (string.IsNullOrEmpty(companyCode))
                {
                    message = "CompanyCode must be not null.";
                    return false;
                }

                using (IDbConnection db = sLoyalty.GetConnection())
                {
                    foreach (DocumentLine docLine in doc.DocumentLines)
                    {
                        if (docLine.ItemType.ToLower() == "member")
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("CompanyCode", companyCode);
                            parameters.Add("CardNo", docLine.SerialNum);
                            parameters.Add("StartDate", docLine.StartDate == null ? null : docLine.StartDate.Value.ToString(StringFormatConst.SQLDateTimeParam));
                            parameters.Add("ExpireDate", docLine.EndDate == null ? null : docLine.EndDate.Value.ToString(StringFormatConst.SQLDateTimeParam));
                            parameters.Add("Status", 'A');
                            parameters.Add("CreatedBy", doc.UCreatedBy);
                            parameters.Add("CustomerId", doc.CardCode);
                            parameters.Add("TransId", doc.TransId);
                            parameters.Add("CardType", docLine.ItemType);
                            //parameters.Add("OwnerType", doc.TransId);
                            //parameters.Add("HoldType", doc.TransId);
                            parameters.Add("MemberValue", docLine.MemberValue ?? 0);

                            var res = db.Query("USP_IU_M_MemberCard", param: parameters, commandType: CommandType.StoredProcedure);
                            if (res != null && res.Any())
                            {
                                DataTable dataTable = res.ToDataTable();
                                if (dataTable != null && dataTable.Rows.Count > 0)
                                {
                                    string errorCode = dataTable.Rows[0]["ErrCode"].ToString();
                                    if (errorCode == "0")
                                    {
                                        result = true;
                                        message += dataTable.Rows[0]["ErrMsg"].ToString();
                                    }
                                    else
                                    {
                                        result = false;
                                        message += $"{errorCode} - {dataTable.Rows[0]["ErrMsg"]}";

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                message = "Exception: " + ex.Message;
            }

            return result;
        }

        public bool PointTransfer(LoyaltyPointTransferModel transferModel, out string message)
        {
            bool result = false;
            message = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    //double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value) - (doc.UDiscountAmount ?? 0);
                    DateTime dtNow = DateTime.Now;
                    var parameters = new DynamicParameters();

                    //parameters.Add("TransId", doc.TransId);
                    parameters.Add("CompanyCode", transferModel.CompanyCode);
                    parameters.Add("StoreId", transferModel.StoreId);
                    parameters.Add("SendCustomerId", transferModel.SendCustomerId);
                    parameters.Add("SendCustomerName", transferModel.SendCustomerName);
                    parameters.Add("SendCardNumber", transferModel.SendCardNumber);
                    parameters.Add("RecivedCustomerId", transferModel.RecivedCustomerId);
                    parameters.Add("RecivedCustomerName", transferModel.RecivedCustomerName);
                    parameters.Add("RecivedCardNumber", transferModel.RecivedCardNumber);

                    parameters.Add("TransDate", transferModel.TransDate == null ? dtNow.ToString(StringFormatConst.SQLDateTimeParam) : transferModel.TransDate.ToString(StringFormatConst.SQLDateTimeParam));
                    parameters.Add("TransType", "PointTransfer");
                    parameters.Add("TransPoint", transferModel.TransPoint);
                    parameters.Add("CreatedBy", transferModel.CreatedBy);
                    parameters.Add("CreatedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                    parameters.Add("ModifiedBy", transferModel.CreatedBy);
                    parameters.Add("ModifiedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                    parameters.Add("CalcStatus", "A");
                    //DateTime expire = new DateTime(dtNow.Year, 12, 31);
                    //parameters.Add("ExpireDate", expire.ToString(StringFormatConst.SQLDateParam));
                    parameters.Add("ExpireDate", null);

                    //  insert loyalty point transfer
                    var res = db.Query("USP_I_LoyaltyPointTransfer", param: parameters, commandType: CommandType.StoredProcedure);

                    if (res != null && res.Any())
                    {
                        DataTable dataTable = res.ToDataTable();
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            string errorCode = dataTable.Rows[0]["ErrCode"].ToString();
                            if (errorCode == "0")
                            {
                                result = true;
                                message += dataTable.Rows[0]["ErrMsg"].ToString();
                            }
                            else
                            {
                                result = false;
                                message += $"Error code: {errorCode}"; // {dataTable.Rows[0]["ErrMsg"]}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    message += "Exception error.";// + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return result;
        }

        private DataTable CreateTableLoyaltyHeader()
        {
            DataTable dt = new DataTable("LOYALTY_HEADER");
            dt.Columns.Add("LoyaltyId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LoyaltyType");
            dt.Columns.Add("LoyaltyName");
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
            dt.Columns.Add("TotalEarnType");
            dt.Columns.Add("TotalEarnValue");
            dt.Columns.Add("MaxTotalEarnValue");
            dt.Columns.Add("CreatedBy");
            dt.Columns.Add("CreatedOn", typeof(DateTime));
            dt.Columns.Add("ModifiedBy");
            dt.Columns.Add("ModifiedOn", typeof(DateTime));
            dt.Columns.Add("Status");

            return dt;
        }

        private DataTable CreateTableLoyaltyExclude()
        {
            DataTable dt = new DataTable("LOYALTY_EXCLUDE");
            dt.Columns.Add("LoyaltyId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LineType");
            dt.Columns.Add("LineCode");
            dt.Columns.Add("LineName");
            dt.Columns.Add("LineUom");

            return dt;
        }

        private DataTable CreateTablePromoBuy()
        {
            DataTable dt = new DataTable("LOYALTY_BUY");
            dt.Columns.Add("LoyaltyId");
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

            return dt;
        }

        private DataTable CreateTableLoyaltyEarn()
        {
            DataTable dt = new DataTable("LOYALTY_EARN");
            dt.Columns.Add("LoyaltyId");
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
            dt.Columns.Add("EarnValue");
            dt.Columns.Add("MaxPointApply");

            return dt;
        }

        private DataTable CreateTablePromoStore()
        {
            DataTable dt = new DataTable("LOYALTY_STORE");
            dt.Columns.Add("LoyaltyId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LineNum");
            dt.Columns.Add("StoreValue");

            return dt;
        }

        private DataTable CreateTableLoyaltyCustomer()
        {
            DataTable dt = new DataTable("LOYALTY_CUSTOMER");
            dt.Columns.Add("LoyaltyId");
            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("LineNum");
            dt.Columns.Add("CustomerValue");
            dt.Columns.Add("CustomerType");

            return dt;
        }

        public bool LoyaltyReCalcPoint(string customerId, bool isOBPoint, out string message)
        {
            bool result = false;
            message = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    //string getCustomer = "SELECT TOP 50 * FROM M_Customer WHERE CustomerId LIKE 'C%' AND ISNULL(DoNotAccumPoints, 0) <> 1 AND ISNULL(CustomF5, '') <> 'Y'";
                    string getCustomer = $"EXEC USP_GetCustomerForCalcPoint 'CP001', '{customerId}'";
                    List<MCustomer> listCustomers = db.Query<MCustomer>(getCustomer, commandType: CommandType.Text).ToList();
                    if (listCustomers != null && listCustomers.Count > 0)
                    {
                        foreach (MCustomer customer in listCustomers)
                        {
                            ////check data
                            //List<TSalesHeader> listHeaderCheck = db.Query<TSalesHeader>($"SELECT * FROM T_SalesHeader WHERE TransId = 'S4FM00400015981' AND CusId = '{customer.CustomerId}' ORDER BY CreatedOn", commandType: CommandType.Text).ToList();
                            //var orderCheck = GetOrderByIdAsync(db, listHeaderCheck[0], listHeaderCheck[0].TransId, listHeaderCheck[0].CompanyCode, listHeaderCheck[0].StoreId);
                            //if (orderCheck != null)
                            //{
                            //    var document = MapSOtoDocument(orderCheck);
                            //    document.DocObjectCode = "CheckPoint";
                            //    double inPoint = this.ApplyLoyalty(document, out _);
                            //}


                            List<TSalesHeader> listHeader = db.Query<TSalesHeader>($"SELECT * FROM T_SalesHeader WHERE CusId = '{customer.CustomerId}' ORDER BY CreatedOn", commandType: CommandType.Text).ToList();

                            double balancePoint = customer.OBPoint;
                            //if (!string.IsNullOrEmpty(customer.CustomF4))
                            //{
                            //    double.TryParse(customer.CustomF4, out balancePoint);
                            //}
                            double startPoint = 0;
                            if (isOBPoint)
                            {
                                startPoint = balancePoint;
                            }
                            string updateCusPoint = $"UPDATE M_Customer SET RewardPoints = {startPoint} WHERE ISNULL(DoNotAccumPoints, 0) <> 1 AND CustomerId = '{customer.CustomerId}'";
                            db.Execute(updateCusPoint, commandType: CommandType.Text);

                            if (balancePoint != 0)
                            {
                                CheckPointUpdateLoyaltyLogOpeningBalancePoint(db, customer.CompanyCode, customer.CustomerId, customer.CustomerName, customer.Phone, customer.CreatedOn.Value, "", balancePoint, customer.CreatedBy);
                            }

                            if (listHeader != null && listHeader.Count > 0)
                            {
                                string deleteLoyaltyLog = $"DELETE T_LoyaltyLog WHERE CustomerId = '{customer.CustomerId}' AND TransType <> 'OpeningBalance'";
                                db.Execute(deleteLoyaltyLog, commandType: CommandType.Text);

                                foreach (var header in listHeader)
                                {
                                    var order = GetOrderByIdAsync(db, header, header.TransId, header.CompanyCode, header.StoreId);
                                    if (order != null)
                                    {
                                        var document = MapSOtoDocument(order);
                                        document.UDiscountAmount = (double)(order.DiscountAmount ?? 0);

                                        if (order.Payments != null && order.Payments.Count > 0)
                                        {
                                            foreach (var payment in order.Payments)
                                            {
                                                if (payment.PaymentCode == "Point")
                                                {
                                                    double outPoint = double.Parse(payment.RefNumber);
                                                    double outAmt = double.Parse(payment.CollectedAmount.ToString());
                                                    if (outAmt < 0)
                                                    {
                                                        outPoint *= -1;
                                                    }
                                                    document.UDiscountAmount += payment.CollectedAmount == null ? 0 : (double)payment.CollectedAmount;

                                                    CheckPointUpdateLoyaltyLog(db, true, document, 0, outPoint, outAmt);
                                                }
                                            }
                                        }

                                        document.DocObjectCode = "CheckPoint";
                                        double inPoint = this.ApplyLoyalty(document, out _);
                                        CheckPointUpdateLoyaltyLog(db, false, document, inPoint, 0, 0);
                                    }
                                }
                            }

                            string updateStatus = $"UPDATE M_Customer SET OBRecal = 'Y', OBRecalDate = GETDATE() WHERE CustomerId = '{customer.CustomerId}'";
                            db.Execute(updateStatus, commandType: CommandType.Text);
                        }
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    message += "Exception error.";// + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return result;
        }

        private Document MapSOtoDocument(SaleViewModel model)
        {
            //GenericResult result = new GenericResult();

            Document document = new Document();
            document.DocDate = model.CreatedOn;
            document.CardCode = model.CusId;
            document.TransId = model.TransId;
            document.StoreId = model.StoreId;
            document.CardName = model.CusName;
            document.NumAtCard = model.Phone;
            document.OriginalRefNo = model.RefTransId;
            if (model.IsCanceled == "C")
            {
                //  canceled
                document.DocumentStatus = "CANCELED";
            }
            else if (!string.IsNullOrEmpty(model.SalesMode) && model.SalesMode.ToLower() == "return")
            {
                //  return
                document.DocumentStatus = "RETURN";
            }
            else if (!string.IsNullOrEmpty(model.SalesMode) && model.SalesMode.ToLower() == "ex")
            {
                //  exchange
                document.DocumentStatus = "EXCHANGE";
            }

            //document.DocDate = DateTime.;
            //document.CardGroup = basket.customer.customerGrpId;
            // currencyCode: string="";
            // storeType: string="";
            // listType: string="";
            // formatConfigId: string="";
            // whsCode: string="";
            //document.DocCurrency = model.Cu;
            document.UCompanyCode = model.CompanyCode;
            document.DocumentLines = new List<DocumentLine>();
            document.UCreatedBy = model.CreatedBy;
            foreach (var line in model.Lines)
            {
                DocumentLine lineDo = new DocumentLine();
                lineDo.ItemCode = line.ItemCode;
                lineDo.Quantity = (double)line.Quantity;
                lineDo.DiscountPercent = line.DiscountRate == null ? 0 : (double)line.DiscountRate;
                //lineDo.Currency = line.Cu
                lineDo.UnitPrice = (double)line.Price;
                lineDo.UoMCode = line.UomCode;
                lineDo.BarCode = line.BarCode;
                lineDo.LineTotal = (double)line.LineTotal;

                lineDo.ItemType = line.ItemType;
                lineDo.PrepaidCardNo = line.PrepaidCardNo;
                lineDo.MemberValue = line.MemberValue;
                lineDo.StartDate = line.StartDate;
                lineDo.EndDate = line.EndDate;

                document.DocumentLines.Add(lineDo);
            }

            return document;
        }

        private SaleViewModel GetOrderByIdAsync(IDbConnection db, TSalesHeader header, string Id, string CompanyCode, string StoreId)
        {
            try
            {
                //var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);
                //List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                //if (settingData.Success)
                //{

                //    SettingList = settingData.Data as List<GeneralSettingStore>;
                //}


                SaleViewModel order = new SaleViewModel();
                //string queryX = $"USP_S_T_SalesHeaderByType N'{companycode}' ,N'{storeId}', N'{TransId}',N'{SalesMan}',N'{Type}',N'{fromdate}',N'{todate}', N'{dataSource}',  N'{Status}',N'{Keyword}'";
                //TSalesHeader header = _saleHeaderRepository.Get($"USP_S_T_SalesHeader_ByTransId N'{CompanyCode}',N'{StoreId}' ,N'{Id}'", null, commandType: CommandType.Text);
                //if (header == null)
                //{
                //    return order;
                //}

                string queryLine = $"USP_S_T_SaleLine N'{CompanyCode}' ,N'{Id}'";
                string queryLineSerial = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesLineSerial t1 with(nolock) left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode = t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";
                //string queryPromo = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesPromo t1 with(nolock)  left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode=t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";

                //string invoiceQuery = $"USP_S_T_SalesInvoice N'{CompanyCode}',N'{StoreId}' ,N'{Id}'";
                string queryPayment = $"USP_S_T_SalesPaymentByTransId";
                //string queryDelivery = $"select * from T_Sales_Delivery with (nolock) where TransId=N'{Id}' and CompanyCode= N'{CompanyCode}'";

                //var queryContractPayment = $"USP_S_T_InvoicePayment N'{CompanyCode}' , N'{StoreId}', N'{header.ContractNo}'";

                //List<TSalesLine> lines = await _saleLineRepository.GetAllAsync(, null, commandType: CommandType.Text);
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode, DbType.String);
                parameters.Add("StoreId", StoreId, DbType.String);
                parameters.Add("TransId", Id, DbType.String);

                List<TSalesPayment> payments = _salepaymentLineRepository.GetAll(queryPayment, parameters, commandType: CommandType.StoredProcedure);

                //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId =N'{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<SaleViewModel>(header);
                //using (IDbConnection db = _saleHeaderRepository.GetConnection())
                //{
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var lines = db.Query<TSalesLineViewModel>(queryLine, null, commandType: CommandType.Text);


                    var serialLines = db.Query<TSalesLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                    var linesView = new List<TSalesLineViewModel>();
                    var NoBom = lines.Where(x => x.BomId == null || x.BomId.ToString() == "").ToList();
                    foreach (var line in NoBom)
                    {
                        //if(!string.IsNullOrEmpty(line.StoreAreaId) && !string.IsNullOrEmpty(line.TimeFrameId))
                        //{
                        //    if()
                        //    line.Lines.Add(line);
                        //}    
                        line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                        linesView.Add(line);
                    }

                    var bomHeader = new List<TSalesLineViewModel>();
                    var bomlines = lines.Where(x => x.BomId != null && x.BomId.ToString() != "").ToList();
                    foreach (var line in linesView)
                    {
                        var bomlineX = bomlines.Where(x => x.BomId == line.ItemCode).ToList();
                        line.Lines = bomlineX;
                    }
                    //foreach(var line in bomlines)
                    //{
                    //    TSalesLineViewModel BOMheader = new TSalesLineViewModel();

                    //    if(bomHeader.Where(x=>x.BarCode!= BOMheader.BarCode).SingleOrDefault()!=null)
                    //    {
                    //        bomHeader.Add(BOMheader);
                    //    }

                    //}    
                    //if(bomHeader.Count > 0)
                    //{
                    //    foreach (var line in bomHeader)
                    //    {
                    //        var bomLine = bomlines.Where(x => x.BomId != line.BarCode).ToList();
                    //        line.Lines = bomLine;
                    //        linesView.Add(line);
                    //    }
                    //}     
                    //var promoLines = db.Query<TSalesPromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                    //var deliveryLines = db.Query<TSalesDelivery>(queryDelivery, null, commandType: CommandType.Text);
                    //string loyaltySystem = $"select SettingValue from S_GeneralSetting with (nolock) where SettingId ='Loyalty' and CompanyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' ";
                    //loyaltySystem = _saleHeaderRepository.GetScalar(loyaltySystem, null, commandType: CommandType.Text);



                    order = _mapper.Map<SaleViewModel>(header);

                    //var linesBooklet = linesView.Where(x => string.IsNullOrEmpty(x.BookletNo)).ToList();
                    //foreach (var item in linesView.Where(x => !string.IsNullOrEmpty(x.BookletNo)).ToList())
                    //{
                    //    if (serialLines.Where(x => x.CustomF1 == item.BookletNo).FirstOrDefault() != null)
                    //    {
                    //        item.EndDate = serialLines.Where(x => x.CustomF1 == item.BookletNo).FirstOrDefault().ExpDate;
                    //    }

                    //}
                    order.Lines = linesView;// lines.ToList();


                    //order.SerialLines = serialLines.ToList();
                    //order.PromoLines = promoLines.ToList();
                    order.Payments = payments;
                    //order.Customer = customer;
                    //var invoice = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "Invoice").FirstOrDefault();

                    //if (invoice != null && (invoice.SettingValue == "true" || invoice.SettingValue == "1"))
                    //{
                    //    var invoiceLines = db.QueryFirstOrDefault<TSalesInvoice>(invoiceQuery, null, commandType: CommandType.Text);
                    //    order.Invoice = invoiceLines;
                    //}
                    //if (!string.IsNullOrEmpty(header.ContractNo))
                    //{
                    //    var ContractSalesPayment = db.Query<TSalesPayment>(queryContractPayment, null, commandType: CommandType.Text);
                    //    order.ContractPayments = ContractSalesPayment.ToList();
                    //}


                    //if (queryContractPayment!= null &&  queryContractPayment.Success && queryContractPayment.Data!=null)
                    //{
                    //    order.ContractPayments = queryContractPayment.Data as List<TSalesPayment>;
                    //}    
                    //order.Deliveries = deliveryLines.ToList();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //}

                return order;
                //result.Success = true;
                //result.Data = order;
            }
            catch (Exception ex)
            {
                //result.Success = false;
                //result.Message = ex.Message;
            }
            return null;
        }

        private bool CheckPointUpdateLoyaltyLog(IDbConnection db, bool isOut, Document doc, double inPoint, double outPoint, double outAmt)
        {
            try
            {
                double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value) - (doc.UDiscountAmount ?? 0);
                DateTime dtNow = doc.DocDate ?? DateTime.Now;
                var parameters = new DynamicParameters();

                parameters.Add("TransId", doc.TransId);
                parameters.Add("CompanyCode", doc.UCompanyCode);
                parameters.Add("StoreId", doc.StoreId);
                parameters.Add("CustomerId", doc.CardCode);
                parameters.Add("CustomerName", doc.CardName);
                parameters.Add("CardNumber", doc.NumAtCard);
                parameters.Add("TransDate", doc.DocDate.Value == null ? dtNow.ToString(StringFormatConst.SQLDateTimeParam) : doc.DocDate.Value.ToString(StringFormatConst.SQLDateTimeParam));

                DateTime expire = new DateTime(dtNow.Year, 12, 31);
                if (!isOut)
                {
                    parameters.Add("TransType", "SalesOrder");
                    parameters.Add("InPoint", inPoint);
                    parameters.Add("OutPoint", 0);
                    parameters.Add("InAmt", docTotal);
                    parameters.Add("OutAmt", 0);
                }
                else
                {
                    parameters.Add("TransType", "Payment");
                    parameters.Add("InPoint", 0);
                    parameters.Add("OutPoint", outPoint);
                    parameters.Add("InAmt", 0);
                    parameters.Add("OutAmt", outAmt);
                }

                parameters.Add("CreatedBy", doc.UCreatedBy);
                parameters.Add("CreatedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                parameters.Add("ModifiedBy", doc.UCreatedBy);
                parameters.Add("ModifiedOn", dtNow.ToString(StringFormatConst.SQLDateTimeParam));
                parameters.Add("CalcStatus", "A");
                parameters.Add("ExpireDate", expire.ToString(StringFormatConst.SQLDateParam));

                //  insert Loyalty Log
                db.Execute("USP_I_T_LoyaltyLog", param: parameters, commandType: CommandType.StoredProcedure);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckPointUpdateLoyaltyLogOpeningBalancePoint(IDbConnection db, string companyCode, string customerId, string customerName, string cardNo, DateTime transDate, string storeId, double inPoint, string createBy)
        {
            try
            {
                //double docTotal = doc.DocumentLines.Sum(l => l.LineTotal.Value) - (doc.UDiscountAmount ?? 0);
                DateTime dtNow = DateTime.Now;
                var parameters = new DynamicParameters();

                //string getCode = $"SELECT TOP 1 CONCAT( 'OBP{customerId}_', RIGHT(CONCAT('0000',ISNULL(right(max(TransId),4),0) + 1),4)) " +
                //    "from T_LoyaltyLog with (nolock) " +
                //    $"where CompanyCode = 'CP001' AND TransId like 'OBP{customerId}_%' AND CustomerId = '{customerId}' " +
                //    "group by  CompanyCode, CreatedOn, TransId " +
                //    "order by CreatedOn desc";

                //var res = db.ExecuteScalar(getCode, commandType: CommandType.Text);
                //if (res != null)
                //{
                //    parameters.Add("TransId", res.ToString());
                //}
                //else
                //{
                //    parameters.Add("TransId", $"OBP{customerId}_0001");
                //}

                string transId = $"OBP{customerId}_0001";
                string qryDelete = $"DELETE T_LoyaltyLog WHERE TransId = '{transId}'";
                db.Execute(qryDelete, commandType: CommandType.Text);

                parameters.Add("TransId", transId);
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", storeId);
                parameters.Add("CustomerId", customerId);
                parameters.Add("CustomerName", customerName);
                parameters.Add("CardNumber", cardNo);
                parameters.Add("TransDate", transDate.ToString(StringFormatConst.SQLDateTimeParam));

                DateTime expire = new DateTime(dtNow.Year, 12, 31);

                parameters.Add("TransType", "OpeningBalance");
                parameters.Add("InPoint", inPoint);
                parameters.Add("OutPoint", 0);
                parameters.Add("InAmt", 0);
                parameters.Add("OutAmt", 0);


                parameters.Add("CreatedBy", createBy);
                parameters.Add("CreatedOn", transDate.ToString(StringFormatConst.SQLDateTimeParam));
                parameters.Add("ModifiedBy", createBy);
                parameters.Add("ModifiedOn", transDate.ToString(StringFormatConst.SQLDateTimeParam));
                parameters.Add("CalcStatus", "A");
                parameters.Add("ExpireDate", expire.ToString(StringFormatConst.SQLDateParam));

                //  insert Loyalty Log
                db.Execute("USP_I_T_LoyaltyLog", param: parameters, commandType: CommandType.StoredProcedure);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckPointUpdateLoyaltyLogAdjustedPoint(IDbConnection db, string companyCode, int index, string customerId, string customerName, string cardNo, DateTime transDate, string storeId, double outPoint, string createBy)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                var parameters = new DynamicParameters();

                string transId = $"ADP{customerId}_{index:0000}";
                string qryDelete = $"DELETE T_LoyaltyLog WHERE TransId = '{transId}'";
                db.Execute(qryDelete, commandType: CommandType.Text);

                parameters.Add("TransId", transId);
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", storeId);
                parameters.Add("CustomerId", customerId);
                parameters.Add("CustomerName", customerName);
                parameters.Add("CardNumber", cardNo);
                parameters.Add("TransDate", transDate.ToString(StringFormatConst.SQLDateTimeParam));

                DateTime expire = new DateTime(dtNow.Year, 12, 31);

                parameters.Add("TransType", "AdjustRewardPoint");
                parameters.Add("InPoint", 0);
                parameters.Add("OutPoint", outPoint);
                parameters.Add("InAmt", 0);
                parameters.Add("OutAmt", 0);


                parameters.Add("CreatedBy", createBy);
                parameters.Add("CreatedOn", transDate.ToString(StringFormatConst.SQLDateTimeParam));
                parameters.Add("ModifiedBy", createBy);
                parameters.Add("ModifiedOn", transDate.ToString(StringFormatConst.SQLDateTimeParam));
                parameters.Add("CalcStatus", "A");
                parameters.Add("ExpireDate", expire.ToString(StringFormatConst.SQLDateParam));

                //  insert Loyalty Log
                db.Execute("USP_I_T_LoyaltyLog", param: parameters, commandType: CommandType.StoredProcedure);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public dynamic GetLoyaltyPointReport(string CompanyCode, string customerId, out string message)
        {
            IEnumerable<dynamic> result = null;
            message = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("CustomerId", customerId);
                    var items = db.Query("USP_RPT_LoyaltyPointReport", parameters, commandType: CommandType.StoredProcedure);
                    result = items;
                }
                catch (Exception ex)
                {
                    message = "Error Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            return result;
        }

        public double CheckPointByTransaction(string transId)
        {
            double inPoint = 0;
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                List<TSalesHeader> listHeader = db.Query<TSalesHeader>($"SELECT * FROM T_SalesHeader WHERE TransId = '{transId}' ORDER BY CreatedOn", commandType: CommandType.Text).ToList();

                if (listHeader != null & listHeader.Count > 0)
                {
                    TSalesHeader header = listHeader[0];
                    var order = GetOrderByIdAsync(db, header, header.TransId, header.CompanyCode, header.StoreId);
                    if (order != null)
                    {
                        var document = MapSOtoDocument(order);
                        document.UDiscountAmount = (double)(order.DiscountAmount ?? 0);

                        document.DocObjectCode = "CheckPoint";
                        inPoint = this.ApplyLoyalty(document, out _);
                    }
                }
            }

            return inPoint;
        }

        public bool LoyaltyAdjustPoint(string customerId, double outPoint, bool excludeOBPoint, int index, out string message)
        {
            bool result = false;
            message = "";
            using (IDbConnection db = sLoyalty.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string getCustomer = $"EXEC USP_GetCustomerForAdjustPoint 'CP001', '{customerId}'";
                    List<MCustomer> listCustomers = db.Query<MCustomer>(getCustomer, commandType: CommandType.Text).ToList();
                    if (listCustomers != null && listCustomers.Count > 0)
                    {
                        foreach (MCustomer customer in listCustomers)
                        {
                            if (customer.OBPoint <= 0 || (customer.RewardPoints ?? 0) <= 0)
                            {
                                continue;
                            }

                            double adjustPoint = 0;

                            if (outPoint <= 0 && excludeOBPoint)
                            {
                                if ((double)customer.RewardPoints.Value > 0)
                                {
                                    adjustPoint = Math.Max(0, Math.Min((double)customer.RewardPoints.Value, customer.OBPoint));
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                adjustPoint = (double)customer.RewardPoints.Value - outPoint;

                            }

                            bool check = CheckPointUpdateLoyaltyLogAdjustedPoint(db, customer.CompanyCode, index, customer.CustomerId, customer.CustomerName, customer.Phone, DateTime.Now, "", adjustPoint, customer.CreatedBy);

                            if (check)
                            {
                                string updateStatus = $"UPDATE M_Customer SET OBRecal = 'YY' WHERE CustomerId = '{customer.CustomerId}'";
                                db.Execute(updateStatus, commandType: CommandType.Text);
                            }
                        }
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    message += "Exception error.";// + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return result;
        }
    }
}

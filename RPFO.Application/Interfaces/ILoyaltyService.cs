using RPFO.Data.Entities;
using RPFO.Data.Models;
using RPFO.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Application.Interfaces
{
    public interface ILoyaltyService
    {
        List<MLoyaltyType> GetLoyaltyTypes(out string msg);
        List<LoyaltyHeaderViewModel> SearchLoyalty(string companyCode, string loyaltyId, int? loyaltyType, string loyaltyName, string customerType, string customerValue, DateTime? validDateFrom, DateTime? validDateTo, int? validTimeFrom, int? validTimeTo, string isMon, string isTue, string isWed, string isThu, string isFri, string isSat, string isSun, string status);
        LoyaltyViewModel GetLoyalty(string companyCode, string loyaltyId, out string msg);
        bool InsertUpdateLoyalty(LoyaltyViewModel loyalty, out string loyaltyId, out string msg);
        double ApplyLoyalty(Document srcDoc, out string message);
        bool InsertLoyaltyLog(bool isOut, Document doc, double point, double outPoint, double outAmt, out string msg);
        double PointConvert(string companyCode, string storeId, double point);
        List<SLoyaltyPointConvert> GetLoyaltyPointConverts(string companyCode, string storeId);
        bool InsertUPdateLoyaltyPointConverts(SLoyaltyPointConvert pointConvert, out string message);
        //List<SLoyaltyExclude> GetLoyaltyExcludes(string companyCode);
        //bool InsertLoyaltyExclude(string companyCode, string lineType, string lineCode, string lineName, out string message);
        //bool DeleteLoyaltyExclude(string companyCode, string lineType, string lineCode, out string message);
        LoyaltyViewModel GetLuckyNo(Document srcDoc, out string message);
        bool InsertUpdateMemberCard(Document doc, out string message);
        bool PointTransfer(LoyaltyPointTransferModel transferModel, out string message);
        bool LoyaltyReCalcPoint(string customerId, bool isOBPoint, out string message);
        dynamic GetLoyaltyPointReport(string CompanyCode, string customerId, out string message);
        double CheckPointByTransaction(string transId);
        bool LoyaltyAdjustPoint(string customerId, double outPoint, bool excludeOBPoint, int index, out string message);
    }
}

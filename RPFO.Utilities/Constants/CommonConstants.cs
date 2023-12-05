using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace RPFO.Utilities.Constants
{
    public class AppConstants
    {
        public const string DefaultFooterId = "DefaultFooterId";

        public const string TEXT_PHRASE = "c+2zsSQ/xwI=";
        public static List<ActiveUser> ActiveUsers = new List<ActiveUser>();
        public static class HubName
        {
            //public const string Announcement = "AnnouncementService";
            public const string Notification = "ReceiveMessage";
            public const string Activity = "ReceiveActivity";
            public const string RequestList = "ReceiveRequestList";
            public const string Request = "ReceiveRequest";
            public const string RequestTeam = "ReceiveRequestTeam";
            public const string RequestGalaries = "ReceiveRequestGalaries";
            public const string OneRequest = "ReceiveOneRequest";
            public const string RequestDetails = "ReceiveRequestDetails";
            public const string Announcement = "ReceiveAnnouncement";
            public const string AnnouncementTop = "ReceiveAnnouncementTop";
            public const string UserProfile = "UserProfile";
            public const string SystemConfig = "ReceiveSystemConfig";
            public const string ResetPassword = "ResetPassword";
        }
       
    }
   
    public class ActiveUser
    {
        public string CompanyCode {get; set;}
        public string UserId {get; set;}
        public string ConnectionId {get; set;}
        public string CounterId {get; set;}  
        public string Status { get; set; } = "Offline";
    }
    public struct oDocStatus
    {
        public const string Open = "O";
        public const string Close = "C";
        public const string Partial = "P";
    }

    public struct ConnectSettingKey
    {
        public const string API_JA_AUTH_URL = "API_JA_AUTH_URL";
        public const string API_JA_MID_URL = "API_JA_MID_URL";
        public const string API_JA_GRANT_TYPE = "API_JA_GRANT_TYPE";
        public const string API_JA_CLIENT_ID = "API_JA_CLIENT_ID";
        public const string API_JA_CLIENT_SECRET = "API_JA_CLIENT_SECRET";
    }

    public struct StringFormatConst
    {
        public const string SQLDateParam = "yyyy-MM-dd";
        public const string SQLDateTimeParam = "yyyy-MM-dd H:mm:ss.fff";
        public const string SQLTimeParam = "Hmm";
    }

    public struct PromoCustomerType
    {
        public const string TypeCodeValue = "C";
        public const string TypeCodeMember = "Customer Code";
        public const string TypeGroupValue = "G";
        public const string TypeGroupMember = "Customer Group";
        public const string TypeRankValue = "R";
        public const string TypeRankMember = "Customer Rank";
    }

    public struct PromoType
    {
        public const int SingeDiscountCode = 1;
        public const string SingeDiscountName = "Singe Discount";

        public const int BuyXGetYCode = 2;
        public const string BuyXGetYName = "Buy X Get Y";

        public const int ComboCode = 3;
        public const string ComboName = "Combo";

        public const int TotalBillCode = 4;
        public const string TotalBillName = "Total Bill";

        public const int MixMatchCode = 6;
        public const string MixMatchName = "Mix and Match";

        public const int PrepaidCardCode = 7;
        public const string PrepaidCardName = "Prepaid Card";

        public const int MultiBuyCode = 8;
        public const string MultiBuyName = "Multi Buy";

        public const int CouponCode = 9;
        public const string CouponName = "Coupon";

        public const int PaymentDiscountCode = 10;
        public const string PaymentDiscountName = "Payment Discount";
    }

    public struct PromoLineType
    {
        public const string BarCode = "BarCode";
        public const string ItemCode = "ItemCode";
        public const string ItemGroup = "ItemGroup";
        public const string Collection = "Collection";
        public const string OneTimeGroup = "OneTimeGroup";
        public const string PaymentCode = "PaymentCode";
        public const string PaymentType = "PaymentType";
    }

    public struct PromoCondition
    {
        public const string CE = "CE";
        public const string FROM = "FROM";
        public const string TO = "TO";
        public const string Amount = "Amount";
        public const string Quantity = "Quantity";
        public const string Accumulated = "Accumulated";
    }

    public struct PromoValueType
    {
        public const string DiscountAmount = "Discount Amount";
        public const string DiscountPercent = "Discount Percent";
        public const string FixedPrice = "Fixed Price";
        public const string FixedQuantity = "Fixed Quantity";
        public const string BonusAmount = "Bonus Amount";
        public const string BonusPercent = "Bonus Percent";
        //public const string GetPercent = "Get Percent";
    }

    public struct PromoMaxApplyType
    {
        public const string MaxQtyByReceiptCode = "MQR";
        public const string MaxQtyByReceiptName = "Max Quantity By Receipt";

        public const string MaxQtyByStoreCode = "MQS";
        public const string MaxQtyByStoreName = "Max Quantity By Store";

        public const string MaxQtyByPeriodCode = "MQP";
        public const string MaxQtyByPeriodName = "Max Quantity By Period";
    }

    public struct POSType
    {
        public const string Event = "E";
        public const string Retail = "R";
    }

    public struct LoyaltyType
    {
        public const int BasedProductsCode = 1;
        public const string BasedProductsName = "Based on products";

        public const int BasedSpendingCode = 2;
        public const string BasedSpendingName = "Based on spending";

        public const int LuckyDrawCode = 3;
        public const string LuckyDrawName = "Lucky Draw";
    }

    public struct LoyaltyLineType
    {
        public const string BarCode = "BarCode";
        public const string ItemCode = "ItemCode";
        public const string ItemGroup = "ItemGroup";
        public const string Collection = "Collection";
    }

    public struct LoyaltyCondition
    {
        public const string CE = "CE";
        public const string FROM = "FROM";
        public const string TO = "TO";
        public const string Amount = "Amount";
        public const string Quantity = "Quantity";
    }
    public struct LicenseStatus
    {
        public const string EXP = "Expried";
        public const string ACTIVE = "Active";
        public const string INACTIVE = "InActive";
        public const string NA = "NotAvailable";
        public const string NULL = "NULL";
    }

    public struct RoundingType
    {
        public const string NoRounding = "NoRounding";
        public const string RoundToFiveHundredth = "RoundToFiveHundredth";
        public const string RoundToTenHundredth = "RoundToTenHundredth";
        public const string RoundToOne = "RoundToOne";
        public const string RoundToTen = "RoundToTen";
    }

    public struct LoyaltyValueType
    {
        public const string FixedPoint = "Fixed Point";
        public const string PercentAmount = "Percent Amount";
    }
}

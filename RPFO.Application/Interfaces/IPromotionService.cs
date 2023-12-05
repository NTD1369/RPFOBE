using RPFO.Data.Entities;
using RPFO.Data.Models;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IPromotionService
    {
        List<MPromoType> GetPromoTypes(out string msg);
        List<PromoHeaderViewModel> SearchPromo(string companyCode, string promoId, int? promotype, string promoName, string customerType, string customerValue, DateTime? validDateFrom, DateTime? validDateTo, int? validTimeFrom, int? validTimeTo, string isMon, string isTue, string isWed, string isThu, string isFri, string isSat, string isSun, string isCombine, string status);
        PromotionViewModel GetPromotion(string companyCode, string promoId, out string msg);
        bool InsertUpdatePromotion(PromotionViewModel promotion, out string promotionId, out string msg);
        List<SchemaHeaderViewModel> SearchSchema(string companyCode, string schemaId, string schemaName, string promoId, string status);
        SchemaViewModel GetSchema(string companyCode, string schemaId, out string msg);
        bool InsertUpdateSchema(SchemaViewModel schema, out string msg);
        GenericResult CheckVoucherPromotion(Document srcDoc);
        Document ApplyPromotion(Document srcDoc);
        Task<GenericResult> Remove(string companyCode, string promoId);
        List<PromotionResultViewModel> ImportData(DataImport model);
        //Task<GenericResult> Import(DataImport model);
        List<SPromoSchema> CheckSchemaHeader(string companyCode, string storeId, string customerCode, string customerGrp, double totalBuy, DateTime docDate);
        List<SPromoHeader> CheckPromotions(string companyCode, string storeId, int promoType, string customerCode, string customerGrp, double totalBuy, DateTime docDate);
        Document ApplySingleSchema(Document doc, string schemaId);
        Document ApplySinglePromotion(Document doc, string promoId);
        List<SPromoSchema> SimulatorSchema(Document doc);
        List<SPromoHeader> SimulatorPromotions(Document doc);

        GenericResult CheckInActiveGetBuy(InActivePromoViewModel model);
        Document ApplyPaymentDiscount(Document doc);
    }
}

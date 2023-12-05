using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPFO.Application.Interfaces;
using RPFO.Data.Models;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private IPromotionService PromotionService;
        //private string LogPath = "C:\\RPFO.API.Log\\";
        public PromotionsController(IPromotionService promotionService)
        {
            this.PromotionService = promotionService;
        }

        [HttpGet("PromoType")]
        public IActionResult PromotionsType()
        {
            GenericResult result = new();
            try
            {
                var data = PromotionService.GetPromoTypes(out string msg);
                if (data != null && data.Count > 0)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        result.Message = msg;
                    }
                    else
                    {
                        result.Message = "Cannot found data.";
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("Search")]
        public IActionResult SearchPromotion(string companyCode, string promoId, int? promotype, string promoName, string customerType, string customerValue, DateTime? validDateFrom, DateTime? validDateTo, int? validTimeFrom, int? validTimeTo, string isMon, string isTue, string isWed, string isThu, string isFri, string isSat, string isSun, string isCombine, string status)
        {
            GenericResult result = new();
            try
            {
                var data = PromotionService.SearchPromo(companyCode, promoId, promotype, promoName, customerType, customerValue, validDateFrom, validDateTo, validTimeFrom, validTimeTo, isMon, isTue, isWed, isThu, isFri, isSat, isSun, isCombine, status);
                if (data != null && data.Count > 0)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot found data.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("Promotion")]
        public IActionResult GetPromotion(string companyCode, string promoId)
        {
            GenericResult result = new();
            try
            {
                var data = PromotionService.GetPromotion(companyCode, promoId, out string msg);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        result.Message = msg;
                    }
                    else
                    {
                        result.Message = "Cannot found data.";
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("Remove")]
        //[HttpDelete]
        //[Route("Remove/{companyCode}/{promotionId}")]
        public async Task<GenericResult> Remove(string companyCode, string promotionId)
        {
            var data = await PromotionService.Remove(companyCode, promotionId);
            return data;
        }

        //[AllowAnonymous]
        [HttpPost("CreateUpdate")]
        public IActionResult CreateUpdatePromotion([FromBody] PromotionViewModel promotion)
        {
            GenericResult result = new();
            try
            {
                if (promotion == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(promotion.CompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                var data = PromotionService.InsertUpdatePromotion(promotion, out string promotionId, out string msg);
                if (data)
                {
                    result.Code = 1;
                    result.Success = true;
                    result.Data = promotionId;
                    result.Message = msg;
                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        result.Message = msg;
                    }
                    else
                    {
                        result.Message = "Cannot found data.";
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("SearchSchema")]
        public IActionResult SearchSchema(string companyCode, string schemaId, string schemaName, string promoId, string status)
        {
            GenericResult result = new();
            try
            {
                var data = PromotionService.SearchSchema(companyCode, schemaId, schemaName, promoId, status);
                if (data != null && data.Count > 0)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot found data.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("Schema")]
        public IActionResult GetSchema(string companyCode, string schemaId)
        {
            GenericResult result = new();
            try
            {
                var data = PromotionService.GetSchema(companyCode, schemaId, out string msg);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;

                }
                else
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        result.Message = msg;
                    }
                    else
                    {
                        result.Message = "Cannot found data.";
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpPost("CreateUpdateSchema")]
        public IActionResult CreateUpdateSchema([FromBody] SchemaViewModel schema)
        {
            GenericResult result = new();
            try
            {
                if (schema == null)
                {
                    return NoContent();
                }

                //if (string.IsNullOrEmpty(schema.SchemaId))
                //{
                //    result.Message = "SchemaId must be not null.";
                //    return Ok(result);
                //}

                if (string.IsNullOrEmpty(schema.CompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }
                schema.SchemaLines = schema.SchemaLines.OrderBy(x => x.Priority).ToList();
                int Ind = 1;
                foreach (var sche in schema.SchemaLines)
                {
                    sche.VirtualIndex = Ind;
                    Ind++;
                }
                var TotalList = schema.SchemaLines.Where(x => x.PromoType == "4" || x.PromoTypeName == "Total Bill").ToList();
                int countNum = schema.SchemaLines.Count();
                bool check = true;
                if (TotalList != null && TotalList.Count > 0)
                {
                    int num = countNum - TotalList.Count;
                    var checkIndex = TotalList.Where(sche => sche.VirtualIndex <= num).FirstOrDefault();
                    if (checkIndex != null)
                        check = false;

                }
                if (check)
                {
                    var data = PromotionService.InsertUpdateSchema(schema, out string msg);
                    if (data)
                    {
                        result.Success = true;
                        result.Data = "Create/Update schema is successfuly.";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(msg))
                        {
                            result.Message = msg;
                        }
                        else
                        {
                            result.Message = "Cannot found data.";
                        }
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "Priority. Total bill must be at the end of the schema.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }
        [HttpPost("CheckVoucherPromotion")]
        public GenericResult CheckVoucherPromotion(Document srcDoc)
        {
            return PromotionService.CheckVoucherPromotion(srcDoc);
        }

        //[AllowAnonymous]
        [HttpPost("Apply")]
        public IActionResult ApplyPromotion([FromBody] Document doc)
        {
            GenericResult result = new();
            //var json = doc.ToJson();
            try
            {
                if (doc == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(doc.UCompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                var data = PromotionService.ApplyPromotion(doc);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot apply promotion.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return UnprocessableEntity(result);
            }
        }

        [HttpPost]
        [Route("Import")]
        public IActionResult Import(DataImport model)
        {
            GenericResult result = new();
            //var json = doc.ToJson();
            try
            {
                if (model == null || model.Promotion == null || model.Promotion.Count == 0)
                {
                    return NoContent();
                }

                var data = PromotionService.ImportData(model);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot import promotion.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return UnprocessableEntity(result);

                //return BadRequest(ex.Message);
            }
        }

        //[HttpPost]
        //[Route("Import")]
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    return await PromotionService.Import(model);
        //}

        [HttpGet]
        [Route("SchemaList")]
        public IActionResult GetSchemaList(string companyCode, string storeId, string customerCode, string customerGrp, double totalBuy, DateTime docDate)
        {
            GenericResult result = new();
            try
            {

                if (docDate.Hour == 0 && docDate.Minute == 0 && docDate.Second == 0)
                {
                    docDate = docDate.AddHours(DateTime.Now.Hour);
                    docDate = docDate.AddMinutes(DateTime.Now.Minute);
                    docDate = docDate.AddSeconds(DateTime.Now.Second);
                }

                List<Data.Entities.SPromoSchema> data = PromotionService.CheckSchemaHeader(companyCode, storeId, customerCode, customerGrp, totalBuy, docDate);
                if (data != null && data.Count > 0)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot found data.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpGet]
        [Route("PromoList")]
        public IActionResult GetPromotionList(string companyCode, string storeId, int promoType, string customerCode, string customerGrp, double totalBuy, DateTime docDate)
        {
            GenericResult result = new();
            try
            {
                if (docDate.Hour == 0 && docDate.Minute == 0 && docDate.Second == 0)
                {
                    docDate = docDate.AddHours(DateTime.Now.Hour);
                    docDate = docDate.AddMinutes(DateTime.Now.Minute);
                    docDate = docDate.AddSeconds(DateTime.Now.Second);
                }
                var data = PromotionService.CheckPromotions(companyCode, storeId, promoType, customerCode, customerGrp, totalBuy, docDate);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot found data.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Route("ApplySchema")]
        public IActionResult ApplySingleSchema([FromBody] Document doc, string schemaId)
        {
            GenericResult result = new();
            try
            {
                if (doc == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(doc.UCompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                if (string.IsNullOrEmpty(schemaId))
                {
                    result.Message = "SchemaId must be not null.";
                    return Ok(result);
                }

                var data = PromotionService.ApplySingleSchema(doc, schemaId);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot apply promotion.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return Ok(result);

                //return BadRequest(ex.Message);
            }
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("ApplyPromo")]
        public IActionResult ApplySinglePromotion([FromBody] Document doc, string promoId)
        {
            GenericResult result = new();
            //var json = doc.ToJson();
            try
            {
                if (doc == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(doc.UCompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                if (string.IsNullOrEmpty(promoId))
                {
                    result.Message = "PromoId must be not null.";
                    return Ok(result);
                }

                var data = PromotionService.ApplySinglePromotion(doc, promoId);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot apply promotion.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return Ok(result);

                //return BadRequest(ex.Message);
            }
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("Simulator")]
        public IActionResult ApplySimulator([FromBody] Document doc, string schema)
        {
            GenericResult result = new();
            //var json = doc.ToJson();
            try
            {
                if (doc == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(doc.UCompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                if (schema == "Y")
                {
                    var data = PromotionService.SimulatorSchema(doc);
                    if (data != null)
                    {
                        result.Success = true;
                        result.Data = data;
                    }
                    else
                    {
                        result.Message = "Cannot apply promotion.";
                    }
                }
                else
                {
                    var data = PromotionService.SimulatorPromotions(doc);
                    if (data != null)
                    {
                        result.Success = true;
                        result.Data = data;
                    }
                    else
                    {
                        result.Message = "Cannot apply promotion.";
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return UnprocessableEntity(result);
            }
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("CheckInActiveGetBuy")]
        public IActionResult CheckInActiveGetBuy(InActivePromoViewModel inActivePromoViewModel)
        {
            GenericResult result = new();
            try
            {
                if (inActivePromoViewModel == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(inActivePromoViewModel.CompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                if (string.IsNullOrEmpty(inActivePromoViewModel.PromoLineType))
                {
                    result.Message = "PromoLineType must be not null.";
                    return Ok(result);
                }

                if (string.IsNullOrEmpty(inActivePromoViewModel.PromoId))
                {
                    result.Message = "PromoId must be not null.";
                    return Ok(result);
                }

                result = PromotionService.CheckInActiveGetBuy(inActivePromoViewModel);

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return UnprocessableEntity(result);
            }
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("ApplyPayment")]
        public IActionResult ApplyPaymentDiscount([FromBody] Document doc)
        {
            GenericResult result = new();
            //var json = doc.ToJson();
            try
            {
                if (doc == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(doc.UCompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                if (!doc.SalesPayments.Any())
                {
                    result.Message = "SalesPayment must be not null.";
                    return Ok(result);
                }

                var data = PromotionService.ApplyPaymentDiscount(doc);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                }
                else
                {
                    result.Message = "Cannot apply payment discount.";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return Ok(result);

                //return BadRequest(ex.Message);
            }
        }
    }
}

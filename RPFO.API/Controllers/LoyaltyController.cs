using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPFO.Application.Interfaces;
using RPFO.Data.Models;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        private ILoyaltyService LoyaltyService;

        public LoyaltyController(ILoyaltyService loyaltyService)
        {
            this.LoyaltyService = loyaltyService;
        }

        [HttpGet("Type")]
        public IActionResult LoyaltyType()
        {
            GenericResult result = new();
            try
            {

                var data = LoyaltyService.GetLoyaltyTypes(out string msg);
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
        public IActionResult SearchLoyalty(string companyCode, string loyaltyId, int? loyaltyType, string loyaltyName, string customerType,
            string customerValue, DateTime? validDateFrom, DateTime? validDateTo, int? validTimeFrom, int? validTimeTo, string isMon,
            string isTue, string isWed, string isThu, string isFri, string isSat, string isSun, string isCombine, string status)
        {
            GenericResult result = new();
            try
            {

                var data = LoyaltyService.SearchLoyalty(companyCode, loyaltyId, loyaltyType, loyaltyName, customerType, customerValue, validDateFrom, validDateTo, validTimeFrom, validTimeTo, isMon, isTue, isWed, isThu, isFri, isSat, isSun, status);
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

        [HttpGet("Loyalty")]
        public IActionResult GetLoyalty(string companyCode, string loyaltyId)
        {
            GenericResult result = new();
            try
            {
                var data = LoyaltyService.GetLoyalty(companyCode, loyaltyId, out string msg);
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

        //[AllowAnonymous]
        [HttpPost("CreateUpdate")]
        public IActionResult CreateUpdateLoyalty([FromBody] LoyaltyViewModel loyalty)
        {
            GenericResult result = new();
            try
            {
                if (loyalty == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(loyalty.CompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                var res = LoyaltyService.InsertUpdateLoyalty(loyalty, out string loyaltyId, out string msg);
                if (res)
                {
                    result.Success = true;
                    result.Data = loyaltyId;// "Create/Update loyalty is successfuly.";
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

        [HttpPost("Apply")]
        public IActionResult ApplyLoyalty([FromBody] Document doc)
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

                double data = LoyaltyService.ApplyLoyalty(doc, out string msg);

                result.Success = true;
                result.Data = data;
                result.Message = msg;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("RedeemPoint")]
        public IActionResult RedeemPoint(string companyCode, string storeId, double point)
        {
            GenericResult result = new();
            try
            {
                result.Data = LoyaltyService.PointConvert(companyCode, storeId, point);
                result.Success = true;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        //[HttpGet]
        //public IActionResult Test()
        //{
        //    Rounding(Utilities.Constants.RoundingType.RoundToFiveHundredth, 4.24);
        //    return Ok();
        //}

        //private double Rounding(string roundType, double src)
        //{
        //    double result = 0.0;
        //    if (roundType == Utilities.Constants.RoundingType.NoRounding)
        //    {
        //        result = src;
        //    }
        //    else if (roundType == Utilities.Constants.RoundingType.RoundToFiveHundredth)
        //    {
        //        result = Math.Round(src, 2, MidpointRounding.AwayFromZero);
        //    }
        //    else if (roundType == Utilities.Constants.RoundingType.RoundToTenHundredth)
        //    {

        //    }
        //    else if (roundType == Utilities.Constants.RoundingType.RoundToOne)
        //    {

        //    }
        //    else if (roundType == Utilities.Constants.RoundingType.RoundToTen)
        //    {

        //    }

        //    return result;
        //}

        [HttpGet("PointConvert")]
        public IActionResult LoyaltyPointConvert(string companyCode, string storeId)
        {
            GenericResult result = new();
            try
            {
                result.Data = LoyaltyService.GetLoyaltyPointConverts(companyCode, storeId);
                result.Success = true;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpPost("CreateUpdatePointConvert")]
        public IActionResult CreateUpdateLoyaltyPointConvert([FromBody] Data.Entities.SLoyaltyPointConvert pointConvert)
        {
            GenericResult result = new();
            try
            {
                if (pointConvert == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(pointConvert.CompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                if (string.IsNullOrEmpty(pointConvert.StoreId))
                {
                    result.Message = "StoreId must be not null.";
                    return Ok(result);
                }

                var data = LoyaltyService.InsertUPdateLoyaltyPointConverts(pointConvert, out string msg);
                if (data)
                {
                    result.Success = true;
                    result.Data = "Create/Update Point Convert is successfuly.";
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

        //[HttpGet("Excludes")]
        //public IActionResult GetLoyaltyExcludes(string companyCode)
        //{
        //    GenericResult result = new();
        //    try
        //    {
        //        result.Data = LoyaltyService.GetLoyaltyExcludes(companyCode);
        //        result.Success = true;

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = "Exception: " + ex.Message;
        //        return BadRequest(result);
        //    }
        //}

        //[HttpPost("InsertExclude")]
        //public IActionResult InsertLoyaltyExclude(string companyCode, string lineType, string lineCode, string lineName)
        //{
        //    GenericResult result = new();
        //    try
        //    {
        //        var res = LoyaltyService.InsertLoyaltyExclude(companyCode, lineType, lineCode, lineName, out string message);
        //        if (res)
        //        {
        //            result.Success = true;
        //        }
        //        else
        //        {
        //            result.Success = false;
        //            result.Message = message;
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = "Exception: " + ex.Message;
        //        return BadRequest(result);
        //    }
        //}

        //[HttpPost("DeleteExclude")]
        //public IActionResult DeleteLoyaltyExclude(string companyCode, string lineType, string lineCode)
        //{
        //    GenericResult result = new();
        //    try
        //    {
        //        var res = LoyaltyService.DeleteLoyaltyExclude(companyCode, lineType, lineCode, out string message);
        //        if (res)
        //        {
        //            result.Success = true;
        //        }
        //        else
        //        {
        //            result.Success = false;
        //            result.Message = message;
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = "Exception: " + ex.Message;
        //        return BadRequest(result);
        //    }
        //}

        //[AllowAnonymous]
        [HttpPost("LuckyNo")]
        public IActionResult GetLuckyNo([FromBody] Document doc)
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

                if (string.IsNullOrEmpty(doc.StoreId))
                {
                    result.Message = "StoreId must be not null.";
                    return Ok(result);
                }

                LoyaltyViewModel data = LoyaltyService.GetLuckyNo(doc, out string msg);
                if (data != null)
                {
                    result.Success = true;
                    result.Data = data;
                    result.Message = msg;
                }
                else
                {
                    result.Success = false;
                    result.Data = data;
                    result.Message = msg;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [HttpPost("MemberCard")]
        public IActionResult InsertUpdateMemberCard([FromBody] Document doc)
        {
            GenericResult result = new();
            try
            {
                if (doc == null)
                {
                    return NoContent();
                }

                //if (string.IsNullOrEmpty(doc.UCompanyCode))
                //{
                //    result.Message = "CompanyCode must be not null.";
                //    return Ok(result);
                //}

                if (string.IsNullOrEmpty(doc.CardCode))
                {
                    result.Message = "CardCode must be not null.";
                    return Ok(result);
                }

                bool res = LoyaltyService.InsertUpdateMemberCard(doc, out string msg);
                if (res)
                {
                    result.Success = true;
                    result.Message = msg;
                }
                else
                {
                    result.Success = false;
                    result.Message = msg;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [HttpPost("PointTransfer")]
        public IActionResult LoyaltyPointTransfer([FromBody] LoyaltyPointTransferModel transferModel)
        {
            GenericResult result = new();
            try
            {
                if (transferModel == null)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(transferModel.CompanyCode))
                {
                    result.Message = "CompanyCode must be not null.";
                    return Ok(result);
                }

                if (string.IsNullOrEmpty(transferModel.SendCustomerId))
                {
                    result.Message = "SendCustomerId must be not null.";
                    return Ok(result);
                }

                if (string.IsNullOrEmpty(transferModel.RecivedCustomerId))
                {
                    result.Message = "RecivedCustomerId must be not null.";
                    return Ok(result);
                }

                if (transferModel.TransPoint <= 0)
                {
                    result.Message = "TransPoint must be greater than 0 .";
                    return Ok(result);
                }

                result.Success = LoyaltyService.PointTransfer(transferModel, out string msg);
                result.Message = msg;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [AllowAnonymous]
        [HttpPost("ReCalcPoint")]
        public IActionResult LoyaltyReCalcPoint(string customerId, bool isOBPoint, string password)
        {
            GenericResult result = new();
            try
            {
                string desPass = DateTime.Now.ToString("HHmm");
                if (string.IsNullOrEmpty(password) || password != desPass)
                {
                    result.Success = false;
                    result.Message = "Wrong Password.";

                    return BadRequest(result);
                }

                result.Success = LoyaltyService.LoyaltyReCalcPoint(customerId, isOBPoint, out string msg);
                result.Message = msg;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [AllowAnonymous]
        [HttpPost("AdjustPoint")]
        public IActionResult LoyaltyAdjustPoint(string customerId, double outPoint, bool excludeOBPoint, int index, string password)
        {
            GenericResult result = new();
            try
            {
                string desPass = DateTime.Now.ToString("HHmm");
                if (string.IsNullOrEmpty(password) || password != desPass)
                {
                    result.Success = false;
                    result.Message = "Wrong Password.";

                    return BadRequest(result);
                }

                result.Success = LoyaltyService.LoyaltyAdjustPoint(customerId, outPoint, excludeOBPoint, index, out string msg);
                result.Message = msg;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [HttpGet("PointReport")]
        public IActionResult LoyaltyPointReport(string companyCode, string customerId)
        {
            GenericResult result = new();
            try
            {
                var res = LoyaltyService.GetLoyaltyPointReport(companyCode, customerId, out string msg);
                if (res != null)
                {
                    result.Success = true;
                    result.Data = res;
                }
                else
                {
                    result.Success = false;
                }
                result.Message = msg;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [AllowAnonymous]
        [HttpGet("CheckPoint")]
        public IActionResult CheckPoint(string transId)
        {
            GenericResult result = new();
            try
            {
                var res = LoyaltyService.CheckPointByTransaction(transId);
                if (res != 0)
                {
                    result.Success = true;
                    result.Data = res;
                }
                else
                {
                    result.Success = false;
                }
                //result.Message = msg;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }
    }
}

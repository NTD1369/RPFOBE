using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RPFO.Utilities.Dtos;
using System.Linq;

namespace MWI.API.Helpers
{
    public class OnlyABEOWebhookAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //base.OnActionExecuting(context);

            string ciaRequest = string.Empty;
            var ciaRequestObject = filterContext.HttpContext.Request.Headers.FirstOrDefault(x => x.Key.ToUpper() == "X-ABEO-SECRET");
            if (ciaRequestObject.Key != null)
                ciaRequest = ciaRequestObject.Value.ToString();
            if (string.IsNullOrEmpty(ciaRequest))
            {
                var res = new GenericResult
                {
                    Success = false,
                    Code = 401,
                    Message = "Invalid request (not from ABEO)"
                };
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.Result = new JsonResult(res);
                return;
            }
            // Extract the raw body of the request 
            string secretKey = Utils.GetConfigByKey("Abeo_Secret");
            if (string.Compare(secretKey, ciaRequest, true) != 0)
            {
                var res = new GenericResult
                {
                    Success = false,
                    Code = 401,
                    Message = "Invalid request (not from ABEO)"
                };
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.Result = new JsonResult(res);
                return;
            }
        }
    }
}

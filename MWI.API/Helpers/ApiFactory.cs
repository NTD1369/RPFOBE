using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MWI.API.Helpers
{
    public class ApiFactory
    {
        public static bool CheckAbeoOnly(HttpContext context, IConfiguration config, out HttpResponseMessage httpResponse)
        {
            httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            string request = string.Empty;
            var requestObject = context.Request.Headers.FirstOrDefault(x => x.Key == "X-ABEO-SECRET");
            if (requestObject.Key != null)
                request = requestObject.Value.ToString();
            if (string.IsNullOrEmpty(request))
            {
                httpResponse.Content = new StringContent("Invalid request (not from ABEO)");
                return false;
            }

            string secretKey = config.GetSection("AppSettings:Abeo_Secret").Value;
            string secretDecrypt = Encryptor.DecryptString(secretKey, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.Compare(secretDecrypt, request, true) != 0)
            {
                httpResponse.Content = new StringContent("Invalid request (not from ABEO)");
                return false;
            }

            return true;
        }
    }
}

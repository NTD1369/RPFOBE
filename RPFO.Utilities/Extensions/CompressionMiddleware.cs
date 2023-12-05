using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Utilities.Extensions
{
    public class CompressionMiddleware
    {
        private readonly RequestDelegate _next;

        public CompressionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var acceptEncoding = httpContext.Request.Headers["Accept-Encoding"].ToString();
            if (!string.IsNullOrEmpty(acceptEncoding))
            {
                if (acceptEncoding.ToString().IndexOf("gzip", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var stream = httpContext.Response.Body;
                        httpContext.Response.Body = memoryStream;
                        await _next(httpContext);
                        using (var compressedStream = new GZipStream(stream, CompressionLevel.Optimal))
                        {
                            httpContext.Response.Headers.Add("Content-Encoding", new string[] { "gzip" });
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await memoryStream.CopyToAsync(compressedStream);
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RPFO.API.Helpers
{
    public class ClientHelper
    {
        public HttpClient _client = new HttpClient();
        public HashCode Crypt = new HashCode();

        public void conect()
        {
            //string localhostString = "http://localhost:5000/api/";
            //string localhostString = "http://192.168.1.83:7998/api/";
            //string localhostString = "https://pos.farmersmarket.vn:8888/api/";
            string localhostString = "https://localhost:7060/";

            _client.Timeout = TimeSpan.Parse("00:25:00");
            _client.BaseAddress = new Uri(localhostString);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_client.Dispose();
        }
    }
}

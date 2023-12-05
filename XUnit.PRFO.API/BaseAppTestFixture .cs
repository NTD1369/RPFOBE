using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPFO.API;
using RPFO.Application.Implements;
using RPFO.Application.Interfaces;
using System.IO;

namespace XUnit.PRFO.API
{
    public class BaseAppTestFixture : WebApplicationFactory<Startup>
    {
        private ServiceProvider _serviceProvider;
        public IPaymentMethodService PaymentMethodService { get; private set; }

        public BaseAppTestFixture()
        {
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
        }
    }
}

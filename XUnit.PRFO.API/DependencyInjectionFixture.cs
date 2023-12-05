using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPFO.API;
using RPFO.Application.Implements;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnit.PRFO.API
{
    public class DependencyInjectionFixture : IDisposable
    {
        private readonly TestServer _server;
        public IServiceCollection Services { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IPaymentMethodService PaymentMethodService { get; private set; }
        public IPermissionService PermissionService { get; private set; }

        public ISaleService SaleService { get; private set; }
        public IGeneralSettingService SettingService { get; private set; }
        public IShiftService ShiftService { get; private set; }
        public ITableInforService TableInforService { get; private set; }
        public IPlacePrintService PlacePrintService { get; private set; }
        public IPlaceInforService PlaceInforService { get; private set; }

        public DependencyInjectionFixture()
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.IntegrationTest.json")
               .Build();

            var whb = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseConfiguration(configuration)
                .ConfigureTestServices(s =>
                {
                    Services = s;
                });

            _server = new TestServer(whb);
            ServiceProvider = Services.BuildServiceProvider();

            //Create sale order services
            PaymentMethodService = ServiceProvider.GetRequiredService<IPaymentMethodService>();
            SettingService = ServiceProvider.GetRequiredService<IGeneralSettingService>();
            SaleService = ServiceProvider.GetRequiredService<ISaleService>();
            ShiftService = ServiceProvider.GetRequiredService<IShiftService>();
            PermissionService = ServiceProvider.GetRequiredService<IPermissionService>();

            //Fnb table service
            TableInforService = ServiceProvider.GetRequiredService<ITableInforService>();
            PlacePrintService = ServiceProvider.GetRequiredService<IPlacePrintService>();

            PlaceInforService = ServiceProvider.GetRequiredService<IPlaceInforService>();
        }

        public void Dispose()
        {

        }
    }
}

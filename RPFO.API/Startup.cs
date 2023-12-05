using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using RPFO.API.Errors;
using RPFO.API.Helpers;
using RPFO.API.Hubs;
using RPFO.API.Middleware;
using RPFO.Application.AutoMapper;
using RPFO.Application.Implements;
using RPFO.Application.Interfaces;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using StackExchange.Redis;

namespace RPFO.API
{

    public class Startup
    {
        readonly string MyPolicy = "_myPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                //string redis = RPFO.Utilities.Helpers.Encryptor.DecryptString(Configuration.GetConnectionString("Redis"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                //var config = ConfigurationOptions.Parse(redis, true);
                //var pass = Configuration.GetConnectionString("RedisPass");
                //if (!string.IsNullOrEmpty(pass))
                //{
                //    var passEnc = RPFO.Utilities.Helpers.Encryptor.DecryptString(pass, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                //    config.Password = passEnc;
                //}
                string redis = RPFO.Utilities.Helpers.Encryptor.DecryptString(Configuration.GetConnectionString("Redis"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                var config = ConfigurationOptions.Parse(redis, true);
                config.ConnectTimeout = 600 * 1000;
              
                return ConnectionMultiplexer.Connect(config);
            });
             
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainToViewModelMappingProfile());
                cfg.AddProfile(new ViewModelToDomainMappingProfile());
            });
            //services.AddAuthenticationCore();

            services.AddSingleton<IFileProvider>(
             new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            //services.AddAutoMapper();
            services.AddAutoMapper(typeof(Startup));
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddCors();
            //services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
            //    builder
            //    .AllowAnyMethod()
            //    .AllowAnyHeader().SetIsOriginAllowed(origin => true)
            //    .AllowCredentials()
            //    .WithOrigins("http://localhost:4200");
            //}));
            //app.UseCors(x => x.WithOrigins().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());
            services.AddScoped<ConnectionFactory>();
            //services.AddTransient<SemaphoreWaiter>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILoyaltyRankService, LoyaltyRankService>();
            services.AddTransient<IMovementTypeService, MovementTypeService>();
            services.AddTransient<IToDoListService, ToDoListService>();
            services.AddTransient<ISalesPlanService, SalesPlanService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IStoreService, StoreService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerGroupService, CustomerGroupService>();
            services.AddTransient<ISaleService, SaleService>();
            services.AddTransient<IInvoiceService, InvoiceService>();
            services.AddTransient<IShippingService, ShippingService>();
            services.AddTransient<IShippingDivisionService, ShippingDivisionService>();
            services.AddTransient<IShiftService, ShiftService>();
            services.AddTransient<IBasketService, BasketService>();
            services.AddTransient<IBankInService, BankInService>();
            services.AddTransient<ITablePlaceService, TablePlaceService>();
            services.AddTransient<IPlaceInforService, PlaceInforService>();
            services.AddTransient<IMerchandiseCategoryService, MerchandiseCategoryService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<ILicenseTypeService, LicenseTypeService>();
            services.AddTransient<IPaymentMethodService, PaymentMethodService>();
            services.AddTransient<IStoreGroupService, StoreGroupService>();
            services.AddTransient<IStoreService, StoreService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IWarehouseService, WarehouseService>();
            services.AddTransient<IControlService, ControlService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<ITableInforService, TableInforService>();
            services.AddTransient<IStorePaymentService, StorePaymentService>();
            services.AddTransient<IFormatConfigService, FormatConfigService>();
            services.AddTransient<IBOMService, BOMService>();
            services.AddTransient<IVersionService, VersionService>();
            services.AddTransient<ICapacityService, CapacityService>();
            services.AddTransient<IGoodReceiptService, GoodReceiptService>();
            services.AddTransient<IGoodIssueService, GoodIssueService>();
            services.AddTransient<ITimeFrameService, TimeFrameService>();
            services.AddTransient<IPurchaseService, PurchaseService>();
            services.AddTransient<IGRPOService, GRPOService>();
            services.AddTransient<IPrepaidCardService, PrepaidCardService>();
            services.AddTransient<IShortcutService, ShortcutService>();
            services.AddTransient<ICurrencyService, CurrencyService>();
            services.AddTransient<IStoreCurrencyService, StoreCurrencyService>();
            services.AddTransient<IExchangeRateService, ExchangeRateService>();
            services.AddTransient<IDeliveryInforService, DeliveryInforService>();
            services.AddTransient<IBarcodeSetupService, BarcodeSetupService>();
            services.AddTransient<IItemRejectPaymentService, ItemRejectPaymentService>();
            services.AddTransient<IKeyCapService, KeyCapService>();
            services.AddTransient<IStoreClientService, StoreClientService>();
            services.AddTransient<IPriceListNameService, PriceListNameService>();
            services.AddTransient<IReasonService, ReasonService>();
            services.AddTransient<IDatasourceEditService, DatasourceEditService>();
            services.AddTransient<IBankTerminalService, BankTerminalService>();
            services.AddTransient<IPickupAmountService, PickupAmountService>();
            services.AddTransient<IReleaseNoteService, ReleaseNoteService>();
            services.AddTransient<IDevisionService, DevisionService>();

            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<ITaxService, TaxService>();
            services.AddTransient<IUomService, UomService>();
            services.AddTransient<IItemUomService, ItemUomService>();
            services.AddTransient<IItemSerialService, ItemSerialService>();
            services.AddTransient<IItemSerialStockService, ItemSerialStockService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<ILicenseService, LicenseService>();
            services.AddTransient<IPriorityPriceListService, PriorityPriceListService>();
            //services.AddTransient<IItemS, ItemUomService>();

            services.AddTransient<IInventoryCountingService, InventoryCountingService>();
            services.AddTransient<IInventoryPostingService, InventoryPostingService>();
            services.AddTransient<IItemListingService, ItemListingService>();
            services.AddTransient<IThirdPartyLogService, ThirdPartyLogService>();
            services.AddTransient<ILocalLogService, LocalLogService>();
            services.AddTransient<IEmployeeSalaryService, EmployeeSalaryService>();
            services.AddTransient<ISalesTargetService, SalesTargetService>();
            services.AddTransient<IEmployeeSalesTargetSummaryService, EmployeeSalesTargetSummaryService>();

            services.AddTransient<IPromotionService, PromotionService>();
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IInventoryTransferService, InventoryTransferService>();
            services.AddTransient<ICustomerGroupService, CustomerGroupService>();
            services.AddTransient<ILoyaltyService, LoyaltyService>();
            services.AddTransient<IPeripherallService, PeripherallService>();
            services.AddTransient<IStoreAreaService, StoreAreaService>();
            services.AddTransient<IUserStoreService, UserStoreService>();
            services.AddTransient<ITerminalPeripherallService, TerminalPeripherallService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IPriceListService, PriceListService>();
            services.AddTransient<IItemGroupService, ItemGroupService>();
            services.AddTransient<IItemStorageService, ItemStorageService>();
            services.AddTransient<ISaleTypeService, SaleTypeService>();
            services.AddTransient<IVoidOrderSettingService, VoidOrderSettingService>();
            services.AddTransient<IHolidayService, HolidayService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IVoucherTransactionService, VoucherTransactionService>();
            services.AddTransient<IGeneralSettingService, GeneralSettingService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IEndDateService, EndDateService>();
            services.AddTransient<IDenominationService, DenominationService>();
            services.AddTransient<IInvoiceInforService, InvoiceInforService>();
            services.AddTransient<ICurrencyRoundingOffService, CurrencyRoundingOffService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IItemVariantService, ItemVariantService>();

            services.AddTransient<IPurchaseRequestService, PurchaseRequestService>();
            services.AddTransient<IGoodsReturnService, GoodsReturnService>();
            services.AddTransient<IStoreWareHouseService, StoreWareHouseService>();
            services.AddTransient<ILicensePlateService, LicensePlateService>();
            services.AddTransient<IEmployeeTimekeepingService, EmployeeTimekeepingService>();
            services.AddTransient<IPlacePrintService, PlacePrintService>();
         
            services.AddControllers();//.AddNewtonsoftJson();

            //Remove null json

            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //});



            //Delivery 
            services.AddTransient<IDeliveryService, DeliveryService>();
            services.AddTransient<IDeliveryReturnService, DeliveryReturnService>();
            //Delivery Order
            services.AddTransient<IDeliveryOrderService, DeliveryOrderService>();

            services.AddTransient<IReceiptfromProductionService, ReceiptfromProductionService>();
            services.AddTransient<IProductionOrderService, ProductionOrderService>();

            services.AddTransient<ISalesChannelService, SalesChannelService>();
            services.AddTransient<IClientDisallowanceService, ClientDisallowanceService>();








            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToArray();
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errors);
                };
            });
            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RPFO.API", Version = "v1" });
            });

            //Add Authorize
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                           .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };

               });
            services.AddSignalR();
            //services.AddResponseCompression();
            services.AddResponseCompression(options =>
            {
                IEnumerable<string> MimeTypes = new[]
                {
                        // General
                        "text/plain",
                        "text/html",
                        "text/css",
                        "font/woff2",
                        "application/javascript",
                        "image/x-icon",
                        "image/png"
                };

                options.EnableForHttps = true;
                options.ExcludedMimeTypes = MimeTypes;
                options.Providers.Add<BrotliCompressionProvider>();
                //options.Providers.Add<GzipCompressionProvider>();
                //options.Providers.Add<CustomCompressionProvider>();
                //options.MimeTypes =
                //    ResponseCompressionDefaults.MimeTypes.Concat(
                //        new[] { "image/svg+xml" });
            });
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);
        }
        //public class DateTimeConverter : JsonConverter<DateTime>
        //{
        //    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //    {
        //        Debug.Assert(typeToConvert == typeof(DateTime));
        //        return DateTime.Parse(reader.GetString());
        //    }

        //    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        //    {
        //        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
        //    }
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();

            //}
            //else
            //{
            //    app.UseExceptionHandler(builder =>
            //    {
            //        builder.Run(async context =>
            //        {
            //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //            var error = context.Features.Get<IExceptionHandlerFeature>();
            //            if (error != null)
            //            {
            //                context.Response.AddApplicationError(error.Error.Message);
            //                await context.Response.WriteAsync(error.Error.Message);
            //            }
            //        });
            //    });
            //}
            app.UseMiddleware<ExceptionMiddleware>();
            //app.UseStatusCodePagesWithReExecute("/api/errors/{0}");
            app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPFO.API v1")); 
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPFO.API v1");
                c.RoutePrefix = "document";
                c.DocumentTitle = "RPFO.API v1";
                c.DefaultModelsExpandDepth(-1);
            });
            app.UseCors(x => x.WithOrigins().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());
            //app.UseCors("CorsPolicy");
            //"http://192.168.1.83:4200", "http://localhost:4200", "http://sapvn.com:4200/"
            app.UseHttpsRedirection();

            app.UseAuthentication();

            //, 
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(
            //      Path.Combine(env.ContentRootPath, "wwwroot")),
            //     RequestPath = "/api/wwwroot"
            // });

            app.UseRouting();
            app.UseResponseCompression();
            app.UseAuthorization();
            //app.UseMvc(m => m.SetTimeZoneInfo(TimeZoneInfo.Utc));
            //app.UseMvc( );
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignaRHub>("/api/pushNotification");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute("Spa", "{*url}", defaults: new { controller = "Fallback", action = "Index" });
            });

            app.UseEndpoints(endpoints =>
            {

                //endpoints.MapControllers().RequireCors(MyPolicy);
                //endpoints.MapRazorPages();

                endpoints.MapControllers();


            });

        }
    }
}


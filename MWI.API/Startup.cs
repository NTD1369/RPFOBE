using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RPFO.Application.AutoMapper;
using RPFO.Application.Implements;
using RPFO.Application.ImplementsMwi;
using RPFO.Application.Interfaces;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWI.API
{
    public class Startup
    {
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
                string redis = RPFO.Utilities.Helpers.Encryptor.DecryptString(Configuration.GetConnectionString("Redis"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                var config = ConfigurationOptions.Parse(redis, true);
                return ConnectionMultiplexer.Connect(config);
            });
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainToViewModelMappingProfile());
                cfg.AddProfile(new ViewModelToDomainMappingProfile());
            });
            //services.AddAuthenticationCore();


            //services.AddAutoMapper();
            services.AddAutoMapper(typeof(Startup));
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddCors();
            services.AddScoped<ConnectionFactory>();
            services.AddTransient(typeof(IGenericRepositoryMulti<,>), typeof(GenericRepositoryMulti<,>));
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddTransient<IStoreService, StoreService>();
            services.AddTransient<IGeneralSettingService, GeneralSettingService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IMasterDataService, MasterDataService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<RPFO.Application.InterfacesMwi.IInvoiceService, RPFO.Application.ImplementsMwi.InvoiceService>();
            services.AddTransient<IRpfoAPIService, RpfoAPIService>();
            services.AddTransient<IMwiAPIService, MwiAPIService>();
            services.AddTransient<ISalesService, SalesService>();
            //services.AddTransient<IRoleService, RoleService>();

            //services.AddMvc().AddNewtonsoftJson();
            //services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new MWI.API.Converter.TimeSpanToStringConverter()));
            //services.AddCors();
           
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MWI.API", Version = "v1" });
            });

            //Add Authorize
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options => {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                           .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };

               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MWI.API v1"));
            //}

            app.UseRouting();
            app.UseCors(x => x.WithOrigins().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());
            app.UseAuthentication();
            app.UseAuthorization();
             
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MWI.API v1");
                c.RoutePrefix = "document";
                c.DocumentTitle = "MWI.API v1";
                c.DefaultModelsExpandDepth(-1);
            });
        }
    }
}

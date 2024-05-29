using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using BaseAPI;
using Extensions;
using Models.AutoMapper;
using Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Services;
using Hangfire;
using Hangfire.SqlServer;
using Interface.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Log.Logger = new Serilog.LoggerConfiguration()
              .ReadFrom.Configuration(Configuration)
              .CreateLogger();

            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        readonly string SignalROrigins = "SignalROrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add import data
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
            //SQL
            services.AddDbContext<AppDbContext.AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SampleDbContext"))
                    .EnableSensitiveDataLogging();
            });
            //Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(typeof(AppAutoMapper).Assembly);

            services.AddHttpClient();

            //Using ..BaseAPI/ServiceExtensions
            services.ConfigureRepositoryWrapper();
            services.ConfigureService();
            services.ConfigureSwagger();

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder
                    .WithOrigins("https://localhost:5001")
                    .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   ;
                });
                options.AddPolicy(SignalROrigins,
                builder =>
                {
                    builder
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()
                   .SetIsOriginAllowed(hostName => true)
                   ;
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddSession(options =>
            {
                //// Set a short timeout for easy testing.
                //options.IdleTimeout = TimeSpan.FromSeconds(10);
                //options.Cookie.HttpOnly = true;
                //// Make the session cookie essential
                //options.Cookie.IsEssential = true;
            });

            var key = Encoding.ASCII.GetBytes(appSettings.secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });
            services.AddSignalR();


            services.AddControllers();

            // Configure API versioning
            services.AddApiVersioning(options =>
            {
                // Specify the default API version
                options.DefaultApiVersion = ApiVersion.Default;
                // Specify supported API versions
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
            //// 200 MB
            //const int maxRequestLimit = 209715200;
            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.MaxRequestBodySize = maxRequestLimit;
            //});
            //////nếu dùng kes
            ////services.Configure<KestrelServerOptions>(options =>
            ////{
            ////    options.Limits.MaxRequestBodySize = maxRequestLimit;
            ////});
            //services.Configure<FormOptions>(x =>
            //{
            //    x.ValueLengthLimit = maxRequestLimit;
            //    x.MultipartBodyLengthLimit = maxRequestLimit;
            //    x.MultipartHeadersLengthLimit = maxRequestLimit;
            //});

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddSerilog();

            //serviceProvider.MigrationDatabase(Configuration);

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "Upload")),
                RequestPath = string.Empty
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                     Path.Combine(env.ContentRootPath, "Download")),
                RequestPath = string.Empty
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "Template")),
                RequestPath = "/Template"
            });
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //         Path.Combine(env.ContentRootPath, "Upload/User")),
            //    RequestPath = "/User",
            //});

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //         Path.Combine(env.ContentRootPath, "Upload/QRCode")),
            //    RequestPath = "/QRCode",
            //});
            //dùng cho build static
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStaticHttpContext();

            app.UseSession();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthorization();


            //Hangfire
            app.UseHangfireDashboard();
            RecurringJob.AddOrUpdate<ICollectionPlanService>(
                "Create Collection Plan Every Month",
                x => x.CronJobCollectionPlan(),
               Cron.Daily);
            //RecurringJob.AddOrUpdate<IStudentPlanService>(
            //    "Cập nhật hết hạn buổi học - mỗi ngày cập nhật một lần",
            //    x => x.Expired(),
            //    "* */1 * * *");

            //# field #   meaning        allowed values
            //# -------   ------------   --------------
            //#    1      minute         0-59
            //#    2      hour           0-23
            //#    3      day of month   1-31
            //#    4      month          1-12 (or names, see below)
            //#    5      day of week    0-7 (0 or 7 is Sun, or use names)

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("../swagger/v1/swagger.json", "Elearn");
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "version 1.0");
                c.SwaggerEndpoint($"/swagger/v2/swagger.json", "version 2.0");
                c.InjectStylesheet("../css/swagger.min.css");
                c.RoutePrefix = "docs";
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DomainHub>("/hubs/notifications").RequireCors(SignalROrigins);
                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}"
                  );
            });
        }
    }
}

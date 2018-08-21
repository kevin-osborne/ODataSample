using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData;
using NLog.Extensions.Logging;
using OData.Controllers.OData;
using OData.Models;
using OData.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System;

namespace OData
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddFilter("Microsoft", LogLevel.Warning); // switch to Information to enable request tracing
                builder.AddFilter("System", LogLevel.Error);
                builder.AddFilter("Engine", LogLevel.Debug);
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc();
            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = PlatformServices.Default.Application.ApplicationBasePath;
            if (!path.Contains("OData.Tests"))
            {
                //configure NLog
                loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                loggerFactory.ConfigureNLog("nlog.config");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            IEdmModel model = GetEdmModel(app.ApplicationServices);

            ODataSimplifiedOptions odata_options = new ODataSimplifiedOptions(){ EnableWritingODataAnnotationWithoutPrefix = false };

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("*"));
            
            app.Map("/odata/api",
                api =>
                {
                    api.UseMvc(routeBuilder =>
                    {
                        routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

                        string odata_route_name = "ODataRoute";
                        routeBuilder.MapODataServiceRoute(odata_route_name, "data", a =>
                        {
                            a.AddService(Microsoft.OData.ServiceLifetime.Singleton, sp => model);
                            a.AddService<IODataPathHandler>(Microsoft.OData.ServiceLifetime.Singleton,
                                sp => new DefaultODataPathHandler());
                            a.AddService<IEnumerable<IODataRoutingConvention>>(
                                Microsoft.OData.ServiceLifetime.Singleton,
                                sp => ODataRoutingConventions.CreateDefaultWithAttributeRouting(odata_route_name,
                                    routeBuilder));
                            a.AddService<ODataSerializerProvider>(Microsoft.OData.ServiceLifetime.Singleton,
                                sp => new SampleODataSerializerProvider(sp, loggerFactory));
                            a.AddService<ODataDeserializerProvider>(Microsoft.OData.ServiceLifetime.Singleton,
                                sp => new DefaultODataDeserializerProvider(sp));
                            a.AddService<ILoggerFactory>(Microsoft.OData.ServiceLifetime.Singleton,
                                sp => loggerFactory);
                            a.AddService<ODataSimplifiedOptions>(Microsoft.OData.ServiceLifetime.Singleton,
                                sp => odata_options);
                            a.AddService<ODataPayloadValueConverter, SampleODataPayloadValueConverter>(Microsoft.OData
                                .ServiceLifetime.Singleton);
                        });
                        routeBuilder.EnableDependencyInjection();
                    });
                });

            var webSocketOptions = new Microsoft.AspNetCore.Builder.WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5),
                ReceiveBufferSize = 999999
            };
        }

        private static IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            builder.EntitySet<SampleObject>(nameof(SampleObject).ToLower()).EntityType.HasKey(s => s.Id).MediaType();
            var model = builder.GetEdmModel();
            return model;
        }
    }
}

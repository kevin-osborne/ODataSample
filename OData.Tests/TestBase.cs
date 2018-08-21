using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using NLog.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System;
using Xunit.Abstractions;
using Xunit;

namespace OData.Tests
{
    public class TestBase : IClassFixture<TestFixture>, IDisposable
    {
        protected TestFixture _fixture;

        public const string TestingCookieAuthentication = "TestCookieAuthentication";
        protected ILogger _logger;
        protected ILoggerFactory _loggerFactory;

        public TestServer Server { get; }

        public HttpClient Client { get; }

        public HttpClient UserClient
        {
            get
            {
                var client = Server.CreateClient();
                return client;
            }
        }

        public TestBase(TestFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;           

            var path = PlatformServices.Default.Application.ApplicationBasePath;
            path = Path.Combine(path, "..", "..", "..", "..", "OData");
            Server = new TestServer
            (
                new WebHostBuilder()
                    .UseContentRoot(path)
                    .UseStartup<TestStartup>()
            );
            Client = Server.CreateClient();
            _loggerFactory = Server.Host.Services.GetService<ILoggerFactory>();
            _loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            _loggerFactory.ConfigureNLog("nlog.config");
            _logger = _loggerFactory.CreateLogger(this.GetType());
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //_loggerFactory.Dispose();
            _fixture.Dispose();
        }
    }
}

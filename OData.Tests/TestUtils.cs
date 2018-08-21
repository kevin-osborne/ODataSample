using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNet.OData;

namespace OData.Tests
{
    class TestUtils
    {
        public static void DebugWrite(string message, [CallerMemberName] string memberName = "")
        {
            Debug.WriteLine(string.Format("[{0}]: {1}", memberName, message));
        }

        public static void ConsoleLog(ILogger pLogger, string message, [CallerMemberName] string memberName = "")
        {
            string log = string.Format("[{0}]: {1}", memberName, message);
            Console.WriteLine(log);
            pLogger.LogInformation(log);
        }
        
        public static Controller InitControllerForTest(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext
            {
                HttpContext = httpContext, 
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };
            controller.ControllerContext = new ControllerContext(actionContext);
            return controller;
        }

        public static ODataController InitODataControllerForTest(ODataController controller)
        {
            var httpContext = new DefaultHttpContext();            
            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };
            controller.ControllerContext = new ControllerContext(actionContext);            
            return controller;
        }

        public static string GetTestOutputFolder()
        {
            string result = "c:\\work\\temp\\pdf\\";
            if(!Directory.Exists(result))
            {
                result = Path.GetTempPath();
            }
            return result;
        }
    }

    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            T value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }
    }
}

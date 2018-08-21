using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OData.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Mime;
using System.Net;
using System.Threading.Tasks;
using System;

namespace OData.Controllers.OData
{
    public class SampleObjectController : ODataController, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly List<SampleObject> _list;

        public SampleObjectController(ILoggerFactory loggerFactory)
            : base()
        {
            _logger = loggerFactory.CreateLogger<SampleObjectController>();
            _loggerFactory = loggerFactory;
            _list = new List<SampleObject>();
            var o1 = new SampleObject
            {
                Id = 1, 
                SampleProperty = "Sample1", 
                AnotherProperty = "Another1" 
            };
            var o2 = new SampleObject
            {
                Id = 2, 
                SampleProperty = "Sample2", 
                AnotherProperty = "Another2" 
            };
            _list.Add(o1);
            _list.Add(o2);
        }

        [EnableQuery]
        [ODataRoute("sampleobject")]
        public List<SampleObject> Get()
        {
            _logger.LogInformation("[SampleObjectController: Get", "called...");
            return _list;
        }

        [HttpGet("data/sampleobject/{id}/content")]
        public IActionResult DownloadSampleObject(string id)
        {
            _logger.LogInformation("[SampleObjectController.DownloadSampleObject] called with id: " + id);
            try
            {
                string file_name = "pdf-sample.pdf";
                var bytes = System.IO.File.ReadAllBytes(file_name);
                var ms = new MemoryStream();
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;                
                return this.File(ms, "application/pdf", file_name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[SampleObjectController] Get");
            }

            _logger.LogInformation("[SampleObjectController: Get", "error; returning null");

            return NotFound();
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
            //_loggerFactory = null;
        }
    }
}
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using OData.Models;
using OData.Utils;
using System.Collections.Generic;
using System;

namespace OData.Serialization
{
    public class SampleODataSerializerProvider : DefaultODataSerializerProvider, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Dictionary<string, ODataEdmTypeSerializer> _EntitySerializers;

        public SampleODataSerializerProvider(IServiceProvider rootContainer, ILoggerFactory loggerFactory)
            : base(rootContainer)
        {
            _logger = loggerFactory.CreateLogger<ODataSerializerProvider>();
            _loggerFactory = loggerFactory;
            _EntitySerializers = new Dictionary<string, ODataEdmTypeSerializer>();
            _logger.LogTrace("[ODataSerializerProvider()] typeof(SampleObject).FullName: " + typeof(SampleObject).FullName);
            _EntitySerializers[typeof(SampleObject).FullName] = new SampleObjectEntitySerializer(this);
        }

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            _logger.LogTrace("[ODataSerializerProvider.GetEdmTypeSerializer] called...");
            if (edmType.IsEntity())
            {
                _logger.LogTrace("[ODataSerializerProvider.GetEdmTypeSerializer] edmType: " + edmType.ToString());
                string stripped_type = ODataUtils.StripEdmTypeString(edmType.ToString());
                _logger.LogTrace("[ODataSerializerProvider.GetEdmTypeSerializer] stripped_type: " + stripped_type);
                if (_EntitySerializers.ContainsKey(stripped_type))
                {
                    _logger.LogTrace("[ODataSerializerProvider.GetEdmTypeSerializer] serializer found for: " + stripped_type);
                    return _EntitySerializers[stripped_type];
                }
            }            
            return base.GetEdmTypeSerializer(edmType);
        }

        public override ODataSerializer GetODataPayloadSerializer(Type type, HttpRequest request)
        {
            
            _logger.LogDebug("[ODataSerializerProvider.GetODataPayloadSerializer] type: " + type);
            var serializer = new SampleODataPayloadSerializer(base.GetODataPayloadSerializer(type, request), _loggerFactory);
            return serializer;
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
        }
    }
}
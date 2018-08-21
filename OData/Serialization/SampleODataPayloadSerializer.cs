using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.OData;

namespace OData.Serialization
{
    public class SampleODataPayloadSerializer : ODataSerializer
    {
        private readonly ILogger _logger;
        private readonly ODataSerializer _serializer;

        public SampleODataPayloadSerializer(ODataSerializer serializer, ILoggerFactory loggerFactory)
            : base(serializer.ODataPayloadKind)
        {
            _logger = loggerFactory.CreateLogger<SampleODataPayloadSerializer>();
            _serializer = serializer;
        }

        public override void WriteObject(object graph, Type type, ODataMessageWriter messageWriter, ODataSerializerContext writeContext)
        {
            _logger.LogDebug("[ODataPayloadSerializer.WriteObject] type: " + type);
            _serializer.WriteObject(graph, type, messageWriter, writeContext);
        }
    }
}

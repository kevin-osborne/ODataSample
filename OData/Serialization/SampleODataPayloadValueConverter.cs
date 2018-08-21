using Microsoft.Extensions.Logging;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Serialization
{
    public class SampleODataPayloadValueConverter : ODataPayloadValueConverter
    {
        private readonly ILogger _logger;
        public SampleODataPayloadValueConverter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ODataPayloadValueConverter>();
        }

        public override object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            _logger.LogDebug("[ODataPayloadValueConverter.ConvertToPayloadValue] value: " + value);
            if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).ToString("R", CultureInfo.InvariantCulture);
            }

            return base.ConvertToPayloadValue(value, edmTypeReference);
        }

        public override object ConvertFromPayloadValue(object value, IEdmTypeReference edmTypeReference)
        {
            _logger.LogDebug("[ODataPayloadValueConverter.ConvertFromPayloadValue] value: " + value);
            if (edmTypeReference.IsDateTimeOffset() && value is string)
            {
                return DateTimeOffset.Parse((string)value, CultureInfo.InvariantCulture);
            }

            return base.ConvertFromPayloadValue(value, edmTypeReference);
        }
    }
}

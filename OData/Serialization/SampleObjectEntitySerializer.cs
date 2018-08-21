using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;
using OData.Models;
using System;

namespace OData.Serialization
{
    public class SampleObjectEntitySerializer : SampleODataResourceSerializer<SampleObject>
    {
        public SampleObjectEntitySerializer(ODataSerializerProvider serializerProvider) : base(serializerProvider)
        {
        }

        public override Uri BuildLinkForStreamProperty(SampleObject entity, ResourceContext context)
        {
            var path = context.Request.GetDisplayUrl();
            if (path.Contains("?"))
            {
                path = path.Substring(0, path.IndexOf("?"));
            }
            string url = string.Format("{0}/{1}/content", path, entity.Id);
            return new Uri(url, UriKind.Absolute);
        }

    }
}
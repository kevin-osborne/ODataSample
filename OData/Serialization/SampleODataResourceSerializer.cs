using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData;
using Microsoft.Data.OData;
using Microsoft.OData;
using System;

namespace OData.Serialization
{
    public abstract class SampleODataResourceSerializer<T> : ODataResourceSerializer where T : class
    {
        protected SampleODataResourceSerializer(ODataSerializerProvider serializerProvider)
            : base(serializerProvider)
        {
        }

        public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            var resource = base.CreateResource(selectExpandNode, resourceContext);

            var instance = resourceContext.ResourceInstance as T;

            if (instance != null)
            {
                resource.MediaResource = new Microsoft.OData.ODataStreamReferenceValue
                {
                    ContentType = ContentType,
                    ReadLink = BuildLinkForStreamProperty(instance, resourceContext)
                };
            }

            return resource;
        }

        public virtual string ContentType
        {
            //get { return "application/octet-stream"; }
            get { return "application/pdf"; }
        }

        public abstract Uri BuildLinkForStreamProperty(T entity, ResourceContext resourceContext);
    }
}

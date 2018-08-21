using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using OData.Tests;

namespace OData.Tests.OData
{
    public class ODataRouteTest : TestBase
    {
        public ODataRouteTest(ITestOutputHelper testOutputHelper, TestFixture fixture)
            : base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task TestMetadata()
        {
            var response = await UserClient.GetAsync("/odata/api/data");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            TestUtils.ConsoleLog(_logger, string.Format("responseString: {0}", responseString));

            Assert.Contains("EntitySet", responseString);
        }

        [Fact]
        public async Task TestEntitySampleObject()
        {
            
            var response = await UserClient.GetAsync("/odata/api/data/sampleobject");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            TestUtils.ConsoleLog(_logger, string.Format("responseString: {0}", responseString));

            Assert.Contains("Another1", responseString);
        }

        [Fact]
        public async Task TestQuerySampleObject()
        {
            var response = await UserClient.GetAsync("/odata/api/data/sampleobject?$top=1&$orderby=Id%20desc");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            TestUtils.ConsoleLog(_logger, string.Format("responseString: {0}", responseString));

            Assert.Contains("Another2", responseString);
        }

        [Fact]
        public async Task TestMediaLinkSampleObject()
        {
            var response = await UserClient.GetAsync("/odata/api/data/sampleobject?$top=1&$orderby=id%20desc");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            TestUtils.ConsoleLog(_logger, string.Format("responseString: {0}", responseString));

            Assert.Contains("odata.mediaReadLink", responseString);
            Assert.Contains("odata.mediaContentType", responseString);
            Assert.Contains("application/pdf", responseString);
            Assert.Contains("content", responseString);
            Assert.DoesNotContain("orderby", responseString);
            Assert.Contains("@odata", responseString);

        }

        [Fact]
        public async Task TestMediaLinkDownloadSampleObject()
        {
            var response = await UserClient.GetAsync("/odata/api/data/sampleobject/1/content");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("%PDF-1.3", responseString);

        }

    }
}

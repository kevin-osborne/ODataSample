using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OData.Controllers.OData;
using OData.Models;
using OData.Tests;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Xunit.Abstractions;
using Xunit;

namespace OData.Tests.OData
{
    public class SampleObjectUnitTest : TestBase
    {
        public SampleObjectUnitTest(ITestOutputHelper testOutputHelper, TestFixture fixture)
            : base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public void TestSampleObjectGetList()
        {
            int count = 0;
           
            // arrange
            SampleObjectController controller = TestUtils.InitODataControllerForTest(new SampleObjectController(_loggerFactory)) as SampleObjectController;

            // act
            var result = controller.Get() as List<SampleObject>;

            // assert
            Assert.NotNull(result);
            TestUtils.ConsoleLog(_logger, result.ToString());
            TestUtils.ConsoleLog(_logger, result.Count.ToString());
            Assert.True(result.Count > 0);
            SampleObject obj = result.Where(x=> x.SampleProperty.Contains("1")).FirstOrDefault() ;
            Assert.True(obj != null);
            string json = JsonConvert.SerializeObject(obj);
            TestUtils.ConsoleLog(_logger, json);
            count = result.Count;
            
            Assert.True(count > 0);
        }

        [Fact]
        public void TestSampleObjectGet_1()
        {
            // arrange
            SampleObjectController controller = TestUtils.InitODataControllerForTest(new SampleObjectController(_loggerFactory)) as SampleObjectController;

            // act
            var result = controller.Get() as List<SampleObject>;

            // assert
            Assert.NotNull(result);
            TestUtils.ConsoleLog(_logger, result.ToString());
            TestUtils.ConsoleLog(_logger, result.Count.ToString());
            Assert.True(result.Count > 0);
            SampleObject obj = result[0] as SampleObject;
            Assert.NotNull(obj);
            TestUtils.ConsoleLog(_logger, obj.Id.ToString());
            Assert.True(obj.Id > 0);
        }

        [Fact]
        public void TestSampleObjectDownload_1()
        {
            // arrange
            SampleObjectController controller = TestUtils.InitODataControllerForTest(new SampleObjectController(_loggerFactory)) as SampleObjectController;

            // act
            var result = controller.DownloadSampleObject("?") as FileStreamResult;

            // assert
            Assert.NotNull(result);
            TestUtils.ConsoleLog(_logger, result.ToString());
            Assert.NotNull(result);
            TestUtils.ConsoleLog(_logger, result.FileDownloadName);
            Assert.NotNull(result.ContentType);
            TestUtils.ConsoleLog(_logger, result.ContentType);
            Assert.NotNull(result.FileStream);
            TestUtils.ConsoleLog(_logger, result.FileStream.Length.ToString());
            Assert.True(result.FileStream.Length == 7945);
        }

    }
}

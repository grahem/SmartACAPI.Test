using System;
using SmartACAPI;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Amazon.DynamoDBv2.DataModel;
using SmartACDeviceAPI.Controllers;
using SmartACDeviceAPI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartACDeviceAPI.Services;
using SmartACDeviceAPI.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace SmartACAPI.Test
{
    public class DeviceAuthControllerTest
    {

        private DeviceAuthZController controller;

        public DeviceAuthControllerTest()
        {
            var service = new Mock<IDeviceAuthZService>();
            service.Setup(_ => _.Authorize(It.IsAny<string>(), It.IsAny<string>()))
                   .Returns(Task.FromResult<string>("{authToken}"));

            var controllerLogger = new Mock<ILogger<DeviceAuthZController>>();
            controller = new DeviceAuthZController(service.Object, controllerLogger.Object);

        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Post_With_Valid_Credentials_Returns_Valid()
        {
            DeviceAuthenticationModel model = new DeviceAuthenticationModel { SerialNumber = "123", Secret = "123" };
            var x = await controller.Authorize(model);
            Assert.True(x is OkObjectResult);
        }
    }
}

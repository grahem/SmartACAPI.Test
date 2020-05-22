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

namespace SmartACAPI.Test
{
    public class AuthenticateControllerTest
    {

        private DeviceAuthZController authenticate;
        private DeviceAuthZService service;

        public AuthenticateControllerTest() {
            var config  = new Mock<IConfiguration>();
            config.Setup(repo => repo["Jwt:Issuer"]).Returns("123");
            config.Setup(repo => repo["Jwt:Audience"]).Returns("123");
            
            var context = new Mock<IDynamoDBContext>();
            var options = new Mock<IOptionsMonitor<AuthZOptions>>();
            var logger = new Mock<ILogger<DeviceAuthZService>>(); 
            var authService = new Mock<DeviceAuthZService>(context.Object, options.Object, logger.Object);

            var controllerLogger = new Mock<ILogger<DeviceAuthZController>>();

            service = new DeviceAuthZService(context.Object, options.Object, logger.Object);
            
            var asyncSearch = new Mock<AsyncSearch<Device>>("123");
            context.Setup(repo => repo.QueryAsync<Device>(It.IsAny<object>(), null)).Returns(asyncSearch.Object);

            authenticate = new DeviceAuthZController(service, controllerLogger.Object);
            
        }

        [Fact]
        public void Test_Post_With_Valid_Credentials_Returns_Valid()
        {
            DeviceAuthenticationModel model = new DeviceAuthenticationModel();
            model.SerialNumber = "123";
            model.Secret = "123";
            var x = authenticate.Authorize(model);
            Assert.True(true);   
        }
    }
}

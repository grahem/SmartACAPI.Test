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
using SmartACDeviceAPI.Exceptions;

namespace SmartACAPI.Test
{
    public class UserAuthControllerTest
    {

        private UserAuthController controller;
        private Mock<IUserAuthZService> service;

        public UserAuthControllerTest()
        {
            service = new Mock<IUserAuthZService>();
            var controllerLogger = new Mock<ILogger<UserAuthController>>();
            controller = new UserAuthController(service.Object, controllerLogger.Object);

        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Authorize_With_Valid_Credentials_Returns_OK()
        {
            service.Setup(_ => _.Authorize(It.IsAny<string>(), It.IsAny<string>()))
                   .Returns(Task.FromResult<string>("{authToken}"));
            var x = await controller.Authorize(new UserAuthenticationModel{ Username = "123", Password = "123"});
            Assert.True(x is OkObjectResult);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Authorize_With_Invalid_Credentials_Returns_BadRequest()
        {
            service.Setup(_ => _.Authorize(It.IsAny<string>(), It.IsAny<string>()))
                   .Returns(Task.FromResult<string>(null));
            var x = await controller.Authorize(new UserAuthenticationModel{ Username = "123", Password = "123"});
            Assert.True(x is BadRequestResult);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Authorize_With_Failed_Credentials_Throws_AuthZException()
        {
            service.Setup(_ => _.Authorize(It.IsAny<string>(), It.IsAny<string>()))
                   .Throws(new AuthZException());
            var x = await controller.Authorize(new UserAuthenticationModel{ Username = "123", Password = "123"});
            Assert.True(x is UnauthorizedResult);
        }
    }
}

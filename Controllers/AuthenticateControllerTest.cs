using System;
using SmartACAPI;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Amazon.DynamoDBv2.DataModel;
using SmartACDeviceAPI.Controllers;
using SmartACDeviceAPI.Models;
using Microsoft.Extensions.Logging;

namespace SmartACAPI.Test
{
    public class AuthenticateControllerTest
    {

        private readonly ILogger<AuthenticateController> logger;

        public AuthenticateControllerTest() {
            var config  = new Mock<IConfiguration>();
            config.Setup(repo => repo["Jwt:Issuer"]).Returns("123");
            config.Setup(repo => repo["Jwt:Audience"]).Returns("123");
            
            var context = new Mock<IDynamoDBContext>();
            var authrnticate = new AuthenticateController(config, context, logger);
        }

        [Fact]
        public void Test_Post_With_Valid_Credentials_Returns_Valid()
        {
            
        }
    }
}

using Moq;
using SmartACDeviceAPI.Services;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartACDeviceAPI.Options;
using SmartACDeviceAPI.Models;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using SmartACDeviceAPI.Exceptions;

namespace SmartACDeviceAPI.Test.Services
{

    public class DeviceZServiceTest
    {

        private readonly DeviceAuthZService service;

        private readonly Mock<IDynamoDBContext> dbContext;
        private readonly Device badDevice = new Device { SerialNumber = "Bad", Secret = "Secret" };
        private readonly Device goodDevice = new Device { SerialNumber = "Good", Secret = "Secret" };

        public DeviceZServiceTest()
        {
            this.dbContext = new Mock<IDynamoDBContext>();
            var logger = new Mock<ILogger<DeviceAuthZService>>();
            var options = new Mock<IOptionsMonitor<AuthZOptions>>();
            options.Setup(options => options.CurrentValue).Returns(new AuthZOptions { SecretKey = "1234567891011121314" });

            this.service = new DeviceAuthZService(dbContext.Object, options.Object, logger.Object);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Bad_Device_Can_Not_Authenticate()
        {

            dbContext.Setup(context => context.LoadAsync<Device>(badDevice.SerialNumber, It.IsAny<CancellationToken>()))
                     .Returns(() => Task.FromResult<Device>(null));
            var val = await service.Authorize(badDevice.SerialNumber, badDevice.Secret);
            Assert.True(val == null);
        }
         [Fact, Trait("Category", "Test")]
        public async void Test_Bad_Device_Secret_Can_Not_Authenticate()
        {

            dbContext.Setup(context => context.LoadAsync<Device>(badDevice.SerialNumber, It.IsAny<CancellationToken>()))
                     .Returns(() => Task.FromResult<Device>(new Device{Secret = "Bad Secret"}));
            await Assert.ThrowsAsync<AuthZException>(() => service.Authorize(badDevice.SerialNumber, badDevice.Secret));
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Good_Device_Can_Authorize()
        {
            //ensure good password returns string.
            dbContext.Setup(context => context.LoadAsync<Device>(goodDevice.SerialNumber, CancellationToken.None))
                     .Returns(() => Task.FromResult<Device>(goodDevice));
            var val = await service.Authorize(goodDevice.SerialNumber, goodDevice.Secret);
            Assert.True(val.Length == 165);
        }

    }
}